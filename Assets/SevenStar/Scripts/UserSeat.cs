using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UserSeat : MonoBehaviour
{
    public GamePlayerInfo m_Player;

    // ui component
    [Header("UI Component")]
    public Text m_UserNameText;
    public Text m_UserMoneyText;
    public Text m_BetMoneyText;
    public Image m_Avata;
    public BettingTimer m_BettingTimer;
    public Image m_CallBetball;
    public Sprite m_CallSprite;
    public Sprite m_BetSprite;
    public CardObject m_HandCardObj1;
    public CardObject m_HandCardObj2;
    public UserBlindIcon m_UserBlindIcon;
    public Image m_ResultMark;
    public Sprite m_WinSprite;
    public Sprite m_LoseSprite;
    public BettingMoneyEff m_BettingMoneyEff;
    public Image m_AllintMark;
    public GameObject m_TurnImage;

    public int m_SeatNum;
    public int m_AvataIdx;
    // for my seat
    [Header("For My Seat")]
    public Text m_RankOfCardText;
    public GameObject m_ExitWaitingBar;
    public bool m_IsMySeat;
    
    public void InitData()
    {
        if (m_UserNameText)
            m_UserNameText.text = "";
        if (m_UserMoneyText)
            m_UserMoneyText.text = "";
        if (m_BetMoneyText)
            m_BetMoneyText.text = "";
        if (m_RankOfCardText)
            m_RankOfCardText.text = "-";
        if (m_UserBlindIcon)
            m_UserBlindIcon.IconInit();
        if (m_Avata)
            m_Avata.gameObject.SetActive(false);
        if (m_CallBetball)
            m_CallBetball.gameObject.SetActive(false);
        if (m_HandCardObj1)
            m_HandCardObj1.gameObject.SetActive(false);
        if (m_HandCardObj2)
            m_HandCardObj2.gameObject.SetActive(false);
    }

    public void ReadyBetting() // bet component disable
    {
        if (m_BetMoneyText)
            m_BetMoneyText.text = "";
        if (m_CallBetball)
            m_CallBetball.gameObject.SetActive(false);
    }

    public void SetUserName(string name)
    {
        if (m_UserNameText)
            m_UserNameText.text = name;
    }

    public void SetMoney(UInt64 money)
    {
        if (m_UserMoneyText)
        {
            if (money > 0)
            {
                //m_UserMoneyText.text = "" + money.ToString();
                m_UserMoneyText.text = TransformMoney.MoneyTransform(money,false);
                SetAllinMark(false);
            }
            else if (money == 0)
            {
                SetAllinMark(true);
            }
        }
        if (m_IsMySeat)
            SevenStarLogic.Instance.m_SSPlayMgr.m_MyExtraMoney = money;
    }

    public void SetBettingMoney(UInt64 betMoney)
    {
        if (betMoney == 0)
        {
            if (m_BetMoneyText)
                m_BetMoneyText.text = "";
            return;
        }
        if (m_BetMoneyText)
            //m_BetMoneyText.text = "" + betMoney.ToString();
            m_BetMoneyText.text = TransformMoney.MoneyTransform(betMoney,true);
    }

    public void SetAvata(int idx, bool isFold)
    {
        if (m_Avata==null)
            return;
        
        if (idx >= 0)
        {
            if (m_Avata.gameObject.activeSelf == false)
                m_Avata.gameObject.SetActive(true);
            if(isFold)
            {
                m_Avata.sprite = AvataMgr.Instance.GetAvataBlackSprite(idx);
            }
            else
            {
                m_Avata.sprite = AvataMgr.Instance.GetAvataSprite(idx);
                //m_BettingTimer.SetTimerImage(m_Avata.sprite);

                if (m_Player.m_IsInitPlaying == false)
                {
                    m_Avata.sprite = AvataMgr.Instance.GetAvataBlackSprite(idx);
                }
            }
        }
        else
        {
            m_Avata.gameObject.SetActive(false);
            m_BettingTimer.gameObject.SetActive(false);
        }
        
    }

    public void SetHandCardActive(bool isActive)
    {
        if (m_HandCardObj1)
            m_HandCardObj1.gameObject.SetActive(isActive);
        if (m_HandCardObj2)
            m_HandCardObj2.gameObject.SetActive(isActive);
    }

    public void SetActiveDLMark(bool isActive)
    {
        m_UserBlindIcon.SetActiveBlindIcon(0,isActive);
    }

    public void UnactiveBlindIcon()
    {
        m_UserBlindIcon.UnactiveBlindIcon();
        m_CallBetball.gameObject.SetActive(false);
    }

    public void ActiveBlindIcon()
    {
        m_UserBlindIcon.ActiveBlindIcon();
        m_CallBetball.gameObject.SetActive(true);
    }

    public void SetBlindIcon(int type)
    {
        m_UserBlindIcon.SetActiveBlindIcon(type,true);
    }

    public void SetCallBetBall_Image(bool isCall)
    {
        if (m_CallBetball.gameObject.activeSelf == false)
            m_CallBetball.gameObject.SetActive(true);

        if (m_CallBetball && isCall)
            m_CallBetball.sprite = m_CallSprite;
        else
            m_CallBetball.sprite = m_BetSprite;
    }

    public void SetResultMark(bool isWin)
    {
        if (m_ResultMark == null)
            return;
        if (m_ResultMark.gameObject.activeSelf == false)
            m_ResultMark.gameObject.SetActive(true);
        if (isWin)
            m_ResultMark.sprite = m_WinSprite;
        else
            m_ResultMark.gameObject.SetActive(false);
    }

    public void SetHandCoverCardActive(bool isActive)
    {
        if(m_HandCardObj1)
            m_HandCardObj1.SetCoverCard(isActive);
        if (m_HandCardObj2)
            m_HandCardObj2.SetCoverCard(isActive);
    }

    public void SetAllinMark(bool isAllin)
    {
        if (m_AllintMark)
            m_AllintMark.gameObject.SetActive(isAllin);
    }

    // for my seat
    public void SetRankOfCard(string text)
    {
        if (m_RankOfCardText && (SevenStarLogic.Instance.m_IsGameStart == true))
            m_RankOfCardText.text = text;
    }

    public void SetExitWatingBar(bool isActive)
    {
        if (m_ExitWaitingBar == null)
            return;
        m_ExitWaitingBar.SetActive(isActive);
    }
}
