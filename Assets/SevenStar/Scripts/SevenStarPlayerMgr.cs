using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SevenStarPlayerMgr : MonoBehaviour
{
    [Header("Game Players")]
    public GamePlayerInfo[] m_Players;
    public UserSeat[] m_UserSeats;
    public int m_MyPlayerIdx = -1;
    public int m_PlayerNum = -1;
    public GamePlayerInfo m_PlayerSelf = null;

    private SevenStarLogic m_Logic;

    private void Awake()
    {
        m_Logic = SevenStarLogic.Instance;
        if (m_Logic.m_SSPlayerMgr == null)
            m_Logic.m_SSPlayerMgr = this;
    }

    public void Init()
    {
        InitUserSeats();
        RefreshPlayerList(true);
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////// Refresh Players //////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void RefreshPlayerList(bool First)
    {
        if (First)
        {
            StartCoroutine(GetPlayerList(First));
        }
        else
        {
            TexasHoldemClient.Instance.Send_int(Protocols.RoomPlayerList, m_Logic.m_RoomIdx);
        }
    }

    IEnumerator GetPlayerList(bool First)
    {
        TexasHoldemClient c = TexasHoldemClient.Instance;
        if (First)
            c.Send_int(Protocols.RoomPlayerList, m_Logic.m_RoomIdx);
        RecvPacketObject obj = null;

        int i, j;
        while (obj == null)
        {
            obj = c.PopPacketObject(Protocols.RoomPlayerList);
            yield return null;
        }


        List<int> UserList = null;
        ParserGame.GetPlayerList(obj, ref UserList);
        if (UserList == null)
        {
            m_Logic.Client.SendOutRoom();
            SceneManager.LoadSceneAsync("2_Lobby");
            yield break;
        }
        for (i = 0; i < UserList.Count; i++)
        {
            if (UserList[i] == -1)
                continue;
            c.SendGetUserInfo(UserList[i]);
        }

        if (First)  // 난입시 처음 세팅 - 바닥패, 사용자 베팅정보;;
        {
            // 바닥패;
            c.Send_int(Protocols.GetOnCard, 0);
            obj = null;
            while (obj == null)
            {
                obj = c.PopPacketObject(Protocols.GetOnCard);
                yield return null;
            }
            byte[] Card = new byte[5];
            ParserGame.GetOnCard(obj, ref Card); // 바닥패 겟;
            if (Card != null)
            {
                SevenStarPlayMgr playMgr = SevenStarLogic.Instance.m_SSPlayMgr;
                for (int k = 0; k < 5; k++)
                {
                    if (Card[k] == 0)
                        break;
                    CardShapeType type = (CardShapeType)playMgr.GetCardType(Card[k]);
                    int num = playMgr.GetCardNum(Card[k]);
                    playMgr.m_CommunityCard.SetComCardInfo(k, type, num);
                    playMgr.m_CommunityCard.OpenComCard(k);
                    playMgr.m_CommunityCard.SetComCardToInitPos(k);
                }
            }
        }
        
        int[] idxs = UserList.ToArray();
        j = idxs.Length;

        // check my idx
        int playerCount = 0;
        m_MyPlayerIdx = ClientObject.Instance.m_UserIdx;
        for (i = 0; i < j; i++)
        {
            if (idxs[i] == m_MyPlayerIdx)
                m_PlayerNum = i;
            if (idxs[i] > -1)
                playerCount++;
        }

        m_Logic.m_PlayerCount = 0;
        int playerSeatNum = 0;
        for (i = 0; i < j; i++)
        {
            // calculate player seat num
            playerSeatNum = (m_Players.Length - m_PlayerNum + i) % m_Players.Length;

            // Set user to player seat
            m_Players[playerSeatNum].SetUserIndex(idxs[i]);
            int num= idxs[i] == -1 ? -1 : i; ;
            m_Players[playerSeatNum].SetNum(num);
            m_Players[playerSeatNum].m_UserSeat.m_SeatNum = num;

            // Set player self
            if (idxs[i] == m_MyPlayerIdx)
                m_PlayerSelf = m_Players[playerSeatNum];
            if(idxs[i] != -1)
                m_Logic.m_PlayerCount++;
        }

        // 난입시 플레이어 베팅 정보요청;
        if (First)
        {
            c.SendPlayInfo(SevenStarLogic.Instance.m_RoomIdx);
            m_Logic.Client.Send_int(Protocols.TestRoomInComplete, 0);
        }
        StartCoroutine(GetPlayerList(false));
        
    }

    public void RemovePlayer(int idx)
    {
        StartCoroutine(RemovePlayerRoutine(idx));
    }

    IEnumerator RemovePlayerRoutine(int idx)
    {
        yield return new WaitForEndOfFrame();
        GamePlayerInfo player = GetExistPlayerFromIdx(idx);
        player.ResetPlayer(); 
        player.m_UserSeat.InitData();
        player.m_IsInitPlaying = false;
        //yield return GetPlayerList(false);
        TexasHoldemClient.Instance.Send_int(Protocols.RoomPlayerList, m_Logic.m_RoomIdx);
        yield return null;
    }



    //////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////// Common method  /////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////

    private void InitUserSeats()
    {
        for (int i = 0; i < m_Players.Length; i++)
        {
            m_Players[i].m_UserSeat.InitData();
        }
        m_Logic.m_SSPlayMgr.m_PotMoneyText.text = "0";
    }

    public GamePlayerInfo GetExistPlayerFromIdx(int userIdx)
    {
        GamePlayerInfo player = null;
        for (int i = 0; i < m_Players.Length; i++)
        {
            if (m_Players[i].m_PlayerIdx == userIdx)
            {
                player = m_Players[i];
                break;
            }
        }
        return player;
    }

    public GamePlayerInfo GetExistPlayerFromNum(int userNum)
    {
        GamePlayerInfo player = null;
        for (int i = 0; i < m_Players.Length; i++)
        {
            if (m_Players[i].m_PlayerNum == userNum)
            {
                player = m_Players[i];
                break;
            }
        }
        return player;
    }

    public void SetDealerMarkActive(int userIdx, bool isActive)
    {
        StartCoroutine(SetDealerMarkActiveRoutine(userIdx, isActive));
    }

    private IEnumerator SetDealerMarkActiveRoutine(int userIdx, bool isActive)
    {
        yield return new WaitForEndOfFrame();
        GamePlayerInfo player_DL = GetExistPlayerFromIdx(userIdx);
        while (player_DL == null)
        {
            player_DL = GetExistPlayerFromIdx(userIdx);
            yield return null;
        }
        player_DL.m_UserSeat.SetActiveDLMark(isActive);
    }

    public void SetHoldCard(int userIdx, byte card1, byte card2)
    {
        StartCoroutine(SetHoldCardRoutine(userIdx, card1, card2));
    }

    private IEnumerator SetHoldCardRoutine(int userIdx, byte card1, byte card2)
    {
        yield return new WaitForEndOfFrame();
        GamePlayerInfo player = GetExistPlayerFromIdx(userIdx);
        while (player == null)
        {
            player = GetExistPlayerFromIdx(userIdx);
            yield return null;
        }
        player.SetHandCard1(card1);
        player.SetHandCard2(card2);

        // my player
        // player card rank text
        player.SetCardRankText();
        // my cover card off
        if (userIdx == m_MyPlayerIdx)
            player.m_UserSeat.SetHandCoverCardActive(false);
    }

    public void SetResultCard(int UserIdx, byte[] card)
    {
        for (int i = 0; i < m_Players.Length; i++)
        {
            if (m_Players[i].m_PlayerIdx == UserIdx)
            {
                m_Players[i].SetResultCard(card);
                return;
            }
        }
    }

    /////////////////////////////////////////////////////////////////
    // first turn, in free flop turn
    // set blind player - SS,BB and DL
    ////////////////////////////////////////////////////////////////

    public void SetBlindBet(int smallBlind, int bigBlind)
    {
        //renew user info
        UserBetting(smallBlind, m_Logic.m_SSPlayMgr.m_SBMoney,true);
        GamePlayerInfo player_SB = GetExistPlayerFromIdx(smallBlind);
        player_SB.m_UserSeat.SetBlindIcon(1);

        //renew user info
        UserBetting(bigBlind, m_Logic.m_SSPlayMgr.m_BBMoney,true);
        GamePlayerInfo player_BB = GetExistPlayerFromIdx(bigBlind);
        player_BB.m_UserSeat.SetBlindIcon(2);
    }
    
    //
    // after betting ui setting
    //
    public void UserBetting(int idx, UInt64 bettingMoney, bool isFirst)
    {
        TexasHoldemClient.Instance.SendGetUserInfo(idx);
        UserInfo info = UserMgr.Instance.GetUserInfo(idx);
        GamePlayerInfo player = GetExistPlayerFromIdx(idx);

        // user betting eff
        StartCoroutine(BettingEffRoutine(player,bettingMoney,info.UserMoney, isFirst));
    }

    IEnumerator BettingEffRoutine(GamePlayerInfo player, UInt64 bettingMoney, UInt64 extraMoney, bool isFirst)
    {
        UInt64 oldPrevCallMoney = SevenStarLogic.Instance.m_SSPlayMgr.m_PrevCallMoney;
        UInt64 calBettingMoney = player.m_BettedMoney + bettingMoney;
        SevenStarLogic.Instance.m_SSPlayMgr.m_PrevCallMoney = calBettingMoney;
        // check if betting type check
        if (bettingMoney!=0)
            yield return player.m_UserSeat.m_BettingMoneyEff.BettingEff();
        player.SetUserBetting(oldPrevCallMoney, bettingMoney, extraMoney);
        if(isFirst == false)
            player.SetBettingTimer(0);
        yield return null;
    }


    public void SetBettingTimer(int idx)
    {
        for (int i = 0; i < m_Players.Length; i++)
        {
            if (m_Players[i].m_PlayerIdx == -1)
                continue;
            if (m_Players[i].m_PlayerIdx == idx)
            {
                m_Players[i].m_UserSeat.m_BettingTimer.StartTimer();
                m_Players[i].m_UserSeat.m_TurnImage.SetActive(true);
            }
        }
    }

}
