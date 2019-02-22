using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RoomPanel : MonoBehaviour
{
    public RectTransform m_RectTransform;
    public GridLayoutGroup m_GridLayout;
    public Rect m_ContainerRect;

    public Vector2 m_RoomPanelSize;
    public Vector2 m_CellCount;
    private float m_CellWidth;
    private float m_CellHeight;

    private void Start()
    {
        if (m_RectTransform == null)
            m_RectTransform=GetComponent<RectTransform>();
        SetPanelSize();
        SetGridSizeByRT();
    }

    private void SetPanelSize()
    {
        m_RectTransform.sizeDelta = m_ContainerRect.size;
    }

    private void SetGridSizeByRT()
    {
        m_RoomPanelSize = m_RectTransform.rect.size;
        //m_CellWidth = (m_RoomPanelSize.x / m_CellCount.x) - m_GridLayout.spacing.x;
        //m_CellHeight = (m_RoomPanelSize.y / m_CellCount.y) - m_GridLayout.spacing.y;
        m_CellWidth = (m_RoomPanelSize.x / m_CellCount.x);
        m_CellHeight = (m_RoomPanelSize.y / m_CellCount.y);
        Vector2 spacing = new Vector2(m_CellWidth * 0.1f,m_CellHeight*0.07f);
        m_GridLayout.spacing = spacing;
        m_CellWidth -= m_GridLayout.spacing.x;
        m_CellHeight -= m_GridLayout.spacing.y;

        if (LobbyLogic.Instance.m_IsOldViewRoom6R == false) // if room view 1 -- set cell size (3:2)
            m_CellWidth = (m_CellHeight / 2) * 3;

        m_GridLayout.cellSize = new Vector2(m_CellWidth, m_CellHeight);
    }


}
