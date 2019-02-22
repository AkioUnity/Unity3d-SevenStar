using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class LobbyBottom : MonoBehaviour
{
    [Header("Create Room")]
    public InputField m_CreateRoomName;

    [Header("Bank")]
    public GameObject m_BankPanelRoot;
    public GameObject m_BankPanel;
    public GameObject m_BankDepositPanel;
    public GameObject m_BankWithdrawalPanel;
    public GameObject m_BankGiftPanel;
    public Text m_BalanceText;
    public Text m_MyPointText;
    public Text m_TotalPointText;
    public Text m_BankDepositBalanceText;
    public InputField m_BankDepositValue;
    public Text m_BankWithdrawalBalanceText;
    public InputField m_BankWithdrawalValue;
    public InputField m_GiftNickName;
    public InputField m_GiftValue;

    [Header("Deposit Request")]
    public Text m_NickNameText;
    public InputField m_DRValue;
    public InputField m_BankAccountName;
    public GameObject m_DRValue_Alert;
    public GameObject m_BankAccountName_Alert;
    public InputFieldCheckMark m_DRValueCheckMark;
    public InputFieldCheckMark m_BankAccountCheckMark;

    [Header("Withdrawal")]
    public WithdrawalProc m_WithdrawProc;

    [Header("Item Shop")]
    public GameObject m_ItemShopPanel;
    public Text m_ItemShopMyMoneyText;
    public GameObject m_AvataContentHolder;
    public Toggle[] m_AvataItems;
    public int m_SelectedToggleIdx=0;

    private UInt64 m_BankMoney=0;

    private void Start()
    {
        int count = m_AvataContentHolder.transform.childCount;
        m_AvataItems = new Toggle[count];
        for (int i = 0; i < count; i++)
        {
            m_AvataItems[i] = m_AvataContentHolder.transform.GetChild(i).GetComponent<Toggle>();
        }

        // set itemshop avata image
        /*
        Sprite[] avataSprites = AvataMgr.Instance.m_AvataSpriteList.ToArray();
        for (int i = 0; i < avataSprites.Length; i++)
        {
            m_AvataItems[i].transform.GetChild(0).GetComponent<Image>().sprite = AvataMgr.Instance.m_AvataSpriteList[i];
        }*/
    }

    ///////////////////////////////////////////////////  OnClick CreateRoom ////////////////////

    public void OnClick_CreateRoom()
    {
        //ActivePopupWork(m_CreateRoomPanel);
        LobbyLogic.Instance.m_Alert_RoomNameDuplication.SetActive(false);
        LobbyPanels.Instance.SelectBottomBtns(LobbyPanelType.CreateRoom);
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
    }

    public void OnClick_CreateRoomOK()
    {
        UInt64 leastMoney = 40;
        int blindType = LobbyLogic.Instance.GetBlindType_CRToggle();
        if (blindType == 2)
            leastMoney = 100;
        else if (blindType == 3)
            leastMoney = 200;
        else if (blindType == 4)
            leastMoney = 400;

        if (LobbyLogic.Instance.m_UserInfo.m_Money == 0 || LobbyLogic.Instance.m_UserInfo.m_Money < leastMoney)
        {
            AlertPanel.Instance.StartAlert(2, AlertType.NoMoney);
            OnClick_CreateRoomCancel();
            return;
        }

        if (m_CreateRoomName.text.Length == 0)
        {
            Debug.Log("방이름쓰세요");
            return;
        }
        // create room routine
        StartCoroutine(LobbyLogic.Instance.CreateRoom(m_CreateRoomName.text));
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
    }

    public void OnClick_CreateRoomCancel()
    {
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
    }

    ///////////////////////////////////////////////////  OnClick Bank ////////////////////

    public void OnClick_Bank()
    {
        if (m_BankPanelRoot == null || m_BankPanelRoot.activeSelf == true)
            return;
        ActivePopupWork(m_BankPanelRoot);

        TexasHoldemClient c = TexasHoldemClient.Instance;
        c.SendGetUserInfo(LobbyLogic.Instance.m_UserInfo.m_UserIndex);
        c.SendGetBankMoney();

        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
    }

    public void OnClick_BankCancel()
    {
        m_BankPanelRoot.SetActive(false);
        
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
    }

    public void OnClick_BankDeposit()
    {
        if (m_BankDepositPanel == null || m_BankDepositPanel.activeSelf == true)
            return;
        ActivePopupWork(m_BankDepositPanel);
        m_BankPanel.SetActive(false);
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
    }

    public void OnClick_BankDepositOk()
    {
        if (LobbyLogic.Instance.m_UserInfo.m_UserIndex <= 0)
            return;
        TexasHoldemClient c = TexasHoldemClient.Instance;
        if (m_BankDepositValue.text.Length == 0) return;

        UInt64 depositMoney = UInt64.Parse(m_BankDepositValue.text);
        if (depositMoney == 0)
        {
            //alert
            AlertPanel.Instance.StartAlert(2, AlertType.BankDepositFail);
            return;
        }
        if(depositMoney > LobbyLogic.Instance.m_UserInfo.m_Money)
        {
            //alert
            AlertPanel.Instance.StartAlert(2, AlertType.BankDepositFail);
            m_BankDepositValue.text = LobbyLogic.Instance.m_UserInfo.m_Money.ToString();
            return;
        }

        c.SendBankInMoney(depositMoney);

        c.SendGetUserInfo(LobbyLogic.Instance.m_UserInfo.m_UserIndex);
        c.SendGetBankMoney();

        // close deposit panel
        OnClick_BankDepositClose();
        AlertPanel.Instance.StartAlert(2, AlertType.BankDepositSucc);
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
    }

    public void OnClick_BankDepositClose()
    {
        m_BankDepositValue.text = "";
        m_BankDepositPanel.SetActive(false);
        m_BankPanel.SetActive(true);
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
    }

    public void OnClick_BankWithdrawal()
    {
        if (m_BankWithdrawalPanel == null || m_BankWithdrawalPanel.activeSelf == true)
            return;
        ActivePopupWork(m_BankWithdrawalPanel);
        m_BankPanel.SetActive(false);
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
    }

    public void OnClick_BankWithdrawalOK()
    {
        if (LobbyLogic.Instance.m_UserInfo.m_UserIndex <= 0)
            return;
        TexasHoldemClient c = TexasHoldemClient.Instance;
        if (m_BankWithdrawalValue.text.Length == 0) return;

        UInt64 withdrawMoney = UInt64.Parse(m_BankWithdrawalValue.text);
        if (withdrawMoney == 0)
        {
            //alert.
            AlertPanel.Instance.StartAlert(2, AlertType.BankWithdrawalFail);
            return;
        }
        if(withdrawMoney > m_BankMoney)
        {
            //alert
            AlertPanel.Instance.StartAlert(2, AlertType.BankWithdrawalFail);
            m_BankWithdrawalValue.text = m_BankMoney.ToString();
            return;
        }

        c.SendBankOutMoney(withdrawMoney);

        c.SendGetUserInfo(LobbyLogic.Instance.m_UserInfo.m_UserIndex);
        c.SendGetBankMoney();

        // close withdrawal panel
        OnClick_BankWithdrawalClose();
        AlertPanel.Instance.StartAlert(2, AlertType.BankWithdrawalSucc);
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
    }

    public void OnClick_BankWithdrawalClose()
    {
        m_BankWithdrawalValue.text = "";
        m_BankWithdrawalPanel.SetActive(false);
        m_BankPanel.SetActive(true);
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
    }

    public void OnClick_Gift()
    {
        if (m_BankGiftPanel == null || m_BankGiftPanel.activeSelf == true)
            return;
        ActivePopupWork(m_BankGiftPanel);
        m_BankPanel.SetActive(false);
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
    }

    public void OnClick_GiftOK()
    {
        if(m_GiftNickName.text.Length == 0)
        {
            AlertPanel.Instance.StartAlert(2, AlertType.BankGiftFail_IDNotExist);
            return;
        }
        if (m_GiftValue.text=="0" || m_GiftValue.text.Length==0)
            return;

        UInt64 giftMoney = UInt64.Parse(m_GiftValue.text);
        if (giftMoney > LobbyLogic.Instance.m_UserInfo.m_Money)
        {
            //alert
            AlertPanel.Instance.StartAlert(2, AlertType.BankGiftFail_NotEnoughMoney);
            return;
        }

        TexasHoldemClient.Instance.SendGiftMoney( m_GiftNickName.text, giftMoney);
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
                m_BankGiftPanel.SetActive(false);
                m_BankPanel.SetActive(true);
                break;
            case ParserUserInfo.MoneyGiftResult.NoUser:
                AlertPanel.Instance.StartAlert(2, AlertType.BankGiftFail_IDNotExist);
                m_GiftNickName.text = "";
                break;
            case ParserUserInfo.MoneyGiftResult.NoHaveMoney:
                AlertPanel.Instance.StartAlert(2, AlertType.BankGiftFail_NotEnoughMoney);
                break;
        }


    }

    public void OnClick_GiftClose()
    {
        m_BankGiftPanel.SetActive(false);
        m_BankPanel.SetActive(true);
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
    }

    ///////////////////////////////////////////////////  OnClick DepositRequest ////////////////////

    public void OnClick_DepositRequest()
    {
        m_NickNameText.text = LoginSession.Instance.m_LoginID;
    }

    public void OnClick_DepositRequestOk()
    {
        if(m_DRValue == null || m_DRValue.text.Length == 0 || m_DRValue.text == "0" || m_BankAccountName.text.Length==0)
        {
            //AlertPanel.Instance.StartAlert(2, AlertType.ChargeRequestFail);
            m_DRValue_Alert.SetActive(true);
            m_BankAccountName_Alert.SetActive(true);
            m_DRValueCheckMark.SetCheckMark(false,true);
            m_BankAccountCheckMark.SetCheckMark(false, true);
            return;
        }
        UInt64 Money = UInt64.Parse(m_DRValue.text);
        //TexasHoldemClient.Instance.SendDepositRequest(Money);
        //for dollar
        Money *= 100;
        TexasHoldemClient.Instance.SendChargeRequest(Money, m_BankAccountName.text);

        AlertPanel.Instance.StartAlert(2, AlertType.ChargeRequestSucc);
        // 메세지 보내기로 대체;;
        m_DRValue_Alert.SetActive(false);
        m_BankAccountName_Alert.SetActive(false);
        m_DRValueCheckMark.SetCheckMark(false, false);
        m_BankAccountCheckMark.SetCheckMark(false, false);

        m_DRValue.text = "";
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
    }

    public void OnClick_DepositRequestCancel()
    {
        m_DRValue.text = "";
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
    }

    public void OnValueChange_DepositInput()
    {
        m_DRValue_Alert.SetActive(false);
        m_BankAccountName_Alert.SetActive(false);
        m_DRValueCheckMark.SetCheckMark(false, false);
        m_BankAccountCheckMark.SetCheckMark(false, false);
    }

    ///////////////////////////////////////////////////  OnClick Withdrawal ////////////////////

    public void OnClick_Withdrawal()
    {
        m_WithdrawProc.Init();
    }

    ///////////////////////////////////////////////////  OnClick ItemShop ////////////////////

    public void OnClick_ItemShop()
    {
        if (m_ItemShopPanel == null || m_ItemShopPanel.activeSelf == true)
            return;
        m_SelectedToggleIdx = -1;
        ActivePopupWork(m_ItemShopPanel);
        m_ItemShopMyMoneyText.text = TransformMoney.MoneyTransform(LobbyLogic.Instance.m_UserInfo.m_Money,false);
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
    }

    public void OnClick_ItemShopBuy()
    {
        m_SelectedToggleIdx = GetToggleSelectedIdx();
        if (m_SelectedToggleIdx == -1)
            return;
        if(m_SelectedToggleIdx == LobbyLogic.Instance.m_UserInfo.m_Avatar)
        {
            AlertPanel.Instance.StartAlert(2, AlertType.Avata_Same);
            return;
        }
        /*
        int price = m_AvataItems[m_SelectedToggleIdx].GetComponent<AvataItem>().m_AvataPrice;
        if (LobbyLogic.Instance.m_UserInfo.m_Money < (UInt64)price)
        {
            AlertPanel.Instance.StartAlert(2, AlertType.NoMoney);
            m_ItemShopPanel.SetActive(false);
            return;
        }

        TexasHoldemClient c = TexasHoldemClient.Instance;
        c.Send_int(Protocols.PlayerSetAvatar, m_SelectedToggleIdx);
        c.SendGetUserInfo(LobbyLogic.Instance.m_UserInfo.m_UserIndex);
        UInt64 money = LobbyLogic.Instance.m_UserInfo.m_Money - (UInt64)price;
        c.Send_UInt64(Protocols.PlayerSetMoney, money);
        c.SendGetUserInfo(LobbyLogic.Instance.m_UserInfo.m_UserIndex);
        */
        // set avata
        TexasHoldemClient c = TexasHoldemClient.Instance;
        c.Send_int(Protocols.PlayerSetAvatar, m_SelectedToggleIdx);
        c.SendGetUserInfo(LobbyLogic.Instance.m_UserInfo.m_UserIndex);

        //m_ItemShopPanel.SetActive(false);
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
    }

    public void OnClick_ItemShopCancel()
    {
        for (int i = 0; i < m_AvataItems.Length; i++)
            m_AvataItems[i].isOn = false;
        m_ItemShopPanel.SetActive(false);
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
    }

    public void OnClick_ToggleItem()
    {
        m_SelectedToggleIdx = GetToggleSelectedIdx();
    }

    private int GetToggleSelectedIdx()
    {
        for (int i = 0; i < m_AvataItems.Length; i++)
        {
            if(m_AvataItems[i].isOn)
                return i;
        }
        return -1;
    }

   

    /////////////////////////////////////////////////////////////////////////////////////////////

    private void Update()
    {
        /*try
        {
            RecvPacketObject obj = TexasHoldemClient.Instance.PopPacketObject(Protocols.GetBankMoney);
            if (obj != null)
            {
                UInt64 bankMoney = 0;
                ParserUserInfo.GetBankMoney(obj, ref bankMoney);
                m_BankMoney = bankMoney;
            }
        }
        catch (Exception err)
        {
            Debug.Log(err.ToString());
        }

        SetBankUI(m_BankMoney);*/
        SetDepositUI();
        CheckCreateRoomName();
    }

    private void CheckCreateRoomName()
    {
        if(m_CreateRoomName)
            m_CreateRoomName.text = Regex.Replace(m_CreateRoomName.text, @"[^a-zA-Z0-9]", "");
    }

    private void SetBankUI(UInt64 bankMoney)
    {
        if (m_BalanceText)
            m_BalanceText.text = TransformMoney.MoneyTransform(bankMoney,false);
        if (m_MyPointText)
            m_MyPointText.text = TransformMoney.MoneyTransform(LobbyLogic.Instance.m_UserInfo.m_Money,false);
        if (m_TotalPointText)
            m_TotalPointText.text = TransformMoney.MoneyTransform(bankMoney + LobbyLogic.Instance.m_UserInfo.m_Money,false);
        if (m_BankDepositBalanceText)
            m_BankDepositBalanceText.text = bankMoney.ToString();
        if (m_BankWithdrawalBalanceText)
            m_BankWithdrawalBalanceText.text = bankMoney.ToString();
    }

    private void SetDepositUI()
    {
        if (m_NickNameText)
            //m_NickNameText.text = LobbyLogic.Instance.m_UserInfo.m_UserNameValue;
            m_NickNameText.text = LoginSession.Instance.m_LoginID;
    }

    private void ActivePopupWork(GameObject obj)
    {
        //obj.transform.localScale = Vector3.zero;
        obj.SetActive(true);
        //StartCoroutine(PopupWork(obj));
    }

    private IEnumerator PopupWork(GameObject obj)
    {
        Vector3 scale = obj.transform.localScale;
        float per = 0;
        while (per < 1)
        {
            per += Time.deltaTime;
            scale = Vector3.Lerp(scale, Vector3.one, per);
            obj.transform.localScale = scale;
            yield return null;
        }
    }
}
