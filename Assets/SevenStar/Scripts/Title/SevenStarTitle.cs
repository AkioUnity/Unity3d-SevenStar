using System;
using System.Collections;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityUtil;

public class SevenStarTitle : UtilHalfSingleton<SevenStarTitle>
{

    public GameObject m_LoginPanel;
    public GameObject m_SignUpPanel;
    public LoginProc m_LoginProc;
    public SignUpProc m_SignUpProc;

    private bool m_IsPopupAni = false;


    private IEnumerator Start()
    {
        Debug.Log("SevenStarTitle Start");
     
        // Set sleep time out
//        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        // Connectiion work
//        if(ClientObject.Instance.m_Client.IsConnect == false)
//        {
//            yield return new WaitForSeconds(0.5f);
//            Debug.Log("Connect Work");
//            ClientObject.Instance.m_Client.Connect("211.238.13.182");
//            //ClientObject.Instance.m_Client.Connect("118.45.180.254");
//            //ClientObject.Instance.m_Client.Connect("127.0.0.1");
//            //ClientObject.Instance.m_Client.Connect("192.168.0.6");
//        }
        
        yield return new WaitForSeconds(0.5f);
    }
    
    public void OnClick_ExitGame()
    {
//        Application.Quit();
    }

    public void OnClick_LoginOK()
    {
        if (m_LoginProc)
            m_LoginProc.TryLogin();
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
    }

    public void OnClick_SignUpOK()
    {
        if (m_SignUpProc)
            m_SignUpProc.TryRegister();
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
    }

    public void OnClick_LoginCancel()
    {
        if (m_LoginPanel == null)
            return;
        m_LoginPanel.SetActive(false);
        m_IsPopupAni = false;

        InputField[] childrenInput = m_LoginPanel.transform.GetComponentsInChildren<InputField>();
        for (int i = 0; i < childrenInput.Length; i++)
            childrenInput[i].text = "";
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
    }

    public void OnClick_SignUpCancel()
    {
        if (m_SignUpPanel == null)
            return;
        m_SignUpPanel.SetActive(false);
        m_IsPopupAni = false;

        InputField[] childrenInput = m_SignUpPanel.transform.GetComponentsInChildren<InputField>();
        for (int i = 0; i < childrenInput.Length; i++)
            childrenInput[i].text = "";
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
    }


    public void OnClick_Login()
    {
        if (m_LoginPanel == null)
            return;
        ActivePopupWorkNotAni(m_LoginPanel);
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
    }

    public void OnClick_SignUp()
    {
        if (m_SignUpPanel == null)
            return;
        m_SignUpProc.InitSignupPanel();
        ActivePopupWorkNotAni(m_SignUpPanel);
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
    }

    private void ActivePopupWorkNotAni(GameObject obj)
    {
        obj.SetActive(true);
    }

    private void ActivePopupWork(GameObject obj)
    {
        obj.transform.localScale = Vector3.zero;
        obj.SetActive(true);
        StartCoroutine(PopupWork(obj));
    }

    private IEnumerator PopupWork(GameObject obj)
    {
        m_IsPopupAni = true;

        Vector3 scale = obj.transform.localScale;
        float per = 0;
        while (per < 1 && m_IsPopupAni == true)
        {
            per += Time.deltaTime;
            scale = Vector3.Lerp(scale, Vector3.one, per);
            obj.transform.localScale = scale;
            yield return null;
        }

        m_IsPopupAni = false;
    }
}
