using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BettingMoneyEff : MonoBehaviour
{
    public GameObject m_StartMoneyObj;
    public CanvasGroup m_SMCanvasGroup;
    public GameObject m_TargetMoneyObj;
    public GameObject m_PotMontyObj;

    private Vector3 m_TargetMoneyInitPos;

    private bool m_IsBettingEffPlaying = false;

    private void Start()
    {
        m_PotMontyObj = SevenStarLogic.Instance.m_SSPlayMgr.m_PotMoneyText.gameObject;
        m_TargetMoneyInitPos = m_TargetMoneyObj.transform.position;
        m_TargetMoneyObj.gameObject.SetActive(false);
    }
    
    private void Update()
    {
      /*  if(Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(BettingEff());
        }
        if(Input.GetKeyDown(KeyCode.W))
        {
            StartCoroutine(BettingEndEff());
        }*/
    }

    private void SetStartMoneyInitPos()
    {
        m_StartMoneyObj.transform.position = gameObject.transform.position;
        m_SMCanvasGroup.alpha = 1;
    }

    public IEnumerator BettingEff()
    {
        m_IsBettingEffPlaying = true;
        SoundMgr.Instance.PlaySoundFx(SoundFXType.BettingChip);

        m_StartMoneyObj.SetActive(true);
        Vector3 startPos = m_StartMoneyObj.transform.position;
        Vector3 targetPos = m_TargetMoneyObj.transform.position;
        float per = 0;
        while (per<1)
        {
            per += Time.deltaTime * 5;
            Vector3 pos = Vector3.Lerp(startPos, targetPos, per);
            m_StartMoneyObj.transform.position = pos;
            yield return null;
        }

        per = 0;
        while (per<1)
        {
            per += Time.deltaTime*5;
            m_SMCanvasGroup.alpha = 1 - per;
            yield return null;
        }

        m_StartMoneyObj.SetActive(false);
        m_TargetMoneyObj.SetActive(true);
        SetStartMoneyInitPos();
        m_IsBettingEffPlaying = false;
    }

    public IEnumerator BettingEndEff()
    {
        // wait betting eff
        while (m_IsBettingEffPlaying == true)
            yield return null;

        Vector3 targetPos = m_PotMontyObj.transform.position;
        float per = 0;
        while (per <1)
        {
            per += Time.deltaTime*4;
            Vector3 pos = Vector3.Lerp(m_TargetMoneyInitPos, targetPos, per);
            m_TargetMoneyObj.transform.position = pos;
            yield return null;
        }

        m_TargetMoneyObj.SetActive(false);
        m_TargetMoneyObj.transform.position = m_TargetMoneyInitPos;
    }
}
