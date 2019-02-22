using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BettingTimer : MonoBehaviour
{
    public Image m_TimerImage;
    public float m_Time;
    public float m_TimeLimit;

    private void Start()
    {
        if (m_TimerImage == null)
            m_TimerImage = GetComponent<Image>();
    }

    private void Update()
    {
        if(m_Time>0)
        {
            m_Time -= Time.deltaTime;
            TimerPer();
        }
    }

    private void TimerPer()
    {
        float nowPer = m_Time / m_TimeLimit;
        if (m_TimerImage)
        {
            m_TimerImage.fillAmount = nowPer;
        }
        if( nowPer < 0.5f && SevenStarLogic.Instance.m_IsMyTurn)
            SoundMgr.Instance.m_IsAlert = true;
    }

    public void StartTimer()
    {
        SoundMgr.Instance.m_IsAlert = false;
        if (m_TimerImage)
            m_TimerImage.gameObject.SetActive(true);
        m_Time = m_TimeLimit+1;
    }

    public void SetTimerPer(float per)
    {
        if (m_TimerImage)
            m_TimerImage.fillAmount = per;
        if (per == 0)
            m_Time = 0;
    }

    public void SetTimerImage(Sprite spr)
    {
        if (m_TimerImage)
            m_TimerImage.sprite = spr;
    }

}
