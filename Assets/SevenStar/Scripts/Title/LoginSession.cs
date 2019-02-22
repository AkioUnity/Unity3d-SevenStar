using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtil;

public class LoginSession : UtilSingleton<LoginSession>
{
    public string m_LoginID;
    public string m_LoginPass;

    //playerprefs
    public string m_BankName;
    public string m_AccountName;
    public string m_AccountNumber;

    public void Init()
    {
        m_LoginID = "";
        m_LoginPass = "";

        m_BankName = "";
        m_AccountName = "";
        m_AccountNumber = "";
    }
    
    public void GetBankData()
    {
        if (m_LoginID.Equals(""))
            return;
        m_BankName = PlayerPrefs.GetString(m_LoginID+"_BankName");
        m_AccountName = PlayerPrefs.GetString(m_LoginID + "_AccountName");
        m_AccountNumber = PlayerPrefs.GetString(m_LoginID+ "_AccountNumber");
    }
}
