using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityUtil;


public class SevenStarLogic : UtilHalfSingleton<SevenStarLogic>
{
    public static bool m_ApplicationPauseTrigger;

    public SevenStarPlayerMgr m_SSPlayerMgr;
    public SevenStarPlayMgr m_SSPlayMgr;

    private TexasHoldemClient m_Client = null;
    private Game_Client m_GameClient = new Game_Client();
    public TexasHoldemClient Client { get { return m_Client; } }

    [Header("Game Status")]
    public int m_RoomIdx = -1;
    public int m_BettingReadyIdx = -1;
    public int m_TurnCount = 0;
    public int m_PlayerCount = 0;
    public bool m_IsMyTurn = false;
    public bool m_IsGameStart = false; // 난입용;;

    [Header("ScreenShot")]
    public Image m_ScreenShotFilter;


    private void OnEnable()
    {
        m_Client = ClientObject.Instance.m_Client;
        m_Client.m_OnRecvData = m_GameClient.OnRecvData;
        m_RoomIdx = ClientObject.Instance.m_RoomIdx;

        m_GameClient.OnNewGameStart = OnGameStart;
        m_GameClient.OnHoldCard = OnHoldCard;
        m_GameClient.OnRoomInPlayer = OnRoomInPlayer;
        m_GameClient.OnRoomOutPlayer = OnRoomOutPlayer;
        m_GameClient.OnRecvButtonUser = OnRecvButtonUser;
        m_GameClient.OnRecvBlindUser = OnRecvBlind;
        m_GameClient.OnRecvPotMoney = OnRecvPotMoney;
        m_GameClient.OnRecvBettingPlayer = OnRecvBettingUser;
        m_GameClient.OnRecvPlayerBetting = OnRecvUserBetting;
        m_GameClient.OnRecvPlayerCall = OnRecvUserBetting_Call;
        m_GameClient.OnRecvPlayerFold = OnRecvFold;
        m_GameClient.OnTableCard = OnRecvTableCard;
        m_GameClient.OnUserResultCard = OnRecvUserResultCard;
        //m_GameClient.OnWinner = OnWinner;
        //m_GameClient.OnDraw = OnDraw;
        m_GameClient.OnRank = OnRank;
        m_GameClient.OnRecvPlayInfo = OnRecvPlayInfo;
        m_GameClient.OnRecvBonusEvent = OnRecvBonus;
    }

    private void Start()
    {
        // init 
        Init();
    }

    private void Init()
    {
        m_SSPlayerMgr.Init();
        m_SSPlayMgr.Init();
    }

    public void OnClick_ScreenShot()
    {
        StartCoroutine(TakeScreenShot());
        SoundMgr.Instance.PlaySoundFx(SoundFXType.Capture);
    }

    private IEnumerator FlashWork()
    {
        if (m_ScreenShotFilter == null)
            yield break;
        m_ScreenShotFilter.color = Color.white;
        Color col = m_ScreenShotFilter.color;
        float per = 0;
        while (per < 1)
        {
            per += Time.deltaTime * 4;
            col.a = 1 - per;
            m_ScreenShotFilter.color = col;
            yield return null;
        }
    }

    private IEnumerator TakeScreenShot()
    {
        yield return FlashWork();
        yield return new WaitForEndOfFrame();

        string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
        string defaultLocation = Application.persistentDataPath + "/" + fileName;
        ScreenCapture.CaptureScreenshot(fileName);

        string folderLocation = "/storage/emulated/0/DCIM/SevenStar/";
        string screenShotLocation = folderLocation + fileName;
        if (!Directory.Exists(folderLocation))
        {
            Directory.CreateDirectory(folderLocation);
        }

        while (!File.Exists(defaultLocation))
            yield return null;
        //Debug.Log("file exist!");
        File.Move(defaultLocation, screenShotLocation);

        //REFRESHING THE ANDROID PHONE PHOTO GALLERY
        AndroidJavaClass classPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject objActivity = classPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass classUri = new AndroidJavaClass("android.net.Uri");
        AndroidJavaObject objIntent = new AndroidJavaObject("android.content.Intent", new object[2] { "android.intent.action.MEDIA_SCANNER_SCAN_FILE", classUri.CallStatic<AndroidJavaObject>("parse", "file://" + screenShotLocation) });
        objActivity.Call("sendBroadcast", objIntent);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////// Callback ///////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private void OnGameStart()
    {
        Debug.Log("OnGameStart ");
        // reset result coroutine for application pause
        m_SSPlayMgr.StopAllResultRoutine();
        m_SSPlayMgr.SkippingResult();
        m_SSPlayMgr.m_IsResult = false;
        m_SSPlayMgr.m_IsBonusEvent = false;

        // start setting
        m_IsGameStart = true;
        for (int i = 0; i < m_SSPlayerMgr.m_Players.Length; i++)
            m_SSPlayerMgr.m_Players[i].ResetPlayer();
        m_SSPlayMgr.m_PrevCallMoney = 0;
        m_SSPlayMgr.m_CommunityCard.Init();
        if (m_SSPlayMgr.m_ResultPanel)
            m_SSPlayMgr.m_ResultPanel.gameObject.SetActive(false);

        // card spread work
        for (int i = 0; i < m_SSPlayerMgr.m_Players.Length; i++)
            m_SSPlayerMgr.m_Players[i].SetHandCardMovingStartPos();
        StartCoroutine(m_SSPlayMgr.HandCardSpreadWork());

        SoundMgr.Instance.PlaySoundFx(SoundFXType.GameStart);
    }

    private void OnHoldCard(int userIdx, byte card1, byte card2)
    {
        Debug.Log("OnRecvHoldCard : " + userIdx + " / " + card1 + " / " + card2);
        m_SSPlayerMgr.SetHoldCard(userIdx, card1, card2);
    }

    private void OnRoomInPlayer(int idx)
    {
        Debug.Log("OnRecvRoomInPlayer : " + idx);
        m_SSPlayerMgr.RefreshPlayerList(false);
    }

    private void OnRoomOutPlayer(int idx)
    {
        Debug.Log("OnRecvRoomOutPlayer : " + idx);
        m_SSPlayerMgr.RemovePlayer(idx);
    }

    private void OnRecvButtonUser(int idx)
    {
        Debug.Log("OnRecvButtonUser User");
        m_SSPlayerMgr.SetDealerMarkActive(idx, true);
    }

    private void OnRecvBlind(int smallblind, int bigblind)
    {
        Debug.Log("OnRecvBlind - SB : " + smallblind + " / BB :" + bigblind);
        m_SSPlayerMgr.SetBlindBet(smallblind, bigblind);
    }

    private void OnRecvPotMoney(UInt64 potMoney, UInt64 callMoney)
    {
        Debug.Log("OnRecvPotMoney - pot:" + potMoney + " / call:" + callMoney);
        m_SSPlayMgr.SetPotMoneyWork(potMoney, callMoney);
    }

    private void OnRecvBettingUser(int idx)
    {
        Debug.Log("OnRecvBettingUser : " + idx);
        m_BettingReadyIdx = idx;
        if (m_BettingReadyIdx == m_SSPlayerMgr.m_MyPlayerIdx)
        {
            m_IsMyTurn = true;
            m_SSPlayMgr.SetBettingButtons();
            SoundMgr.Instance.PlaySoundFx(SoundFXType.MyTurn);
        }

        // set timer
        m_SSPlayerMgr.SetBettingTimer(idx);
    }

    private void OnRecvUserBetting(int idx, UInt64 money)
    {
        Debug.Log("OnRecvUserBetting : " + idx + " / money : " + money);
        m_SSPlayerMgr.UserBetting(idx, money, false);

        GamePlayerInfo player = m_SSPlayerMgr.GetExistPlayerFromIdx(idx);
        int gender = AvataMgr.Instance.GetAvataGender(player.m_UserSeat.m_AvataIdx);
        SoundMgr.Instance.PlayVoiceSound(VoiceFXType.Bet, gender);
        SoundMgr.Instance.m_IsAlert = false;
    }

    private void OnRecvUserBetting_Call(int idx, UInt64 money)
    {
        Debug.Log("OnRecvUserBetting_Call : " + idx + " / money : " + money);
        m_SSPlayerMgr.UserBetting(idx, money, false);
        SoundMgr.Instance.PlaySoundFx(SoundFXType.Check);

        GamePlayerInfo player = m_SSPlayerMgr.GetExistPlayerFromIdx(idx);
        int gender = AvataMgr.Instance.GetAvataGender(player.m_UserSeat.m_AvataIdx);
        if (money != 0)
            SoundMgr.Instance.PlayVoiceSound(VoiceFXType.Call, gender);
        else
            SoundMgr.Instance.PlayVoiceSound(VoiceFXType.Check, gender);
        SoundMgr.Instance.m_IsAlert = false;
    }

    private void OnRecvFold(int UserIdx)
    {
        Debug.Log("OnRecvFold user : " + UserIdx);
        for (int i = 0; i < m_SSPlayerMgr.m_Players.Length; i++)
            m_SSPlayerMgr.m_Players[i].SetFold(UserIdx, true);
        // set button panel
        if (UserIdx == m_SSPlayerMgr.m_PlayerSelf.m_PlayerIdx)
        {
            m_IsMyTurn = false;
            m_SSPlayMgr.SetBettingButtons();
        }

        SoundMgr.Instance.PlaySoundFx(SoundFXType.Fold);
        SoundMgr.Instance.m_IsAlert = false;
    }

    private void OnRecvTableCard(int type, byte[] card)
    {
        m_TurnCount = type;
        m_SSPlayMgr.CommunityCardWork(type, card);
    }

    private void OnRecvUserResultCard(int UserIdx, byte[] card)
    {
        Debug.Log("OnRecvResultCard user : " + UserIdx);
        m_SSPlayerMgr.SetResultCard(UserIdx, card);
    }

    private void OnWinner(int UserIdx, UInt64 Money)
    {
        Debug.Log("OnWinner user : " + UserIdx);
        for (int i = 0; i < m_SSPlayerMgr.m_Players.Length; i++)
            m_SSPlayerMgr.m_Players[i].SetWinner(UserIdx, Money);
        m_SSPlayMgr.UnActiveBettingPanel();
        m_SSPlayMgr.StartResultWork(UserIdx, false);

        m_SSPlayMgr.m_IsResult = true;
    }

    private void OnDraw(int UserIdx, UInt64 Money)
    {
        Debug.Log("OnDraw user : " + UserIdx);
        for (int i = 0; i < m_SSPlayerMgr.m_Players.Length; i++)
            m_SSPlayerMgr.m_Players[i].SetWinner(UserIdx, Money);
        m_SSPlayMgr.UnActiveBettingPanel();
        m_SSPlayMgr.StartResultWork(UserIdx, true);

        m_SSPlayMgr.m_IsResult = true;
    }

    private void OnRank(List<Game_Client_RankData> rankDataList)
    {
        Debug.Log("OnRank");

        bool isDraw = false;
        int winnerCount = 0;
        int winnerIdx = -1;
        for (int i = 0; i < rankDataList.Count; i++)
        {
            if(rankDataList[i].Rank == 0)
            {
                winnerIdx = rankDataList[i].UserIndex;
                for (int j = 0; j < m_SSPlayerMgr.m_Players.Length; j++)
                    m_SSPlayerMgr.m_Players[j].SetWinner(winnerIdx, rankDataList[i].DividendsMoney);
                winnerCount++;
            }
            else
            {
                for (int j = 0; j < m_SSPlayerMgr.m_Players.Length; j++)
                    m_SSPlayerMgr.m_Players[j].SidePotReceiver(rankDataList[i].UserIndex, rankDataList[i].DividendsMoney);
            }
        }

        if (winnerCount > 1)
            isDraw = true;
        m_SSPlayMgr.UnActiveBettingPanel();
        m_SSPlayMgr.StartResultWork(winnerIdx, isDraw);
        m_SSPlayMgr.m_IsResult = true;
    }

    private void OnRecvPlayInfo(int UserIdx, UInt64 TotBettingMoney, UInt64 NowBettingMoney, byte c1, byte c2)
    {
        Debug.Log("OnRecvPlayInfo user : " + UserIdx+" / tot : "+TotBettingMoney+" / now :"+NowBettingMoney);
        for (int i = 0; i < m_SSPlayerMgr.m_Players.Length; i++)
        {
            if(m_SSPlayerMgr.m_Players[i].m_PlayerIdx == UserIdx)
            {
                m_SSPlayerMgr.m_Players[i].m_IsInitPlaying = true;
                //set card
                m_SSPlayerMgr.m_Players[i].m_UserSeat.SetHandCardActive(true);
                m_SSPlayerMgr.m_Players[i].SetHandCard1(c1);
                m_SSPlayerMgr.m_Players[i].SetHandCard2(c2);
                //set bettingmoney
                m_SSPlayerMgr.m_Players[i].m_UserSeat.SetBettingMoney(NowBettingMoney);
                m_SSPlayerMgr.m_Players[i].m_UserSeat.m_BettingTimer.gameObject.SetActive(true);
                m_SSPlayerMgr.m_Players[i].SetBettingTimer(0);
                if (NowBettingMoney != 0)
                    StartCoroutine(m_SSPlayerMgr.m_Players[i].m_UserSeat.m_BettingMoneyEff.BettingEff());
            }
        }

        UInt64 potMoney = UInt64.Parse(m_SSPlayMgr.m_PotMoneyText.text);
        potMoney += TotBettingMoney;
        m_SSPlayMgr.m_PotMoneyText.text = potMoney.ToString();
    }

    private void OnRecvBonus(int type, string eventName, UInt64 money)
    {
        Debug.Log("OnRecvBonus type : " + type + "/ event name : "+ eventName+" / money : "+money);
        m_SSPlayMgr.m_IsBonusEvent = true;
        if (type == 0)
        {
            m_SSPlayMgr.m_BonusEventType = 0;
            m_SSPlayMgr.m_PlaneAni.SetEventNameText(eventName);
            m_SSPlayMgr.m_PlaneAni.SetMoneyText(TransformMoney.MoneyTransform_WholeNumber(money));
        }
        else if(type == 1)
        {
            //m_CKPlayMgr.m_BonusEventType = 1;
            m_SSPlayMgr.m_BoxAni.PlayBoxAni();
            m_SSPlayMgr.m_BoxAni.SetEventNameText(eventName);
            m_SSPlayMgr.m_BoxAni.SetMoneyText(TransformMoney.MoneyTransform_WholeNumber(money));
            m_SSPlayMgr.m_IsBonusEvent = false;
        }
    }
}
