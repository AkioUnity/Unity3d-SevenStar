using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public enum BettingType
{
    Fold=0,
    Check,
    Call,
    Raise,
    Allin
}


public enum BlindValueType
{
    All = 0,
    SmallBlind_20,
    SmallBlind_50,
    SmallBlind_100,
    SmallBlind_200
}

public class SevenStarPlayMgr : MonoBehaviour
{
    [Header("Betting Status")]
    public Slider m_BettingSlider;
    public Text m_MyBetMoneyText;
    public UInt64 m_PrevCallMoney = 0;
    public UInt64 m_MyExtraMoney = 0;
    public UInt64 m_MyBettingMoney = 0;
    public Text[] m_BettingClampText;
    public int m_BettingClampIdx = 0;

    [Header("Bet Blind Info")]
    public BlindValueType m_BetBlindType;
    public UInt64 m_SBMoney;
    public UInt64 m_BBMoney;

    [Header("UI Components")]
    public GameObject m_BettingPanel;
    public GameObject m_MenuPanel;
    public GameObject m_MenuBack;
    public Text m_PotMoneyText;
    public BettingButtons m_BettingBtns;
    public CommunityCard m_CommunityCard;
    public ResultPanel m_ResultPanel;
    public GameObject m_ResultMoneyObj;
    public List<GameObject> m_ResultMoneyObjList = new List<GameObject>();
    public PlaneAni m_PlaneAni;
    public BoxAni m_BoxAni;

    public bool m_IsExitWaiting = false;
    public bool m_IsResult = false;
    public bool m_IsBonusEvent = false;
    public int m_BonusEventType = 0;
    private bool m_CommunityCardWorking = false;
    private SevenStarLogic m_Logic;
    private bool m_IsResultWorking = false;
    private List<IEnumerator> m_ResultRoutineList = new List<IEnumerator>();
    private int m_ForAdminCount = 0;

    private void Awake()
    {
        m_Logic = SevenStarLogic.Instance;
        if (m_Logic.m_SSPlayMgr == null)
            m_Logic.m_SSPlayMgr = this;
    }

    public void Init()
    {
        SetBlindMoney();
        m_CommunityCard.Init();
        m_MyBettingMoney = 0;

    }

    private void SetBlindMoney()
    {
        m_BetBlindType = (BlindValueType)ClientObject.Instance.m_BlindType;
        switch (m_BetBlindType)
        {
            case BlindValueType.All:
                break;
            case BlindValueType.SmallBlind_20:
                m_SBMoney = 20;
                m_BBMoney = 40;
                break;
            case BlindValueType.SmallBlind_50:
                m_SBMoney = 50;
                m_BBMoney = 100;
                break;
            case BlindValueType.SmallBlind_100:
                m_SBMoney = 100;
                m_BBMoney = 200;
                break;
            case BlindValueType.SmallBlind_200:
                m_SBMoney = 200;
                m_BBMoney = 400;
                break;
        }
    }

    public void SetExitWaiting()
    {
        if(SevenStarLogic.Instance.m_IsGameStart)
        {
            if (m_IsExitWaiting)
                m_IsExitWaiting = false;
            else
                m_IsExitWaiting = true;

            // show exit wating panel
            SevenStarLogic.Instance.m_SSPlayerMgr.m_PlayerSelf.m_UserSeat.SetExitWatingBar(m_IsExitWaiting);
        }
        else
        {
            ExitToLobby();
        }
    }

    private void ExitToLobby()
    {
        SevenStarLogic.Instance.Client.SendOutRoom();
        SceneManager.LoadScene("2_Lobby");
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////// OnClick ///////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////

    public void OnClick_Menu()
    {
        bool active = m_MenuPanel.activeSelf == true ? false : true;
        m_MenuPanel.SetActive(active);
    }

    public void OnClick_RoomOut()
    {
        SetExitWaiting();
    }

    public void OnClick_Fold()
    {
        BettingAction(BettingType.Fold, 0);
    }

    public void OnClick_Check()
    {
        BettingAction(BettingType.Check, 0);
    }

    public void OnClick_Call()
    {
        BettingAction(BettingType.Call, 0);
    }

    public void OnClick_Betting()
    {
        bool active = m_BettingPanel.activeSelf == true ? false : true;
        m_BettingPanel.SetActive(active);
        m_BettingBtns.m_ConfirmBtn.gameObject.SetActive(true);

        m_BettingSlider.value = GetBettingSlidePer(m_PrevCallMoney * 2);
        m_BettingClampIdx = 0;
    }

    public void OnClick_BettingConfirm()
    {
        // if raise or all in
        //BettingAction(BettingType.Allin, m_MyBettingMoney);
        UInt64 betMoney = m_MyBettingMoney;
        if (m_BettingSlider.value >= 1)
            betMoney = m_MyExtraMoney;
        //Debug.Log(m_BettingSlider.value + "/" + betMoney);
        BettingAction(BettingType.Raise, betMoney);
    }

    public void OnClick_BettingSlidePlus()
    {
        if (m_BettingClampIdx < 5)
        {
            m_BettingClampIdx++;

            UInt64 unitMoney = m_MyExtraMoney / 5;
            m_MyBettingMoney = unitMoney * (UInt64)m_BettingClampIdx;

            if (m_BettingClampIdx == 5)
            {
                m_MyBettingMoney = m_MyExtraMoney;
                m_BettingSlider.value = 1;
                return;
            }
            m_BettingSlider.value = GetBettingSlidePer(m_MyBettingMoney);
        }
    }

    public void OnClick_BettingSlideMinus()
    {
        if (m_BettingClampIdx > 0)
        {
            m_BettingClampIdx--;

            UInt64 unitMoney = m_MyExtraMoney / 5;
            m_MyBettingMoney = unitMoney * (UInt64)m_BettingClampIdx;

            if (m_MyBettingMoney < m_PrevCallMoney*2)
                m_MyBettingMoney = m_PrevCallMoney*2;
            m_BettingSlider.value = GetBettingSlidePer(m_MyBettingMoney);
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////// betting action ////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////

    private void BettingAction(BettingType type, UInt64 myBettingMoney)
    {
        switch (type)
        {
            case BettingType.Fold:
                Fold();
                break;
            case BettingType.Check:
                Check();
                break;
            case BettingType.Call:
                Call();
                break;
            case BettingType.Raise:
                UInt64 wholeBetMoney = m_Logic.m_SSPlayerMgr.m_PlayerSelf.m_BettedMoney + myBettingMoney;
                Betting(wholeBetMoney);
                break;
            case BettingType.Allin:
                break;
        }

        m_Logic.m_IsMyTurn = false;
        SetBettingButtons();
        UnActiveBettingPanel();
    }

    private void Fold()
    {
        if (m_Logic.m_BettingReadyIdx != m_Logic.m_SSPlayerMgr.m_MyPlayerIdx)
            return;
        m_Logic.Client.SendFold();
    }

    private void Check()
    {
        // need call process
        Call();
    }

    private void Call()
    {
        if (m_Logic.m_BettingReadyIdx != m_Logic.m_SSPlayerMgr.m_MyPlayerIdx)
            return;
        m_Logic.Client.SendCall();
    }

    private void Betting(UInt64 money)
    {
        if (m_Logic.m_BettingReadyIdx != m_Logic.m_SSPlayerMgr.m_MyPlayerIdx)
            return;
        m_Logic.Client.SendBetting(money);
    }



    /////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////// UI Control  ///////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////

    private void Update()
    {
        // betting slide work
        BettingSlideWork();
    }

    public void OnBettingSliderValueChange()
    {
        m_MyBettingMoney = GetMoneyFromSlide();
        //m_MyBetMoneyText.text = "" + m_MyBettingMoney.ToString();
        m_MyBetMoneyText.text = TransformMoney.MoneyTransform(m_MyBettingMoney,true);
        SoundMgr.Instance.PlaySoundFx(SoundFXType.BettingChip,0.5f);
    }

    private void BettingSlideWork()
    {
        if (m_BettingSlider == null)
            return;
 		//min
        if (m_BettingSlider.value < GetBettingSlidePer(m_PrevCallMoney * 2))
            m_BettingSlider.value = GetBettingSlidePer(m_PrevCallMoney * 2);
        //max
        //m_BettingSlider.maxValue = m_MyExtraMoney;

        SetBettingClampText();
    }

    private float GetBettingSlidePer(UInt64 money)
    {
        float per = 0;
        if(money != 0)
            per = (float)money / (float)m_MyExtraMoney;
        return per;
    }

    private UInt64 GetMoneyFromSlide()
    {
        return (UInt64)Mathf.Round(m_BettingSlider.value * m_MyExtraMoney);
    }


    private void SetBettingClampText()
    {
        m_BettingClampText[0].text = TransformMoney.MoneyTransform(m_MyExtraMoney / 5 * 1, true);
        m_BettingClampText[1].text = TransformMoney.MoneyTransform(m_MyExtraMoney / 5 * 2, true);
        m_BettingClampText[2].text = TransformMoney.MoneyTransform(m_MyExtraMoney / 5 * 3, true);
        m_BettingClampText[3].text = TransformMoney.MoneyTransform(m_MyExtraMoney / 5 * 4, true);
    }



    public void SetBettingButtons()
    {
        if (m_BettingBtns && !m_CommunityCardWorking)
        {
            UInt64 bettedMoney = SevenStarLogic.Instance.m_SSPlayerMgr.m_PlayerSelf.m_BettedMoney;
            if (SevenStarLogic.Instance.m_TurnCount == 0 && m_PrevCallMoney == 0)
                m_PrevCallMoney = m_BBMoney;
            m_BettingBtns.CheckBtnInteractable(m_PrevCallMoney, bettedMoney);
            m_BettingBtns.SetBtnInteractable(m_Logic.m_IsMyTurn);
            m_BettingBtns.m_CallBtn_CallMoney.text = TransformMoney.MoneyTransform(m_PrevCallMoney, true);
            if (m_Logic.m_IsMyTurn == false)
                UnActiveBettingPanel();
        }
    }

    public void UnActiveBettingPanel()
    {
        m_Logic.m_IsMyTurn = false;
        m_BettingPanel.SetActive(false);
        m_BettingBtns.SetBtnInteractable(false);
        m_BettingBtns.m_ConfirmBtn.gameObject.SetActive(false);
        m_BettingBtns.m_CallBtn_CallMoney.text = "";
    }

    private void BettingUIReady()
    {
        GamePlayerInfo[] players = m_Logic.m_SSPlayerMgr.m_Players;
        for (int i = 0; i < players.Length; i++)
        {
            players[i].m_BettedMoney = 0;
            players[i].m_UserSeat.UnactiveBlindIcon();
            players[i].m_UserSeat.ReadyBetting();
        }
    }

    private void SetPotMoney(UInt64 money)
    {
        if (m_PotMoneyText)
            //m_PotMoneyText.text = "" + money.ToString();
            m_PotMoneyText.text = TransformMoney.MoneyTransform(money,false);
    }
    
    public void SetPotMoneyWork(UInt64 money, UInt64 callMoney)
    {
        SetPotMoney(money);
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////// Hand Card Spread Work  ////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public IEnumerator HandCardSpreadWork()
    {
        GamePlayerInfo[] players = m_Logic.m_SSPlayerMgr.m_Players;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].m_PlayerIdx == -1)
                continue;
            StartCoroutine(CardMoveRoutine(players[i].m_UserSeat.m_HandCardObj1));
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(CardMoveRoutine(players[i].m_UserSeat.m_HandCardObj2));
            yield return new WaitForSeconds(0.35f);
        }
        yield return null;
    }

    public IEnumerator CardMoveRoutine(CardObject card)
    {
        if (card == null)
            yield break;
        SoundMgr.Instance.PlaySoundFx(SoundFXType.DealCard);

        Vector3 nowPos = card.gameObject.transform.position;
        float per = 0;
        while (per < 1)
        {
            per += Time.deltaTime * 3;
            Vector3 pos = Vector3.Lerp(nowPos, card.m_InitPos, per);
            card.gameObject.transform.position = pos;
            yield return null;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////// Community Card Work  //////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void CommunityCardWork(int type, byte[] card)
    {
        StartCoroutine(CommunityCardWorkRoutine(type, card));
    }

    private IEnumerator CommunityCardWorkRoutine(int type, byte[] card)
    {
        m_CommunityCardWorking = true;

        GamePlayerInfo[] players = m_Logic.m_SSPlayerMgr.m_Players;
        for (int i = 0; i < players.Length; i++)//bet end effect
        {
            if(players[i].m_PlayerIdx != -1)
                StartCoroutine(players[i].m_UserSeat.m_BettingMoneyEff.BettingEndEff());
        }
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ChargeChip);
        yield return new WaitForSeconds(1);

        switch (type)
        {
            case 0://all, free flob
                Debug.Log("OnRecvTableCard all 0"); // init
                // all card for admin
                if (m_IsResult == false)
                {
                    if(SevenStarLogic.m_ApplicationPauseTrigger == true)
                    {
                        if (m_ForAdminCount == 1)
                        {
                            SevenStarLogic.m_ApplicationPauseTrigger = false;
                            m_ForAdminCount = 0;
                            yield return CommunityCardWork_All_ForAdmin(card);
                        }
                        else
                            m_ForAdminCount++;
                    }
                    else
                    {
                        yield return CommunityCardWork_All_ForAdmin(card);
                        m_ForAdminCount = 0;
                    }
                }
                break;
            case 1://flob
                BettingUIReady();
                m_PrevCallMoney = m_SBMoney;
                yield return CommunityCardWork_Flop(card);
                Debug.Log("OnRecvTableCard  flob 1");
                break;
            case 2://turn
                BettingUIReady();
                m_PrevCallMoney = m_SBMoney;
                yield return CommunityCardWork_Turn(card[0]);
                Debug.Log("OnRecvTableCard  turn 2 ");
                break;
            case 3://river
                BettingUIReady();
                m_PrevCallMoney = m_SBMoney;
                yield return CommunityCardWork_River(card[0]);
                Debug.Log("OnRecvTableCard  river 3");
                break;
        }

        m_CommunityCardWorking = false;
        SetBettingButtons();
    }

    private IEnumerator CommunityCardWork_All_ForAdmin(byte[] card) // turn 1 for admin
    {
        yield return new WaitForEndOfFrame();
        m_CommunityCard.SetActiveComAdminCard(true);
        // my player
        GamePlayerInfo player = m_Logic.m_SSPlayerMgr.m_PlayerSelf;
        CardShapeType type = CardShapeType.Back;
        int num = -1;
        for (int i = 0; i < 5; i++)
        {
            //m_CommunityCard.SetComCardToInitPos(i);
            type = (CardShapeType)GetCardType(card[i]);
            num = GetCardNum(card[i]);
            yield return null;
            m_CommunityCard.SetComAdminCardInfo(i, type, num);
            m_CommunityCard.m_ComAdminCards[i].SetCardSprite();
            m_CommunityCard.m_OpenCount++;
            // player card progress setting
            player.SetPlayerCardProg(card[i]);
        }

        // player hand card cover unactive
        for (int i = 0; i < m_Logic.m_SSPlayerMgr.m_Players.Length; i++)
        {
            m_Logic.m_SSPlayerMgr.m_Players[i].m_UserSeat.SetHandCoverCardActive(false);
        }


        // player card rank text
        player.SetCardRankText();
    }

    private IEnumerator CommunityCardWork_Flop(byte[] card) // turn 2
    {
        yield return new WaitForEndOfFrame();
        // my player
        GamePlayerInfo player = m_Logic.m_SSPlayerMgr.m_PlayerSelf;
        CardShapeType type = CardShapeType.Back;
        int num = -1;
        for (int i = 0; i < 3; i++)
        {
            //yield return m_CommunityCard.CardMoveRoutine(i);
            StartCoroutine(m_CommunityCard.CardMoveRoutine(i));
            type = (CardShapeType)GetCardType(card[i]);
            num = GetCardNum(card[i]);
            yield return new WaitForSeconds(0.2f);
            m_CommunityCard.SetComCardInfo(i, type, num);
            m_CommunityCard.OpenComCard(i);
            // player card progress setting
            player.SetPlayerCardProg(card[i]);
        }

        // player card rank text
        player.SetCardRankText();
    }

    private IEnumerator CommunityCardWork_Turn(byte card) // turn 3
    {
        yield return new WaitForEndOfFrame();
        CardShapeType type = (CardShapeType)GetCardType(card);
        int num = GetCardNum(card);
        yield return new WaitForSeconds(0.3f);
        yield return m_CommunityCard.CardMoveRoutine(3);
        yield return new WaitForSeconds(0.2f);
        m_CommunityCard.SetComCardInfo(3, type, num);
        m_CommunityCard.OpenComCard(3);

        // my player
        // player card rank text
        GamePlayerInfo player = m_Logic.m_SSPlayerMgr.m_PlayerSelf;
        player.SetPlayerCardProg(card);
        player.SetCardRankText();
    }

    private IEnumerator CommunityCardWork_River(byte card) // turn 4 - last turn
    {
        yield return new WaitForEndOfFrame();
        CardShapeType type = (CardShapeType)GetCardType(card);
        int num = GetCardNum(card);
        yield return new WaitForSeconds(0.3f);
        yield return m_CommunityCard.CardMoveRoutine(4);
        yield return new WaitForSeconds(0.2f);
        m_CommunityCard.SetComCardInfo(4, type, num);
        m_CommunityCard.OpenComCard(4);

        // my player
        // player card rank text
        GamePlayerInfo player = m_Logic.m_SSPlayerMgr.m_PlayerSelf;
        player.SetPlayerCardProg(card);
        player.SetCardRankText();
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////// Result Work  ///////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void StopAllResultRoutine()
    {
        for (int i = 0; i < m_ResultRoutineList.Count; i++)
        {
            StopCoroutine(m_ResultRoutineList[i]);
        }
    }


    public void StartResultMoneyRoutine(Vector3 targetPos)
    {
        IEnumerator routine = ResultMoneyRoutine(targetPos);
        m_ResultRoutineList.Add(routine);
        StartCoroutine(routine);
    }

    public IEnumerator ResultMoneyRoutine(Vector3 targetPos)
    {
        yield return new WaitForSeconds(2);
        GameObject resultObj = Instantiate(m_ResultMoneyObj) as GameObject;
        m_ResultMoneyObjList.Add(resultObj);
        resultObj.transform.SetParent(m_ResultMoneyObj.transform.parent);
        resultObj.gameObject.SetActive(true);
        resultObj.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        resultObj.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        resultObj.transform.localScale = Vector3.one;
        resultObj.transform.position = m_ResultMoneyObj.transform.position;

        SoundMgr.Instance.PlaySoundFx(SoundFXType.WinChip);

        Vector3 startPos = resultObj.transform.position;
        float per = 0;
        while (per<1)
        {
            per += Time.deltaTime * 4;
            resultObj.transform.position = Vector3.Lerp(startPos, targetPos, per);
            yield return null;
        }

        CanvasGroup cgResultMoney = resultObj.GetComponent<CanvasGroup>();
        per = 0;
        while (per < 1)
        {
            per += Time.deltaTime * 4;
            cgResultMoney.alpha = 1 - per;
            yield return null;
        }

        Destroy(resultObj);
    }

    public void SkippingResult()
    {
        int myIdx = SevenStarLogic.Instance.m_SSPlayerMgr.m_MyPlayerIdx;
        UserInfo info = UserMgr.Instance.GetUserInfo(myIdx);
        if (info.UserMoney <= 0 || info.UserMoney < m_BBMoney)
            ExitToLobby();
        // check exit waiting
        if (m_IsExitWaiting)
            ExitToLobby();

        // user refesh user money
        SevenStarLogic.Instance.m_SSPlayerMgr.RefreshPlayerList(false);
        m_CommunityCard.Init();
        m_ResultPanel.gameObject.SetActive(false);
        GamePlayerInfo[] players = SevenStarLogic.Instance.m_SSPlayerMgr.m_Players;
        for (int i = 0; i < players.Length; i++)
        {
            players[i].ResetPlayer();
            players[i].m_UserSeat.SetHandCardActive(false);
            players[i].m_UserSeat.m_BettingTimer.gameObject.SetActive(false);
        }
        m_IsResultWorking = false;

        // reset result money obj
        if(m_ResultMoneyObjList.Count > 0)
        {
            for (int i = 0; i < m_ResultMoneyObjList.Count; i++)
            {
                Destroy(m_ResultMoneyObjList[i]);
            }
            m_ResultMoneyObjList.Clear();
        }
        SevenStarLogic.Instance.m_IsGameStart = false;
    }

    public void StartResultWork(int userIdx, bool isDraw)
    {
        if (m_IsResultWorking == false)
        {
            IEnumerator routine = ResultWork(userIdx, isDraw);
            m_ResultRoutineList.Add(routine);
            StartCoroutine(routine);
        }
        
    }

    public IEnumerator ResultWork(int userIdx, bool isDraw)
    {
        m_IsResultWorking = true;

        GamePlayerInfo[] players = SevenStarLogic.Instance.m_SSPlayerMgr.m_Players;
        for (int i = 0; i < players.Length; i++)
        {
            // set result mark
            if (players[i].m_PlayerIdx == -1)
            {
                players[i].m_UserSeat.m_ResultMark.gameObject.SetActive(false);
                continue;
            }
            if (players[i].m_IsWinner == false)
                players[i].m_UserSeat.SetResultMark(false);
        }

        if (SevenStarLogic.Instance.m_SSPlayerMgr.m_MyPlayerIdx == userIdx)
            SoundMgr.Instance.PlaySoundFx(SoundFXType.WinnerMyself);
        else
            SoundMgr.Instance.PlaySoundFx(SoundFXType.GameEnd);

        // result eff delay
        yield return new WaitForSeconds(2);
        m_PotMoneyText.text = ""+0;

        // user refesh user money
        SevenStarLogic.Instance.m_SSPlayerMgr.RefreshPlayerList(false);
        yield return new WaitForSeconds(0.5f);

        // set reuslt panel
        if (m_ResultPanel && (m_CommunityCard.m_OpenCount >= 5) && isDraw == false)
        {
            UserInfo info = UserMgr.Instance.GetUserInfo(userIdx);
            GamePlayerInfo winner = SevenStarLogic.Instance.m_SSPlayerMgr.GetExistPlayerFromIdx(userIdx);
            string winnerName = info.UserName;
            byte[] cards = winner.m_ResultCard;
            string rankOfCard = winner.m_CardData.GetScoreString(winner.m_ResultCard, ref cards);
            // set info
            m_ResultPanel.SetResultInfo(winnerName, rankOfCard);
            // set card
            for (int i = 0; i < cards.Length; i++)
            {
                CardShapeType type = (CardShapeType)GetCardType(cards[i]);
                int cardIdx = GetCardNum(cards[i]);
                m_ResultPanel.SetResultPanelCard(i, type, cardIdx);
            }
            m_ResultPanel.gameObject.SetActive(true);
            
        }
        // unactive betting panel
        UnActiveBettingPanel();
        m_IsResultWorking = false;
        
        //if (ChingizKhanLogic.Instance.m_PlayerCount <= 2)
            yield return ResultAfterWork();
    }
    
    private IEnumerator ResultAfterWork()
    {
        yield return new WaitForSeconds(1);
        // check my self money - all in
        int myIdx = SevenStarLogic.Instance.m_SSPlayerMgr.m_MyPlayerIdx;
        UserInfo info = UserMgr.Instance.GetUserInfo(myIdx);
        if (info.UserMoney <= 0 || info.UserMoney < m_BBMoney)
            ExitToLobby();
        // check exit waiting
        if (m_IsExitWaiting)
            ExitToLobby();

        if (m_IsBonusEvent)
        {
            if (m_BonusEventType == 0)
            {
                m_PlaneAni.PlayPlaneAni();
                yield return new WaitForSeconds(7);// plane ani time -7sec
                m_PlaneAni.InitText();
            }
        }

        yield return new WaitForSeconds(3);
        m_CommunityCard.Init();
        m_ResultPanel.gameObject.SetActive(false);
        GamePlayerInfo[] players = SevenStarLogic.Instance.m_SSPlayerMgr.m_Players;
        for (int i = 0; i < players.Length; i++)
        {
            players[i].ResetPlayer();
            players[i].m_UserSeat.SetHandCardActive(false);
            players[i].m_UserSeat.m_BettingTimer.gameObject.SetActive(false);
        }

        SevenStarLogic.Instance.m_IsGameStart = false;
    }



    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////// ETC Function ///////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public int GetCardType(byte c)
    {
        if (c == 0)
            return 0;
        int f = c & 0xF0;
        f = f >> 4;
        if (f < 1 || f > 4)
            return 0;
        return f;
    }

    public int GetCardNum(byte c)
    {
        if (c == 0)
            return 0;
        int b = c & 0x0F;
        if (b < 2 || b > 14)
            return 0;
        return b;
    }
}
