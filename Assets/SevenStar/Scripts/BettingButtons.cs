using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BettingButtons : MonoBehaviour
{
    public Button m_FoldBtn;
    public Button m_CheckBtn;
    public Button m_CallBtn;
    public Button m_BetBtn;
    public Button m_ConfirmBtn;
    public Text m_CallBtn_CallMoney;

    public bool m_IsCheckAvailable = false;


    public void CheckBtnInteractable(UInt64 callMoney, UInt64 nowBettingMoney)
    {
        if (callMoney == nowBettingMoney)
            m_IsCheckAvailable = true;
        else
            m_IsCheckAvailable = false;
    }

    public void SetBtnInteractable(bool isActive)
    {
        if(m_FoldBtn)
            m_FoldBtn.interactable = isActive;
        if (m_BetBtn)
            m_BetBtn.interactable = isActive;
        if (m_CheckBtn && m_IsCheckAvailable)
        {
            m_CheckBtn.interactable = isActive;
            if (m_CallBtn)
                m_CallBtn.interactable = false;
        }
        if (m_CallBtn && !m_IsCheckAvailable)
        {
            m_CallBtn.interactable = isActive;
            if(m_CheckBtn)
                m_CheckBtn.interactable = false;
        }
    }


}
