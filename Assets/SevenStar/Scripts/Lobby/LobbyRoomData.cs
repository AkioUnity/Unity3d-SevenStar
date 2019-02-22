using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyRoomData : MonoBehaviour
{
    public string m_RoomName;
    public string m_RoomHost;
    public int m_RoomNum;
    public int m_RoomIdx;
    public int m_TotalPlayer;
    public int m_NowPlayer;
    public int m_BlindType;
    public int m_AvataIdx;
    //public User m_HostUser;

    public Text m_RoomNameText;
    public Text m_RoomNumText;
    public Text m_HostNameText;
    public Text m_MemberText;
    public Text m_BlindText;
    public Image m_Avata;


    public void OnClick_Room()
    {
        Debug.Log("Clicked Room Num " + m_RoomNum + " / " + m_RoomIdx);
        LobbyLogic.Instance.RoomIn(m_RoomIdx,m_BlindType);
    }

    public void Set(string name, int num, int idx, string reader,int readerAvata, int member, int blindType)
    {
        m_RoomName = name;
        m_RoomNum = num;
        m_RoomIdx = idx;
        m_TotalPlayer = 9;
        m_RoomHost = reader;
        m_NowPlayer = member;
        m_BlindType = blindType;
        m_AvataIdx = readerAvata;
        m_Avata.sprite = AvataMgr.Instance.GetAvataSprite(readerAvata);
        SetText();
    }

    public void SetText()
    {
        m_RoomNameText.text = m_RoomName;
        m_HostNameText.text = m_RoomHost;
        m_MemberText.text = m_NowPlayer + " / " + m_TotalPlayer;
        switch (m_BlindType)
        {
            case 0:
                m_BlindText.text = "???";
                break;
            case 1:
                //m_BlindText.text = "1k/2k";
                m_BlindText.text = "20¢";
                break;
            case 2:
                //m_BlindText.text = "2k/3k";
                m_BlindText.text = "50¢";
                break;
            case 3:
                //m_BlindText.text = "1k/2k";
                m_BlindText.text = "1$";
                break;
            case 4:
                //m_BlindText.text = "2k/3k";
                m_BlindText.text = "2$";
                break;
        }
    }
}
