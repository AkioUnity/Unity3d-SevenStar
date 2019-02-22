using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxAni : MonoBehaviour
{
    public Animator m_Anim;
    public Text m_EventNameText;
    public Text m_MoneyText;

    public AudioSource m_Audio;
    public AudioClip m_PlaneSoundFX1;
    public AudioClip m_PlaneSoundFX2;
    public AudioClip m_PlaneSoundFX3;
    public AudioClip m_PlaneSoundFX4;

    public void PlayBoxAni()
    {
        if (m_Anim == null)
            return;
        m_Anim.SetTrigger("BoxAni");
    }

    public void InitText()
    {
        SetEventNameText("");
        SetMoneyText("");
    }

    public void SetEventNameText(string text)
    {
        if (m_EventNameText)
            m_EventNameText.text = text;
    }

    public void SetMoneyText(string text)
    {
        if (m_MoneyText)
            m_MoneyText.text = text;
    }

    public void PlayAniSoundFX_First()
    {
        if (m_Audio == null)
            return;
        m_Audio.PlayOneShot(m_PlaneSoundFX1);
    }

    public void PlayAniSoundFX_Last()
    {
        if (m_Audio == null)
            return;
        m_Audio.PlayOneShot(m_PlaneSoundFX2);
        m_Audio.PlayOneShot(m_PlaneSoundFX3);
        m_Audio.PlayOneShot(m_PlaneSoundFX4);

    }

}
