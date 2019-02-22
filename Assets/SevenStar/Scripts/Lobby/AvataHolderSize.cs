using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvataHolderSize : MonoBehaviour
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
            m_RectTransform = GetComponent<RectTransform>();
        SetPanelSize();
        SetGridSizeByRT();
    }

    private void SetPanelSize()
    {
        m_ContainerRect = transform.parent.GetComponent<RectTransform>().rect;
        int rowCount = (int)(transform.childCount / m_CellCount.x) + 1;
        float unitHeight = m_ContainerRect.height / 2;
        Vector2 size = m_RectTransform.sizeDelta;
        size.y = unitHeight * rowCount;
        m_RectTransform.sizeDelta = size;
    }

    private void SetGridSizeByRT()
    {
        m_RoomPanelSize = m_ContainerRect.size;
        m_CellWidth = (m_RoomPanelSize.x / m_CellCount.x) - m_GridLayout.spacing.x;
        m_CellHeight = (m_RoomPanelSize.y / m_CellCount.y) - m_GridLayout.spacing.y;
        m_GridLayout.cellSize = new Vector2(m_CellWidth, m_CellHeight);
    }
}
