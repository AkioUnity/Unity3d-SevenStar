using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanel : MonoBehaviour
{
    public Text m_WinnerNameText;
    public Text m_RankOfCardText;
    public CardObject[] m_CardObjs;

    public void SetResultInfo(string winnerName, string rankOfCard)
    {
        if (m_WinnerNameText)
            m_WinnerNameText.text = winnerName;
        if (m_RankOfCardText)
            m_RankOfCardText.text = rankOfCard;
    }

    public void SetResultPanelCard(int objidx, CardShapeType type, int cardIdx)
    {
        if (objidx < 0 || objidx > (m_CardObjs.Length - 1))
            return;
        m_CardObjs[objidx].SetCardSprite(type,cardIdx);
    }
}
