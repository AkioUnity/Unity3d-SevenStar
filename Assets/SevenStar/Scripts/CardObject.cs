using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardObject : MonoBehaviour
{
    public Image m_Img;
    public Image m_CoverImg;
    public CardShapeType m_Type;
    public int m_CardIndex=0;
    public Vector3 m_InitPos;

    public bool m_IsNotMoving = false;
    public Canvas m_Canvas;

    private void Awake()
    {
        if (m_Img == null)
            m_Img = GetComponent<Image>();
        m_InitPos = gameObject.transform.position;

        if (m_Canvas == null)
            m_Canvas = FindObjectOfType<Canvas>();
    }

    public void SetCardMovingStartPos()
    {
        if (m_IsNotMoving == true)
            return;
        // set init pos for moving card
        Vector3 pos = gameObject.transform.position;
        pos.x = m_Canvas.pixelRect.x + (m_Canvas.pixelRect.width / 2);
        pos.y += m_Canvas.pixelRect.height;
        gameObject.transform.position = pos;
    }

    public void SetCardSprite(CardShapeType type, int idx)
    {
        m_Type = type;
        m_CardIndex = idx;
        if (m_Img)
            m_Img.sprite = CardMgr.Instance.GetSpriteByShapeType(type, idx);
    }

    public void SetCardSprite()
    {
        if (m_Img)
            m_Img.sprite = CardMgr.Instance.GetSpriteByShapeType(m_Type, m_CardIndex);
    }

    public void SetCoverCard(bool isActive)
    {
        if (m_CoverImg == null)
            return;

        m_CoverImg.gameObject.SetActive(isActive);
    }
}
