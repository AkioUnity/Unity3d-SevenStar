using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvataItem : MonoBehaviour
{
    public Text m_AvataPriceText;
    public int m_AvataIdx;
    public int m_AvataPrice;

    private void Start()
    {
        if (m_AvataPriceText)
            m_AvataPriceText.text = m_AvataPrice.ToString();
    }

    public void OnClickToggle()
    {
        //if (m_AvataPriceText)
        //    m_AvataPriceText.text = m_AvataPrice.ToString();
    }
}
