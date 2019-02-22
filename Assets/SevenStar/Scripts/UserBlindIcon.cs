using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserBlindIcon : MonoBehaviour
{
    public GameObject m_DL;
    public GameObject m_SB;
    public GameObject m_BB;

    public void IconInit()
    {
        m_DL.SetActive(false);
        m_SB.SetActive(false);
        m_BB.SetActive(false);
    }

    public void UnactiveBlindIcon()
    {
        m_SB.SetActive(false);
        m_BB.SetActive(false);
    }

    public void ActiveBlindIcon()
    {
        m_SB.SetActive(true);
        m_BB.SetActive(true);
    }

    /// <summary>
    /// type 0 : DL, 1: SB, 2: BB
    /// </summary>
    /// <param name="type"></param>
    public void SetActiveBlindIcon(int type, bool isActive)
    {
        IconInit();
        switch (type)
        {
            case 0:
                m_DL.SetActive(isActive);
                break;
            case 1:
                m_SB.SetActive(isActive);
                break;
            case 2:
                m_BB.SetActive(isActive);
                break;
        }
    }

}
