using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBoxRecvPanel : MonoBehaviour
{
    public Text m_MsgText;
    public Text m_DateText;

    public void SetMessageRecvPanelText(string msg, string date)
    {
        if (m_MsgText == null)
            return;
        m_MsgText.text = msg;
        if (m_DateText == null)
            return;
        m_DateText.text = date;
    }

}
