using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProcDot : MonoBehaviour
{
    [Header("Signup Process Panel")]
    public GameObject[] m_SignupProcPanel;
    public Button m_NextBtn;
    public Button m_CreateAccountBtn;
    public Toggle m_SelectAllToggle;
    public Toggle m_Toggle1;
    public Toggle m_Toggle2;
    public Toggle m_Toggle3;

    [Header("Dot")]
    public Sprite m_BlackDot;
    public Sprite m_WhiteDot;
    public Image[] m_DotImage;

    public void Init()
    {
        m_NextBtn.gameObject.SetActive(true);
        m_CreateAccountBtn.gameObject.SetActive(false);
    }

    public void SetSignupProcPanel(int idx)
    {
        if (idx < 0 || idx >= m_SignupProcPanel.Length)
            return;
        if(idx == m_SignupProcPanel.Length-1)
        {
            m_NextBtn.gameObject.SetActive(false);
            m_CreateAccountBtn.gameObject.SetActive(true);
        }

        for (int i = 0; i < m_SignupProcPanel.Length; i++)
        {
            m_SignupProcPanel[i].SetActive(false);
        }
        m_SignupProcPanel[idx].SetActive(true);

        for (int i = 0; i < m_DotImage.Length; i++)
        {
            m_DotImage[i].sprite = m_BlackDot;
        }
        m_DotImage[idx].sprite = m_WhiteDot;
    }


    public void OnClick_SelectAllToggle()
    {
        m_Toggle1.isOn = m_SelectAllToggle.isOn;
        m_Toggle2.isOn = m_SelectAllToggle.isOn;
        m_Toggle3.isOn = m_SelectAllToggle.isOn;
    }

    public bool CheckToggles()
    {
        if(m_Toggle1.isOn && m_Toggle2.isOn && m_Toggle3.isOn)
            return true;
        return false;
    }

    private void Update()
    {
        if (m_CreateAccountBtn == null)
            return;
        m_CreateAccountBtn.interactable = CheckToggles();
    }

}
