using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProfileProc : MonoBehaviour
{
    public UserInfoSet m_UserInfo;
    public Text m_ProfilePanelTitle;

    [Header("Profile Process Panel")]
    public GameObject[] m_Profilepanels;
    public Text m_MainName;
    public Text m_MainNickname;
    public Text m_MainPhoneNumber;

    [Header("Edit Name")]
    public Text m_UserName;
    public InputField m_InputUserName;

    [Header("Edit Nickname")]
    public Text m_UserNickname;
    public InputField m_InputUserNickname;
    public GameObject m_Alert_NicknameDuplication;

    [Header("Edit Phone Number")]
    public Text m_UserPhoneNumber;
    public InputField m_InputUserPhoneNumber;

    [Header("Edit Password")]
    public InputField m_OldPass;
    public InputField m_NewPass;
    public InputField m_ConfirmNewPass;
    public GameObject m_OldPassAlert;
    public GameObject m_PassConfirmAlert;

    private bool m_IsLogoutWorking = false;

    public void Init()
    {
        TexasHoldemClient c = TexasHoldemClient.Instance;
        c.SendGetUserInfo(LobbyLogic.Instance.m_UserInfo.m_UserIndex);
        c.SendGetUserNamePhonenumber();
        SetProfileProcPanel(0);
    }

    public void OnEnable()
    {
        Init();
    }

    private void Update()
    {
        bool re = false;
        if (ParserUserInfo.CheckChangeNickname(ref re))
        {
            if (re)
            {
                Debug.Log("success change nickname");
                TexasHoldemClient c = TexasHoldemClient.Instance;
                c.SendGetUserInfo(LobbyLogic.Instance.m_UserInfo.m_UserIndex);
            }
            else
                Debug.Log("failed change nickname");
        }

        if (ParserUserInfo.CheckChangePhonenumber(ref re))
        {
            if (re)
            { 
                Debug.Log("success change phone number");
                TexasHoldemClient c = TexasHoldemClient.Instance;
                c.SendGetUserInfo(LobbyLogic.Instance.m_UserInfo.m_UserIndex);
                c.SendGetUserNamePhonenumber();
            }
            else
                Debug.Log("failed change phone number");
        }

        if (ParserUserInfo.CheckChangePassword(ref re))
        {
            if (re)
            {
                Debug.Log("success change password");
                TexasHoldemClient c = TexasHoldemClient.Instance;
                c.SendGetUserInfo(LobbyLogic.Instance.m_UserInfo.m_UserIndex);
                SetProfileProcPanel(0);
            }
            else
                Debug.Log("failed change password");
        }

        InitMainPanel();
        InitNamePanel();
        InitNicknamePanel();
        InitPhoneNumberPanel();
    }

    public void OnClick_Logout()
    {
        if (m_IsLogoutWorking == true)
            return;
        StartCoroutine(LogoutWork());
    }

    
    private IEnumerator LogoutWork()
    {
        m_IsLogoutWorking = true;
        TexasHoldemClient.Instance.SendLogOut();
        LoginSession.Instance.Init();
        //ClientObject.Instance.m_Client.Disconnect();
        yield return null;
        /*while (ClientObject.Instance.m_Client.IsConnect)
        {
            yield return null;
        }
        SceneManager.LoadScene("1_Title");*/
    }

    public void OnClick_BackBtn()
    {
        SetProfileProcPanel(0);
    }

    public void OnClick_EditUserName()
    {
        if (m_InputUserName.text.Length <= 0)
            return;
        TexasHoldemClient c = TexasHoldemClient.Instance;
        c.SendChangeUserName(m_InputUserName.text);

        c.SendGetUserInfo(LobbyLogic.Instance.m_UserInfo.m_UserIndex);
        c.SendGetUserNamePhonenumber();
    }

    public void OnClick_EditUserNickname()
    {
        CheckExistNickname();
    }

    public void OnClick_EditUserPhoneNumber()
    {
        if (m_InputUserPhoneNumber.text.Length <= 0)
            return;
        TexasHoldemClient.Instance.SendChangePhonenumber(m_InputUserPhoneNumber.text);
    }

    public void OnClick_EditUserPassword()
    {
        InitPasswordPanel();
        bool oldPass = CheckOldPass();
        bool passType = CheckPassType();
        bool passConfirm = CheckPassConfirm();
        if(oldPass && passType && passConfirm)
            TexasHoldemClient.Instance.SendChangePassword(m_OldPass.text, m_NewPass.text);
    }


    private bool CheckExistNickname()
    {
        if (m_InputUserNickname.text.Length == 0)
        {
            return false;
        }
        TexasHoldemClient.Instance.SendCheckIDName(1, m_InputUserNickname.text);
        StartCoroutine(CheckIDNameWork());
        return true;
    }
    /// <summary>
    /// type
    /// 0 id
    /// 1 nickname
    /// </summary>
    /// <param name="type"></param>
    private IEnumerator CheckIDNameWork()
    {
        TexasHoldemClient c = TexasHoldemClient.Instance;
        RecvPacketObject obj = TexasHoldemClient.Instance.PopPacketObject(Protocols.CheckIDName);
        while (obj == null)
        {
            obj = c.PopPacketObject(Protocols.CheckIDName);
            yield return null;
        }

        if (obj != null)
        {
            bool r = false;
            ParserUserInfo.CheckIDName(obj, ref r);
            if (r)
            {
                Debug.Log("no overlap");
                TexasHoldemClient.Instance.SendChangeNickname(m_InputUserNickname.text);
                m_Alert_NicknameDuplication.SetActive(false);
            }
            else
            {
                Debug.Log("overlap");
                m_Alert_NicknameDuplication.SetActive(true);
            }
        }
    }

    private bool CheckOldPass()
    {
        if(m_OldPass.text.Length <=0)
        {
            m_OldPassAlert.SetActive(true);
            return false;
        }
        if(m_OldPass.text.Equals(LoginSession.Instance.m_LoginPass) == false)
        {
            m_OldPassAlert.SetActive(true);
            return false;
        }
        return true;
    }

    private bool CheckPassType()
    {
        string pass = m_NewPass.text;
        if (pass.Length <= 0)
            return false;
        if (Regex.IsMatch(pass[0].ToString(), @"[a-zA-Z]") == false)
        {
            m_PassConfirmAlert.SetActive(true);
            return false;
        }
        if (Regex.IsMatch(pass, @"[0-9]") == false)
        {
            m_PassConfirmAlert.SetActive(true);
            return false;
        }
        return true;
    }

    private bool CheckPassConfirm()
    {
        if (m_NewPass.text.Length == 0)
        {
            m_PassConfirmAlert.SetActive(true);
            return false;
        }
        if (m_NewPass.text.Equals(m_ConfirmNewPass.text) == false)
        {
            m_PassConfirmAlert.SetActive(true);
            return false;
        }
        return true;
    }


    public void SetProfileProcPanel(int idx)
    {
        if (idx < 0 || idx >= m_Profilepanels.Length)
            return;

        for (int i = 0; i < m_Profilepanels.Length; i++)
        {
            m_Profilepanels[i].SetActive(false);
        }
        m_Profilepanels[idx].SetActive(true);
        InitPanel(idx);
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
    }

    private void InitPanel(int idx)
    {
        switch (idx)
        {
            case 0:
                m_ProfilePanelTitle.text = "Хувийн мэдээлэл";
                break;
            case 1:
                m_ProfilePanelTitle.text = "Нэр";
                InitNamePanel();
                break;
            case 2:
                m_ProfilePanelTitle.text = "Хоч нэр";
                InitNicknamePanel();
                break;
            case 3:
                m_ProfilePanelTitle.text = "Утасны дугаар";
                InitPhoneNumberPanel();
                break;
            case 4:
                m_ProfilePanelTitle.text = "Профайл зураг";
                break;
            case 5:
                m_ProfilePanelTitle.text = "Нууц үг";
                InitPasswordPanel();
                break;
            case 6:
                m_ProfilePanelTitle.text = "Гаргах";
                break;
        }
    }

    private void InitMainPanel()
    {
        m_MainName.text = m_UserInfo.m_UserNameValue;
        m_MainNickname.text = m_UserInfo.m_UserNicknameValue;
        m_MainPhoneNumber.text = m_UserInfo.m_UserPhoneNumValue;
    }


    private void InitNamePanel()
    {
        m_UserName.text = m_UserInfo.m_UserNameValue;

    }

    private void InitNicknamePanel()
    {
        m_UserNickname.text = m_UserInfo.m_UserNicknameValue;
        m_Alert_NicknameDuplication.SetActive(false);
    }

    private void InitPhoneNumberPanel()
    {
        m_UserPhoneNumber.text = m_UserInfo.m_UserPhoneNumValue;
    }

    private void InitPasswordPanel()
    {
        m_OldPassAlert.SetActive(false);
        m_PassConfirmAlert.SetActive(false);
    }

}

