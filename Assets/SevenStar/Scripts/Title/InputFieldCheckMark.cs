using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldCheckMark : MonoBehaviour
{
    public Image m_Img;
    public Sprite m_CheckSprite;
    public Sprite m_XSprite;

    private void Start()
    {
        if (m_Img == null)
            m_Img = GetComponent<Image>();
    }

    public void SetCheckMark(bool isChecked)
    {
        if (m_Img == null)
            return;
        m_Img.enabled = true;

        if(isChecked)
            m_Img.sprite = m_CheckSprite;
        else
            m_Img.sprite = m_XSprite;
    }

    public void SetCheckMark(bool isChecked, bool isActive)
    {
        if (m_Img == null)
            return;
        m_Img.enabled = isActive;

        if (isChecked)
            m_Img.sprite = m_CheckSprite;
        else
            m_Img.sprite = m_XSprite;
    }
}
