using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBoxItem : MonoBehaviour
{
    public MessageBox m_MessageBox;
    public int m_MessageItemIdx = -1;
    public Text m_DataText;
    public Text m_MessageText;
    public int type = 0;
    public bool m_IsGift= true;

    public void OnClick_MessageBoxRead()
    {
        if (m_MessageItemIdx == -1)
            return;
        m_MessageBox.m_SeletedMessageIdx = m_MessageItemIdx;

        if (m_IsGift)
        {
            m_MessageBox.m_MessageBoxRecvGiftPanel.GetComponent<MessageBoxRecvPanel>().SetMessageRecvPanelText(m_MessageText.text, m_DataText.text);
            m_MessageBox.SetMessageBoxPanel(1);
        }
        else
        {
            m_MessageBox.m_MessageBoxRecvNotiPanel.GetComponent<MessageBoxRecvPanel>().SetMessageRecvPanelText(m_MessageText.text, m_DataText.text);
            m_MessageBox.SetMessageBoxPanel(2);
        }
    }

}
