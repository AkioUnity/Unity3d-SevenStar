using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyBottomBtnAction : MonoBehaviour
{
    public GameObject[] m_LobbyBottomSelectedBtns;
    public GameObject[] m_LobbyBottomCoverBtns;


    public void SelectLobbyBottomBtn(int type)
    {
        for (int i = 0; i < m_LobbyBottomCoverBtns.Length; i++)
        {
            m_LobbyBottomCoverBtns[i].SetActive(true);
            m_LobbyBottomSelectedBtns[i].SetActive(false);
        }

        int idx = (int)type;
        if (idx < m_LobbyBottomSelectedBtns.Length)
        {
            m_LobbyBottomSelectedBtns[idx].SetActive(true);
            m_LobbyBottomCoverBtns[idx].SetActive(false);
        }

        LobbyPanels.Instance.SwitchLobbyPanel((LobbyPanelType)type);
    }

}

