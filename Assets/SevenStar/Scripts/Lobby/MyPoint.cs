using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyPoint : MonoBehaviour
{
    public GameObject m_MyPointPanel;
    public GameObject m_GiftPanel;
    public UserInfoSet m_UserInfoSet;

    [Header("My Point")]
    public Text m_Money;
    public Text m_BankMoney;
    public Toggle m_ToBankToggle;
    public Toggle m_ToGamePointToggle;
    public GameObject m_ToBankToggleDefault;
    public GameObject m_ToGamePointToggleDefault;
    public InputField m_Value;

    [Header("Gift")]
    public InputField m_GiftUserIDInputFiled;
    public InputField m_GiftValueInputFiled;
    public InputFieldCheckMark m_IDCheckMark;
    public InputFieldCheckMark m_ValueCheckMark;
    public GameObject m_AlertID;
    public GameObject m_AlertValue;

    public void SetMyPoiintPanel(bool isGift)
    {
        if (isGift == false)
        {
            m_MyPointPanel.SetActive(true);
            m_GiftPanel.SetActive(false);
        }
        else
        {
            m_MyPointPanel.SetActive(false);
            m_GiftPanel.SetActive(true);
        }
        m_GiftUserIDInputFiled.text = "";
        m_GiftValueInputFiled.text = "";
        m_AlertID.SetActive(false);
        m_AlertValue.SetActive(false);
        m_IDCheckMark.SetCheckMark(false, false);
        m_ValueCheckMark.SetCheckMark(false, false);
    }

    private void OnEnable()
    {
        TexasHoldemClient c = TexasHoldemClient.Instance;
        c.SendGetUserInfo(LobbyLogic.Instance.m_UserInfo.m_UserIndex);
        c.SendGetBankMoney();

        SetMyPoiintPanel(false);
    }

    public void OnClick_Send()
    {
        bool isSendToBank = GetSendMode();
        if(isSendToBank) // to bank
        {
            BankDepositOk();
        }
        else // band -> game point
        {
            BankWithdrawalOK();
        }
    }

    public void BankDepositOk()
    {
        if (LobbyLogic.Instance.m_UserInfo.m_UserIndex <= 0)
            return;
        TexasHoldemClient c = TexasHoldemClient.Instance;
        if (m_Value.text.Length == 0) return;

        UInt64 depositMoney = UInt64.Parse(m_Value.text);
        //for dollar
        depositMoney *= 100;
        if (depositMoney == 0)
        {
            //alert
            AlertPanel.Instance.StartAlert(2, AlertType.BankDepositFail);
            return;
        }
        if (depositMoney > LobbyLogic.Instance.m_UserInfo.m_Money)
        {
            //alert
            AlertPanel.Instance.StartAlert(2, AlertType.BankDepositFail);
            m_Value.text = LobbyLogic.Instance.m_UserInfo.m_Money.ToString();
            return;
        }

        c.SendBankInMoney(depositMoney);

        c.SendGetUserInfo(LobbyLogic.Instance.m_UserInfo.m_UserIndex);
        c.SendGetBankMoney();

        AlertPanel.Instance.StartAlert(2, AlertType.BankDepositSucc);
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
        m_Value.text = "";
    }

    public void BankWithdrawalOK()
    {
        if (LobbyLogic.Instance.m_UserInfo.m_UserIndex <= 0)
            return;
        TexasHoldemClient c = TexasHoldemClient.Instance;
        if (m_Value.text.Length == 0) return;

        UInt64 withdrawMoney = UInt64.Parse(m_Value.text);
        withdrawMoney *= 100;
        if (withdrawMoney == 0)
        {
            //alert.
            AlertPanel.Instance.StartAlert(2, AlertType.BankWithdrawalFail);
            return;
        }
        if (withdrawMoney > m_UserInfoSet.m_BankMoney)
        {
            //alert
            AlertPanel.Instance.StartAlert(2, AlertType.BankWithdrawalFail);
            m_Value.text = m_BankMoney.ToString();
            return;
        }

        c.SendBankOutMoney(withdrawMoney);

        c.SendGetUserInfo(LobbyLogic.Instance.m_UserInfo.m_UserIndex);
        c.SendGetBankMoney();

        // close withdrawal panel
        AlertPanel.Instance.StartAlert(2, AlertType.BankWithdrawalSucc);
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
        m_Value.text = "";
    }

    private bool GetSendMode()
    {
        if (m_ToBankToggle.isOn)
            return true;
        return false;
    }

    private void Update()
    {
        SetUserMoney();
        CheckToggleImage();
    }

    private void SetUserMoney()
    {
        if (m_Money == null || m_BankMoney == null)
            return;


        m_Money.text = TransformMoney.GetDollarMoney(m_UserInfoSet.m_Money);
        m_BankMoney.text = TransformMoney.GetDollarMoney(m_UserInfoSet.m_BankMoney);
    }

    private void CheckToggleImage()
    {
        if(m_ToBankToggle.isOn)
        {
            m_ToBankToggleDefault.SetActive(false);
            m_ToGamePointToggleDefault.SetActive(true);
        }
        else
        {
            m_ToBankToggleDefault.SetActive(true);
            m_ToGamePointToggleDefault.SetActive(false);
        }
    }




    ///////////////////////////////////////// gift
    public void OnClick_GiftOK()
    {
        if (m_GiftUserIDInputFiled.text.Length == 0)
        {
            m_AlertID.SetActive(true);
            m_IDCheckMark.SetCheckMark(false, true);

            return;
        }
        if (m_GiftValueInputFiled.text == "0" || m_GiftValueInputFiled.text.Length == 0)
        {
            m_AlertValue.SetActive(true);
            m_ValueCheckMark.SetCheckMark(false, true);
            return;
        }

        UInt64 giftMoney = UInt64.Parse(m_GiftValueInputFiled.text);
        //for dollar
        giftMoney *= 100;
        if (giftMoney > LobbyLogic.Instance.m_UserInfo.m_Money)
        {
            //alert
            AlertPanel.Instance.StartAlert(2, AlertType.BankGiftFail_NotEnoughMoney);
            return;
        }

        TexasHoldemClient.Instance.SendGiftMoney(m_GiftUserIDInputFiled.text, giftMoney);
        //result
        StartCoroutine(GiftResultWork());
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
    }

    IEnumerator GiftResultWork()
    {
        TexasHoldemClient c = TexasHoldemClient.Instance;
        RecvPacketObject obj = null;
        while (obj == null)
        {
            obj = c.PopPacketObject(Protocols.MoneyGift);
            yield return null;
        }

        ParserUserInfo.MoneyGiftResult r = ParserUserInfo.MoneyGiftResult.NoHaveMoney;
        ParserUserInfo.GetMoneyGiftReusult(obj, ref r);
        OnGiftResult(r);
        c.SendGetUserInfo(LobbyLogic.Instance.m_UserInfo.m_UserIndex);
    }
    public void OnGiftResult(ParserUserInfo.MoneyGiftResult r)
    {
        switch (r)
        {
            case ParserUserInfo.MoneyGiftResult.Success:
                AlertPanel.Instance.StartAlert(2, AlertType.BankGiftSucc);
                break;
            case ParserUserInfo.MoneyGiftResult.NoUser:
                AlertPanel.Instance.StartAlert(2, AlertType.BankGiftFail_IDNotExist);
                m_GiftUserIDInputFiled.text = "";
                break;
            case ParserUserInfo.MoneyGiftResult.NoHaveMoney:
                AlertPanel.Instance.StartAlert(2, AlertType.BankGiftFail_NotEnoughMoney);
                break;
        }
    }
}
