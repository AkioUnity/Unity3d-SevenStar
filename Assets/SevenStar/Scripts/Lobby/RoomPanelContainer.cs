using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanelContainer : MonoBehaviour
{
    public GameObject m_RoomPanelPrefab_6R;
    public GameObject m_RoomPanelPrefab_1R;
    public GameObject m_UseRoomPrefab;
    public ScrollSnapRect m_RoomScrollSnap;
    public List<GameObject> m_RoomPanelList;

    public Rect m_InitRect;
    public int m_PanelRoomCountLimit;
    public int m_RoomCount;
    public int m_PanelCount;

    private void Awake()
    {
        //m_InitRect = m_RoomScrollSnap.GetComponent<RectTransform>().rect;
        m_InitRect = GetComponent<RectTransform>().rect;
        m_RoomPanelList = new List<GameObject>();
        SetRoomView(true);
    }

    public void SetRoomView(bool isRoomSix)
    {
        if(isRoomSix)
        {
            m_UseRoomPrefab = m_RoomPanelPrefab_6R;
            m_PanelRoomCountLimit = 6;
        }
        else
        {
            m_UseRoomPrefab = m_RoomPanelPrefab_1R;
            m_PanelRoomCountLimit = 1;
        }
        m_RoomPanelList.Clear();
    }

    public void AddRoom(GameObject roomObj)
    {
        List<LobbyRoomData> roomList = LobbyLogic.Instance.m_RoomList;
        m_RoomCount = roomList.Count;

        int panelIdx = GetRoomPanelIdx();
        int panelCount = m_RoomPanelList.Count;
        if (panelCount == 0 || (panelCount-1) < panelIdx)
            AddRoomPanel();

        roomObj.transform.SetParent(m_RoomPanelList[panelIdx].transform);
        roomObj.transform.localScale = Vector3.one;
    }

    public IEnumerator CheckRoomPanelContainer()
    {
        List<LobbyRoomData> roomList = LobbyLogic.Instance.m_RoomList;
        m_RoomCount = roomList.Count;

        for (int i = 0; i < m_RoomPanelList.Count; i++) // sorting room
        {
            int childCount = m_RoomPanelList[i].transform.childCount;
            if (i != (m_RoomPanelList.Count-1))
            {
                if (childCount < m_PanelRoomCountLimit) 
                {
                    int moveCount = m_PanelRoomCountLimit - childCount;
                    for (int j = 0; j < moveCount; j++)
                    {
                        if(m_RoomPanelList[i+1].transform.childCount != 0)
                            m_RoomPanelList[i + 1].transform.GetChild(j).transform.SetParent(m_RoomPanelList[i].transform);
                    }
                }
            }
        }

        // remove panel have no child
        for (int i = 0; i < m_RoomPanelList.Count; i++)
        {
            int childCount = m_RoomPanelList[i].transform.childCount;
            if (childCount == 0) // remove panel
            {
                Destroy(m_RoomPanelList[i]);
                m_RoomPanelList.RemoveAt(i);
            }
        }

        yield return new WaitForEndOfFrame();
        // scroll snap init
        m_RoomScrollSnap.Init();
    }

    public int GetRoomPanelIdx()
    {
        return (m_RoomCount-1)/m_PanelRoomCountLimit;
    }

    private void AddRoomPanel()
    {
        GameObject roomPanel = Instantiate(m_UseRoomPrefab);
        roomPanel.GetComponent<RoomPanel>().m_ContainerRect = m_InitRect;
        roomPanel.transform.SetParent(this.transform);

        m_RoomScrollSnap.Init();
        // Add room panel list
        m_RoomPanelList.Add(roomPanel);
    }

    /*private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            GameObject roomPanel = Instantiate(m_UseRoomPrefab);
            roomPanel.GetComponent<RoomPanel>().m_ContainerRect = m_InitRect;
            roomPanel.transform.SetParent(this.transform);

            m_RoomScrollSnap.Init();
        }
    }*/


}
