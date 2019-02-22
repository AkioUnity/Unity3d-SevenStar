using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WithdrawalProc : MonoBehaviour
{
    public UserInfoSet m_UserInfoSet;

    [Header("Withdrawal Process Panel")]
    public GameObject[] m_WithdrawalProcPanel;
    public Button m_NextBtn;
    public Button m_BackBtn;
    public Button m_SubmitWithdrawalBtn;
    public GameObject m_AlertWithdra_NotEnoughMoney;
    public GameObject m_AlertWithdraw_Insufficient;
    public InputFieldCheckMark m_WithdrawCheckMark;
    public Text m_BankMoney;
    public InputField m_WithdrawMoney;
    public Text m_SelectWithdrawMoney;
    public InputField m_Bank;
    public InputField m_BankAccountNumber;
    public InputField m_BankAccountName;

    [Header("Dot")]
    public Sprite m_BlackDot;
    public Sprite m_WhiteDot;
    public Image[] m_DotImage;

    private int m_NowProcIdx = 0;

    public void Init()
    {
        m_NowProcIdx = 0;
        SetWithdrawalProcPanel(0);

        TexasHoldemClient c = TexasHoldemClient.Instance;
        c.SendGetUserInfo(LobbyLogic.Instance.m_UserInfo.m_UserIndex);
        c.SendGetBankMoney();

        // get user bank data
        GetUserBankData();
    }

    private void GetUserBankData()
    {
        m_Bank.text = LoginSession.Instance.m_BankName;
        m_BankAccountName.text = LoginSession.Instance.m_AccountName;
        m_BankAccountNumber.text = LoginSession.Instance.m_AccountNumber.ToString();
    }

    private void SetUserBankData()
    {
        string id = LoginSession.Instance.m_LoginID;
        PlayerPrefs.SetString(id + "_BankName",m_Bank.text);
        PlayerPrefs.SetString(id + "_AccountName",m_BankAccountName.text);
        PlayerPrefs.SetString(id + "_AccountNumber",m_BankAccountNumber.text);

        LoginSession.Instance.GetBankData();
    }

    private void Update()
    {
        if (m_BankMoney != null)
            m_BankMoney.text = TransformMoney.GetDollarMoney(m_UserInfoSet.m_BankMoney);
    }

    public void OnChange_WithrawMoney()
    {
        m_WithdrawCheckMark.SetCheckMark(false, false);
    }

    public void Onclick_NextBtn()
    {
        if (m_WithdrawMoney.text.Length == 0)
        {
            m_AlertWithdra_NotEnoughMoney.SetActive(false);
            m_AlertWithdraw_Insufficient.SetActive(false);
            m_WithdrawCheckMark.SetCheckMark(false, true);
            return;
        }

        UInt64 withrawMoney = UInt64.Parse(m_WithdrawMoney.text);
        m_SelectWithdrawMoney.text = withrawMoney.ToString();
        // for dollar
        withrawMoney *= 100;
        if(withrawMoney<1000)
        {
            m_AlertWithdra_NotEnoughMoney.SetActive(true);
            m_AlertWithdraw_Insufficient.SetActive(false);
            m_WithdrawCheckMark.SetCheckMark(false, true);
            return;
        }
        else if(withrawMoney > m_UserInfoSet.m_BankMoney)
        {
            m_AlertWithdra_NotEnoughMoney.SetActive(false);
            m_AlertWithdraw_Insufficient.SetActive(true);
            m_WithdrawCheckMark.SetCheckMark(false, true);
            return;
        }

        m_NowProcIdx++;
        SetWithdrawalProcPanel(m_NowProcIdx);
    }

    public void Onclick_BackBtn()
    {
        m_NowProcIdx--;
        SetWithdrawalProcPanel(m_NowProcIdx);
    }

    public void Onclick_SubmitBtn()
    {
        if (m_Bank.text.Length == 0 || m_BankAccountName.text.Length == 0 || m_BankAccountNumber.text.Length == 0)
        {
            AlertPanel.Instance.StartAlert(2, AlertType.BankWithdrawalFail);
            
            return;
        }
        UInt64 Money = UInt64.Parse(m_SelectWithdrawMoney.text);
        //for dollar
        Money *= 100;
        TexasHoldemClient.Instance.SendWithdrawal(Money, m_Bank.text, m_BankAccountName.text, m_BankAccountNumber.text);
        AlertPanel.Instance.StartAlert(2, AlertType.BankWithdrawalSucc);
        // 메세지 보낼것;;

        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);

        // set user bank data
        SetUserBankData();

        Init();
    }

    public void SetWithdrawalProcPanel(int idx)
    {
        m_AlertWithdra_NotEnoughMoney.SetActive(false);
        m_AlertWithdraw_Insufficient.SetActive(false);
        m_WithdrawCheckMark.SetCheckMark(false, false);
        if (idx < 0 || idx >= m_WithdrawalProcPanel.Length)
            return;

        for (int i = 0; i < m_WithdrawalProcPanel.Length; i++)
        {
            m_WithdrawalProcPanel[i].SetActive(false);
        }
        m_WithdrawalProcPanel[idx].SetActive(true);

        for (int i = 0; i < m_DotImage.Length; i++)
        {
            m_DotImage[i].sprite = m_BlackDot;
        }
        m_DotImage[idx].sprite = m_WhiteDot;
    }

}
