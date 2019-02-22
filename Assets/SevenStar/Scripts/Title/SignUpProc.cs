using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class SignUpProc : MonoBehaviour
{
    [Header("Signup Components")]
    public InputField m_UserID;
    public InputField m_UserName;
    public InputField m_HP;
    public InputField m_Password;
    public InputField m_PasswordConfirm;
    public InputField m_UserNickName;
    public GameObject m_SignUpSuccPop;
    public GameObject m_SignUpFailPop;
    public Toggle m_ShowPassToggle;

    [Header("Signup dot process")]
    public ProcDot m_Procdot;
    public int m_SignupProcIdx = 0;
    public Text m_AlertID;
    public Text m_AlertName;
    public Text m_AlertPhone;
    public Text m_AlertPass;
    public Text m_AlertNickname;
    public InputFieldCheckMark m_CMID;
    public InputFieldCheckMark m_CMName;
    public InputFieldCheckMark m_CMPhone;
    public InputFieldCheckMark m_CMPass;
    public InputFieldCheckMark m_CMPassConfirm;
    public InputFieldCheckMark m_CMNickname;
    private bool m_IDNameCheck=false;

    private void Start()
    {
        InitSignupPanel();
    }

    public void InitSignupPanel()
    {
        InitAlertText();
        InitInputField();
        InitCheckMark();
        m_SignupProcIdx = 0;
        m_Procdot.SetSignupProcPanel(0);
        m_Procdot.Init();
    }

    private void InitAlertText()
    {
        m_AlertID.text = "";
        m_AlertName.text = "";
        m_AlertPhone.text = "";
        m_AlertNickname.text = "";
        m_AlertPass.text = "";
    }

    private void InitInputField()
    {
        m_UserID.text = "";
        m_UserName.text = "";
        m_HP.text = "";
        m_Password.text = "";
        m_PasswordConfirm.text = "";
        m_UserNickName.text = "";
    }

    private void InitCheckMark()
    {
        m_CMID.m_Img.enabled = false;
        m_CMName.m_Img.enabled = false;
        m_CMPhone.m_Img.enabled = false;
        m_CMPass.m_Img.enabled = false;
        m_CMPassConfirm.m_Img.enabled = false;
        m_CMNickname.m_Img.enabled = false;
    }

    private void Update()
    {
        // check character
        if (m_UserID)
            m_UserID.text = RemoveSpecialCharacters(m_UserID.text);
        if (m_UserName)
            m_UserName.text = RemoveSpecialCharacters(m_UserName.text);
        if (m_UserNickName)
            m_UserNickName.text = RemoveSpecialCharacters(m_UserNickName.text);
        if (m_Password)
        {
            string pass = m_Password.text;
            pass = pass.Replace("\'", "");
            pass = pass.Replace("<<", "<");
            pass = pass.Replace(">>", ">");
            pass = pass.Replace(" ", "");
            m_Password.text = pass;
        }
    }

    public string RemoveSpecialCharacters(string str)
    {
        return Regex.Replace(str, @"[^a-zA-Z0-9]", "");
    }

    public void OnClick_SuccPopOk()
    {
        m_SignUpSuccPop.SetActive(false);
        SevenStarTitle.Instance.OnClick_SignUpCancel();
    }

    public void OnClick_FailPopOk()
    {
        m_SignUpFailPop.SetActive(false);
    }

    public void ShowPasswordToggle()
    {
        if (m_ShowPassToggle == null)
            return;
        if(m_ShowPassToggle.isOn)
            m_Password.contentType = InputField.ContentType.Standard;
        else
            m_Password.contentType = InputField.ContentType.Password;
        m_Password.Select();
    }

    public void OnClick_Next()
    {
        InitAlertText();
        StartCoroutine(CheckNextProcWork(m_SignupProcIdx));
    }

    private IEnumerator CheckNextProcWork(int proc)
    {
        m_IDNameCheck = false;
        bool isAllChecked = false;
        switch (proc)
        {
            case 0:
//                if (CheckExistID())
//                {
//                    yield return CheckIDNameWork();
//                    m_CMID.SetCheckMark(true);
//                    if (m_IDNameCheck == false)
//                    {
//                        AlertPanel.Instance.StartAlertOtherObj(m_AlertID.gameObject, AlertType.SignUpIDOverLapping);
//                        m_CMID.SetCheckMark(false);
//                    }
//                }
//                
//                CheckName();
//                CheckPhone();
//                if (CheckName() && CheckPhone() && m_IDNameCheck)
                    isAllChecked = true;
                break;
            case 1:
                if (CheckExistNickname())
                {
//                    yield return CheckIDNameWork();
//                    if (m_IDNameCheck)
//                    {
                        isAllChecked = true;
                        m_CMNickname.SetCheckMark(true);
//                    }
//                    else
//                    {
//                        AlertPanel.Instance.StartAlertOtherObj(m_AlertNickname.gameObject, AlertType.SignUpNickNameOverlapping);
//                        m_CMNickname.SetCheckMark(false);
//                    }
                }
                break;
            case 2:
                // check pass
                if(CheckPassType())
                    m_CMPass.SetCheckMark(true);
                if(CheckPassConfirm())
                    m_CMPassConfirm.SetCheckMark(true);
                if(CheckPassType() && CheckPassConfirm())
                    isAllChecked = true;
                break;
        }
        yield return null;
        if (isAllChecked)
        {
            m_SignupProcIdx++;
            m_Procdot.SetSignupProcPanel(m_SignupProcIdx);
        }
    }

    private bool CheckExistID()
    {
        if (m_UserID.text.Length == 0)
        {
            AlertPanel.Instance.StartAlertOtherObj(m_AlertID.gameObject, AlertType.FillTheBlank);
            m_CMID.SetCheckMark(false);
            return false;
        }
        TexasHoldemClient.Instance.SendCheckIDName(0, m_UserID.text);
        RecvPacketObject obj = TexasHoldemClient.Instance.PopPacketObject(Protocols.CheckIDName);
        return true;
    }

    private bool CheckName()
    {
        if (m_UserName.text.Length == 0)
        {
            AlertPanel.Instance.StartAlertOtherObj(m_AlertName.gameObject, AlertType.FillTheBlank);
            m_CMName.SetCheckMark(false);
            return false;
        }
        m_CMName.SetCheckMark(true);
        return true;
    }

    private bool CheckPhone()
    {
        if (m_HP.text.Length == 0)
        {
            AlertPanel.Instance.StartAlertOtherObj(m_AlertPhone.gameObject, AlertType.FillTheBlank);
            m_CMPhone.SetCheckMark(false);
            return false;
        }
        m_CMPhone.SetCheckMark(true);
        return true;
    }

    private bool CheckExistNickname()
    {
        if (m_UserNickName.text.Length == 0)
        {
            AlertPanel.Instance.StartAlertOtherObj(m_AlertName.gameObject, AlertType.FillTheBlank);
            m_CMNickname.SetCheckMark(false);
            return false;
        }
        TexasHoldemClient.Instance.SendCheckIDName(1, m_UserNickName.text);
        RecvPacketObject obj = TexasHoldemClient.Instance.PopPacketObject(Protocols.CheckIDName);
        return true;
    }

    private bool CheckPassType()
    {
        string pass = m_Password.text;
        if (Regex.IsMatch(pass[0].ToString(), @"[a-zA-Z]") == false)
        {
            AlertPanel.Instance.StartAlertOtherObj(m_AlertPass.gameObject, AlertType.PasswordConfirmFail);
            m_CMPass.SetCheckMark(false);
            return false;
        }
        if (Regex.IsMatch(pass, @"[0-9]") == false)
        {
            AlertPanel.Instance.StartAlertOtherObj(m_AlertPass.gameObject, AlertType.PasswordConfirmFail);
            m_CMPass.SetCheckMark(false);
            return false;
        }
        return true;
    }

    private bool CheckPassConfirm()
    {
        if(m_Password.text.Length == 0)
        {
            AlertPanel.Instance.StartAlertOtherObj(m_AlertPass.gameObject, AlertType.FillTheBlank);
            m_CMPassConfirm.SetCheckMark(false);
            return false;
        }
        if(m_Password.text.Equals(m_PasswordConfirm.text) == false)
        {
            AlertPanel.Instance.StartAlertOtherObj(m_AlertPass.gameObject, AlertType.PasswordConfirmFail);
            m_CMPassConfirm.SetCheckMark(false);
            return false;
        }
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
                m_IDNameCheck = true;
            }
            else
            {
                Debug.Log("overlap");
                m_IDNameCheck = false;
            }

        }
    }



    public void TryRegister()
    {
        RegistWork();
    }

    private void RegistWork()
    {
        string id = m_UserID.text;
        string pass = m_Password.text;
        string name = m_UserName.text;
        string hp = m_HP.text;
        string nickname = m_UserNickName.text;
        if (id.Length == 0 || pass.Length == 0 || name.Length == 0 || nickname.Length == 0 || hp.Length == 0)
        {
            m_SignUpFailPop.SetActive(true);
            return;
        }
        Debug.Log("등록중");
        StartCoroutine(RegisterWork(id, pass, nickname, name, hp));
    }

    private IEnumerator RegisterWork(string id, string pass, string nickname, string name, string phone)
    {
        TexasHoldemClient c = TexasHoldemClient.Instance;
        string[] strArr = new string[5] { id, pass, nickname, name, phone };
        string str = TinyXmlReader.GetString("id", id, "pass", pass, "nick", nickname, "name", name, "phone", phone);
        byte[] data = Encoding.UTF8.GetBytes(str);
        if (c.Send(Protocols.UserRegister, data) == false)
        {
            Debug.Log("네트워크오류");
            yield break;
        }

        RecvPacketObject obj = null;
        while (obj == null)
        {
            obj = c.PopPacketObject(Protocols.UserRegister);
            yield return null;
        }
        ParserUserInfo.RegisterResult r = ParserUserInfo.RegisterResult.None;
        ParserUserInfo.GetUserRegisterResult(obj, ref r);
        Debug.Log(r);
        OnRegisterResult(r);
    }

    private void OnRegisterResult(ParserUserInfo.RegisterResult r)
    {
        switch (r)
        {
            case ParserUserInfo.RegisterResult.Success:
                Debug.Log("등록성공");
                //m_SignUpSuccPop.SetActive(true);
                SevenStarTitle.Instance.m_SignUpPanel.SetActive(false);
                break;
            case ParserUserInfo.RegisterResult.Fail_ID:
                Debug.Log("중복 아이디");
                //m_SignUpFailPop.SetActive(true);
                AlertPanel.Instance.StartAlert(2, AlertType.SignUpIDOverLapping);
                break;
            case ParserUserInfo.RegisterResult.Fail_Nickname:
                Debug.Log("중복 닉네임");
//                m_SignUpFailPop.SetActive(true);
                AlertPanel.Instance.StartAlert(2, AlertType.SignUpNickNameOverlapping);
                break;
            default:
                Debug.Log("알수없는오류");
                m_SignUpFailPop.SetActive(true);
                break;
        }
    }
}
