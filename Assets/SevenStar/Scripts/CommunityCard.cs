using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CommunityCard : MonoBehaviour
{
    public Canvas m_Canvas;
    public CardObject[] m_ComCards;
    public CardObject[] m_ComAdminCards;
    public GameObject m_ComAdminCardPanel;
    public int m_OpenCount;

    public void Init()
    {
        m_OpenCount = 0;
        for (int i = 0; i < m_ComCards.Length; i++)
        {
            m_ComCards[i].SetCardSprite(CardShapeType.Back, -1);
            m_ComAdminCards[i].SetCardSprite(CardShapeType.Back, -1);
            m_ComCards[i].SetCardMovingStartPos();
        }
        SetActiveComAdminCard(false);
    }

    public IEnumerator CardMoveRoutine(int moveCardIdx)
    {
        if (moveCardIdx < 0 || moveCardIdx > (m_ComCards.Length - 1))
            yield break;
        SoundMgr.Instance.PlaySoundFx(SoundFXType.DealCard);

        Vector3 nowPos = m_ComCards[moveCardIdx].gameObject.transform.position;
        float per = 0;
        while (per < 1)
        {
            per += Time.deltaTime*5;
            Vector3 pos = Vector3.Lerp(nowPos, m_ComCards[moveCardIdx].m_InitPos, per);
            m_ComCards[moveCardIdx].gameObject.transform.position = pos;
            yield return null;
        }
    }

    public void SetComCardToInitPos(int cardIdx) // 중간난입시 바닥패용;
    {
        if (cardIdx < 0 || cardIdx > (m_ComCards.Length - 1))
            return;
        m_ComCards[cardIdx].transform.position = m_ComCards[cardIdx].m_InitPos;
    }

    public void SetComCardInfo(int num, CardShapeType type ,int cardIdx)
    {
        if (num < 0 || num > (m_ComCards.Length - 1))
            return;
        m_ComCards[num].m_CardIndex = cardIdx;
        m_ComCards[num].m_Type = type;
    }
    
    public void OpenComCard(int num)
    {
        if (num < 0 || num > (m_ComCards.Length - 1))
            return;
        SoundMgr.Instance.PlaySoundFx(SoundFXType.CardOpen);
        m_ComCards[num].SetCardSprite();
        m_OpenCount++;
    }

    public void SetComAdminCardInfo(int num, CardShapeType type, int cardIdx)
    {
        if (num < 0 || num > (m_ComCards.Length - 1))
            return;
        m_ComAdminCards[num].m_CardIndex = cardIdx;
        m_ComAdminCards[num].m_Type = type;
    }

    public void SetActiveComAdminCard(bool isActive)
    {
        if(m_ComAdminCardPanel)
            m_ComAdminCardPanel.SetActive(isActive);
    }
}
