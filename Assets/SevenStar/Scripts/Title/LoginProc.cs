using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginProc : MonoBehaviour
{
    public GameObject m_LoadingPanel;
    public InputField m_UserID;
    public InputField m_Password;
    public GameObject m_AlertLogin;

    private void Start()
    {
        // Init client objectg
        ClientObject c = ClientObject.Instance;
    }

    private void Update()
    {
        // check character
        if (m_UserID)
            m_UserID.text = Regex.Replace(m_UserID.text, @"[^a-zA-Z0-9]", "");
        if(m_Password)
        {
            string pass = m_Password.text;
            pass=pass.Replace("\'", "");
            pass=pass.Replace("<<", "<");
            pass=pass.Replace(">>", ">");
            pass=pass.Replace(" ", "");
            m_Password.text = pass;
        }
    }

    public void TryLogin()
    {
        StartCoroutine(LoginRoutine());
    }

    IEnumerator LoginRoutine()
    {
        string id = m_UserID.text;
        string pass = m_Password.text;
        if (id.Length == 0 || pass.Length == 0)
        {
            //AlertPanel.Instance.StartAlert(2, AlertType.LoginFail);
            AlertPanel.Instance.StartAlertOtherObj(m_AlertLogin, AlertType.LoginFail);
            yield break;
        }

        if(m_LoadingPanel)
            m_LoadingPanel.SetActive(true);
        Ws.Instance.Login(id,pass);
        
        TexasHoldemClient c = TexasHoldemClient.Instance;
        
        ClientObject.Instance.m_UserIdx = -1;
        RecvPacketObject obj = null;
        while (obj == null)
        {
            obj = c.PopPacketObject(Protocols.Login);
            yield return null;
        }
        
        int UserIndex = 0;
        ParserUserInfo.GetLoginResult(obj, ref UserIndex);

        bool success = false;
        ParserUserInfo.LoginResult r = ParserUserInfo.CheckLoginResult(UserIndex);
        
        switch (r)
        {
            case ParserUserInfo.LoginResult.Success:
                success = true;
                LoginSession.Instance.m_LoginID = id;
                LoginSession.Instance.m_LoginPass = pass;
                LoginSession.Instance.GetBankData();
                Debug.Log("로그인 성공");
                break;
            case ParserUserInfo.LoginResult.Fail:
                Debug.Log("로그인 실패");
                //AlertPanel.Instance.StartAlert(3, AlertType.LoginFail);
                AlertPanel.Instance.StartAlertOtherObj(m_AlertLogin, AlertType.LoginFail);
                break;
            case ParserUserInfo.LoginResult.Overlap:
                Debug.Log("중복접속");
                // AlertPanel.Instance.StartAlert(3, AlertType.LoginDuplication);
                AlertPanel.Instance.StartAlertOtherObj(m_AlertLogin, AlertType.LoginDuplication);
                break;
            case ParserUserInfo.LoginResult.Banned:
                Debug.Log("차단된유저");
                //AlertPanel.Instance.StartAlert(3, AlertType.LoginBanned);
                AlertPanel.Instance.StartAlertOtherObj(m_AlertLogin, AlertType.LoginBanned);
                break;
            default:
                break;
        }
        
        yield return new WaitForSeconds(1);
        if (success)
        {
            ClientObject.Instance.m_UserIdx = UserIndex;
            c.SendGetUserInfo(UserIndex);
            SceneManager.LoadSceneAsync("2_Lobby");
        }
        if (m_LoadingPanel)
            m_LoadingPanel.SetActive(false);
    }
    
    
}
