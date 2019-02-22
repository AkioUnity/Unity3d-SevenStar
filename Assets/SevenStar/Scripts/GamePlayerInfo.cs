using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayerInfo : MonoBehaviour {

    public UserSeat m_UserSeat;
    public int m_PlayerIdx=-1;
    public int m_PlayerNum=-1;
    public int m_Score = 0;
    public UInt64 m_BettedMoney = 0;

    public byte m_HandCard1;
    public byte m_HandCard2;
    public byte[] m_ResultCard = null;
    public List<byte> m_PlayerCardProg = null;

    public bool m_IsInitPlaying = false;
    public bool m_IsFold = false;
    public bool m_IsWinner = false;
    public UInt64 m_WinMoney = 0;

    public CardData m_CardData;

    private void Awake()
    {
        if (m_UserSeat == null)
            m_UserSeat = GetComponent<UserSeat>();
        m_UserSeat.m_Player = this;
        m_CardData = new CardData();
        m_PlayerCardProg = new List<byte>();
        Init();
    }

    public void Init()
    {
        m_PlayerIdx = -1;
        m_PlayerNum = -1;
    }

    public void ResetPlayer()
    {
        m_Score = 0;
        m_BettedMoney = 0;
        m_WinMoney = 0;
        m_HandCard1 = m_HandCard2 = 0;
        m_ResultCard = null;
        m_PlayerCardProg.Clear();
        
        m_IsFold = false;
        m_IsWinner = false;
        m_UserSeat.m_UserBlindIcon.IconInit();
        m_UserSeat.SetRankOfCard("-");
        m_UserSeat.ReadyBetting();
        m_UserSeat.SetAllinMark(false);
        m_UserSeat.m_TurnImage.SetActive(false);
        m_UserSeat.m_ResultMark.gameObject.SetActive(false);
        if (m_PlayerIdx > -1)
        {
            TexasHoldemClient.Instance.SendGetUserInfo(m_PlayerIdx);
            m_UserSeat.m_BettingTimer.gameObject.SetActive(true);
            m_UserSeat.m_BettingTimer.SetTimerPer(0);
            m_UserSeat.SetHandCardActive(true);
            m_UserSeat.SetHandCoverCardActive(true);
            m_IsInitPlaying = true;
        }
    }

    public void SetNum(int n)
    {
        m_PlayerNum = n;
    }

    public void SetFold(int uidx, bool fold)
    {
        if (uidx != m_PlayerIdx) return;
        m_IsFold = fold;
        m_UserSeat.m_BettingTimer.gameObject.SetActive(false);
        m_UserSeat.m_TurnImage.gameObject.SetActive(false);
        m_UserSeat.SetHandCardActive(false);

        int gender = AvataMgr.Instance.GetAvataGender(m_UserSeat.m_AvataIdx);
        SoundMgr.Instance.PlayVoiceSound(VoiceFXType.Fold, gender);
    }

    private void Update()
    {
        Set(UserMgr.Instance.GetUserInfo(m_PlayerIdx));
    }

    public void SetUserIndex(int Index)
    {
        m_PlayerIdx = Index;
    }

    void Set(UserInfo info)
    {
        if (info == null)
        {
            //m_UserSeat.SetUserName("No User");
            m_UserSeat.SetUserName("");
            //m_UserSeat.SetMoney(0);
            m_UserSeat.m_UserMoneyText.text = "";
            m_UserSeat.SetAvata(-1,false);
            m_UserSeat.m_AvataIdx = -1;
            m_UserSeat.SetHandCardActive(false);
            m_UserSeat.SetAllinMark(false);
        }
        else
        {
            m_UserSeat.SetUserName(info.UserName);
            m_UserSeat.SetMoney(info.UserMoney);
            m_UserSeat.SetAvata(info.Avatar, m_IsFold);
            m_UserSeat.m_AvataIdx = info.Avatar;
        }
    }

    public void SetHandCard1(byte card)
    {
        m_HandCard1 = card;
        CardShapeType type = (CardShapeType)SevenStarLogic.Instance.m_SSPlayMgr.GetCardType(card);
        int num = SevenStarLogic.Instance.m_SSPlayMgr.GetCardNum(card);
        m_UserSeat.m_HandCardObj1.SetCardSprite(type, num);
        // set player card progress
        SetPlayerCardProg(card);
    }

    public void SetHandCard2(byte card)
    {
        m_HandCard2 = card;
        CardShapeType type = (CardShapeType)SevenStarLogic.Instance.m_SSPlayMgr.GetCardType(card);
        int num = SevenStarLogic.Instance.m_SSPlayMgr.GetCardNum(card);
        m_UserSeat.m_HandCardObj2.SetCardSprite(type, num);
        // set player card progress
        SetPlayerCardProg(card);
    }

    public void SetHandCardMovingStartPos()
    {
        m_UserSeat.m_HandCardObj1.SetCardMovingStartPos();
        m_UserSeat.m_HandCardObj2.SetCardMovingStartPos();
    }

    public void SetBettingTimer(float per)
    {
        //Debug.Log("Set Betting Timer per : " + per+" / idx"+m_PlayerIdx);
        m_UserSeat.m_BettingTimer.SetTimerPer(per);

        if(per==0)
            m_UserSeat.m_TurnImage.SetActive(false);
        else
            m_UserSeat.m_TurnImage.SetActive(true);
    }

    public void SetUserBetting(UInt64 prevCallMoney, UInt64 bettingMoney, UInt64 extraMoney)
    {
        UInt64 calBettingMoney = m_BettedMoney + bettingMoney;

        if (prevCallMoney == calBettingMoney)
        {
            // call or check
            m_UserSeat.SetCallBetBall_Image(true);
        }
        else if (prevCallMoney < calBettingMoney)
        {
            if (extraMoney == 0)
            {
                // all in
                m_UserSeat.SetCallBetBall_Image(false);
            }
            else
            {
                // raise
                m_UserSeat.SetCallBetBall_Image(false);
            }
        }

        m_UserSeat.SetBettingMoney(calBettingMoney);
        m_BettedMoney = calBettingMoney;
    }

    public void SetPlayerCardProg(byte card)
    {
        if(m_PlayerCardProg.Count<7)
            m_PlayerCardProg.Add(card);
    }

    public void SetCardRankText()
    {
        //if (m_UserSeat.m_RankOfCardText == null)
          //  return;
        if(m_IsFold == true)
        {
            m_UserSeat.SetRankOfCard("-");
            return;
        }
        byte[] cards = m_PlayerCardProg.ToArray();
        string scoreString = m_CardData.GetScoreString(cards, ref cards);
        m_UserSeat.SetRankOfCard(scoreString);
    }

    public void SetResultCard(byte[] card)
    {
        byte[] resultCard = new byte[5];
        Array.Copy(card, resultCard, 5);
        m_ResultCard = resultCard;
        m_Score = m_CardData.GetScore(m_ResultCard, ref resultCard);

        m_HandCard1 = card[5];
        CardShapeType type = (CardShapeType)SevenStarLogic.Instance.m_SSPlayMgr.GetCardType(m_HandCard1);
        int num = SevenStarLogic.Instance.m_SSPlayMgr.GetCardNum(m_HandCard1);
        m_UserSeat.m_HandCardObj1.SetCardSprite(type, num);

        m_HandCard2 = card[6];
        type = (CardShapeType)SevenStarLogic.Instance.m_SSPlayMgr.GetCardType(m_HandCard2);
        num = SevenStarLogic.Instance.m_SSPlayMgr.GetCardNum(m_HandCard2);
        m_UserSeat.m_HandCardObj2.SetCardSprite(type, num);

        
    }

    public void SetWinner(int uidx, UInt64 Money)
    {
        if (uidx != m_PlayerIdx)
            return;
        m_IsWinner = true;
        m_WinMoney = Money;
        m_UserSeat.SetResultMark(true);
        m_UserSeat.SetHandCoverCardActive(false);
        m_UserSeat.m_BettingTimer.gameObject.SetActive(false);
        // winner money eff
        Vector3 targetPos = m_UserSeat.m_Avata.gameObject.transform.position;
        SevenStarLogic.Instance.m_SSPlayMgr.StartResultMoneyRoutine(targetPos);
    }

    public void SidePotReceiver(int uidx, UInt64 Money)
    {
        if (uidx != m_PlayerIdx)
            return;
        if (Money <= 0)
            return;
        //Debug.LogError("side pot receiver idx : " + uidx + "/ money : " + Money);
        // winner money eff
        Vector3 targetPos = m_UserSeat.m_Avata.gameObject.transform.position;
        SevenStarLogic.Instance.m_SSPlayMgr.StartResultMoneyRoutine(targetPos);
    }
}
