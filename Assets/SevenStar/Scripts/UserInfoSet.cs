using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInfoSet : MonoBehaviour {

    public Image m_AvataImage;
    public Text m_UserName;
    public Text m_UserMoney;
    public Text m_UserBankMoney;
        
    public int m_UserIndex = -1;
    public string m_UserNicknameValue = "";
    public string m_UserNameValue = "";
    public string m_UserPhoneNumValue = "";
    public UInt64 m_Money = 0;
    public UInt64 m_BankMoney = 0;
    public int m_Avatar = 0;

    private void OnEnable()
    {
        TexasHoldemClient c = TexasHoldemClient.Instance;
        c.SendGetUserInfo(m_UserIndex);
        c.SendGetBankMoney();
    }

    public int UserIndex
    {
        get
        {
            return m_UserIndex;
        }
        set
        {
            m_UserIndex = value;
        }
    }

	void Update ()
    {
        if (m_UserIndex <= 0)
            NonUser();
        else
            SetUser();

    }

    void NonUser()
    {
        SetAvata(-1);
        SetName("No User");
        SetMoney(0);
    }

    void SetUser()
    {
        UserInfo info = UserMgr.Instance.GetUserInfo(m_UserIndex);
        if(info==null)
        {
            NonUser();
            return;
        }
        SetName(info.UserName);
        SetMoney(info.UserMoney);
        SetAvata(info.Avatar);
        m_BankMoney = info.bankMoney;
        
        /*if(m_BankMoney >= 100)
            m_UserBankMoney.text = (m_BankMoney / 100).ToString();
        else*/
        m_UserBankMoney.text = TransformMoney.GetDollarMoney(m_BankMoney);
        string name = "";
        string phone = "";

        if (ParserUserInfo.CheckUserNamePhonenumber(ref name, ref phone) == false)
            return;
        m_UserNameValue = name;
        m_UserPhoneNumValue = phone;
    }

    void SetAvata(int n)
    {
        if (m_AvataImage == null)
            return;
        m_Avatar = n;
        m_AvataImage.sprite = AvataMgr.Instance.GetAvataSprite(n);
    }

    void SetName(string str)
    {
        if (m_UserName == null)
            return;
        m_UserNicknameValue = str;
        m_UserName.text = str;
    }

    void SetMoney(UInt64 Money)
    {
        if (m_UserMoney == null)
            return;
        m_Money = Money;
        //m_UserMoney.text = Money.ToString();
        //m_UserMoney.text = TransformMoney.MoneyTransform_BMK(Money,false);
        //m_UserMoney.text = TransformMoney.GetDollarMoney(m_Money);
        /*if (m_Money >= 100)
            m_UserMoney.text = (m_Money / 100).ToString();
        else*/
            m_UserMoney.text = TransformMoney.GetDollarMoney(m_Money);
    }
}
