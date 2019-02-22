using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityUtil;


public enum LobbyPanelType
{
    Lobby = 0,
    ChargeRequest,
    WithDrawal,
    MyPoint,
    Profile,
    CreateRoom,
    Notification
}

public class LobbyPanels : UtilHalfSingleton<LobbyPanels>
{
    public Image m_HeaderImage;
    public Sprite m_CreatroomHeaderSprite;
    public Sprite m_DefaultHeaderSprite;
    public LobbyBottomBtnAction m_BottomBtn;
    public GameObject[] m_LobbyPanels;

    public void SwitchLobbyPanel(LobbyPanelType type)
    {
        for (int i = 0; i < m_LobbyPanels.Length; i++)
        {
            if (i == (int)type)
            {
                m_LobbyPanels[i].SetActive(true);
                if(type != LobbyPanelType.Profile)
                    SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
            }
            else
                m_LobbyPanels[i].SetActive(false);
        }
    }

    public void SelectBottomBtns(LobbyPanelType type)
    {
        m_BottomBtn.SelectLobbyBottomBtn((int)type);
    }


}
