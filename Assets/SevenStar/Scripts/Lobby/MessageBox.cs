using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    public GameObject m_MessageBoxGiftPrefab;
    public GameObject m_MessgaeBoxNotiPrefab;
    public List<MessageData> m_MessageBoxList;
    public int m_SeletedMessageIdx = -1;
    public GameObject m_MessageBoxMainPanel;
    public GameObject m_MessageBoxRecvGiftPanel;
    public GameObject m_MessageBoxRecvNotiPanel;

    [Header("Resizing")]
    public GameObject m_ContentHolder;
    public RectTransform m_RectTransform;
    public GridLayoutGroup m_GridLayout;
    private Rect m_ContainerRect;

    public Vector2 m_ContainerSize;
    public Vector2 m_CellCount;
    private float m_CellWidth;
    private float m_CellHeight;

    public string m_ClickedGiftMsg;
    public string m_ClickedGiftDate;

    public void OnClick_MessageRecvOK()
    {
        if (m_SeletedMessageIdx == -1)
            return;
        TexasHoldemClient.Instance.SendMessageRead(m_SeletedMessageIdx);

        // reset user info
        TexasHoldemClient c = TexasHoldemClient.Instance;
        c.SendGetUserInfo(LobbyLogic.Instance.m_UserInfo.m_UserIndex);
        c.SendGetBankMoney();
        // refresh message box
        LobbyLogic.Instance.RefreshMessageBox();
        SetMessageBoxPanel(0);
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
    }

    public void OnClick_MessageRecvClose()
    {
        SetMessageBoxPanel(0);
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
    }

    public void SetMessageBoxPanel(int type)
    {
        m_MessageBoxMainPanel.SetActive(false);
        m_MessageBoxRecvGiftPanel.SetActive(false);
        m_MessageBoxRecvNotiPanel.SetActive(false);

        switch (type)
        {
            case 0:
                m_MessageBoxMainPanel.SetActive(true);
                break;
            case 1:
                m_MessageBoxRecvGiftPanel.SetActive(true);
                break;
            case 2:
                m_MessageBoxRecvNotiPanel.SetActive(true);
                break;
        }
    }

    private void Start()
    {
        m_MessageBoxList = new List<MessageData>();
        if (m_RectTransform == null)
            m_RectTransform = GetComponent<RectTransform>();
        SetPanelSize();
        SetGridSizeByRT();
    }

    private void Update()
    {
        SetPanelSize();
        SetGridSizeByRT();
    }

    public void RefreshMessageItems(List<MessageData> msgList)
    {
        //reverse msglist -- for lastest msg
        msgList.Reverse();

        List<int> nowChildIdxList = new List<int>();
        int childCount = m_ContentHolder.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            int idx = m_ContentHolder.transform.GetChild(i).GetComponent<MessageBoxItem>().m_MessageItemIdx;
            nowChildIdxList.Add(idx);
        }
        // check and destroy
        if(childCount > msgList.Count)
        {
            for (int i = 0; i < childCount; i++)
            {
                bool isExist = false;
                for (int j = 0; j < msgList.Count; j++)
                {
                    if (msgList[j].MessageIdx == nowChildIdxList[i])
                    {
                        isExist = true;
                        break;
                    }
                }
                if(isExist == false)
                    Destroy(m_ContentHolder.transform.GetChild(i).gameObject);
            }
        }
        else if(childCount < msgList.Count)
        {
            //check and add
            for (int i = 0; i < msgList.Count; i++)
            {
                bool isExist = false;
                for (int j = 0; j < nowChildIdxList.Count; j++)
                {
                    if (msgList[i].MessageIdx == nowChildIdxList[j])
                    {
                        isExist = true;
                        break;
                    }
                }
                if (isExist == false)
                    AddMessageItem(msgList[i]);
            }
        }
        
    }

    private void AddMessageItem(MessageData data)
    {
        GameObject msgPrefab = null;
        if (data.Type == 0) // default
        {
            if (data.Money <= 0)
                msgPrefab = m_MessgaeBoxNotiPrefab;
            else
                msgPrefab = m_MessageBoxGiftPrefab;
        }
        else // charge
            msgPrefab = m_MessgaeBoxNotiPrefab;

        if (msgPrefab == null)
            return;
        GameObject obj = Instantiate(msgPrefab);
        obj.transform.SetParent(m_ContentHolder.transform);
        obj.transform.localScale = Vector3.one;

        MessageBoxItem item = obj.GetComponent<MessageBoxItem>();
        item.m_MessageBox = this;
        item.m_MessageItemIdx = data.MessageIdx;
        string year = data.DateTime.Substring(0, 4);
        string month = data.DateTime.Substring(4, 2);
        string day = data.DateTime.Substring(6, 2);
        string hour = data.DateTime.Substring(8, 2);
        string min = data.DateTime.Substring(10, 2);
        string sec = data.DateTime.Substring(12, 2);
        string date = year + "-" + month + "-" + day + " " + hour + ":" + min + ":"+sec;
        //item.m_DataText.text = "Date : " + date + " / Point : " + data.Money;
        //item.m_DataText.text = date + "\nPoint : " + data.Money;

        item.type = data.Type;
        if (data.Type == 0)
        {
            if (data.Money <= 0)
            {
                item.m_DataText.text = date;
                item.m_IsGift = false;
            }
            else
            {
                item.m_DataText.text = date + "\nPoint : " + TransformMoney.GetDollarMoney(data.Money);
                item.m_IsGift = true;
            }
        }
        else
        {
            item.m_IsGift = false;
            item.m_DataText.text = date;
        }

        item.m_MessageText.text = data.Message;
    }

    private void SetPanelSize()
    {
        m_ContainerRect = m_ContentHolder.transform.parent.GetComponent<RectTransform>().rect;
        int rowCount = (int)(m_ContentHolder.transform.childCount / m_CellCount.x);
        float unitHeight = m_ContainerRect.height / m_CellCount.y;
        Vector2 size = m_RectTransform.sizeDelta;
        size.y = unitHeight * rowCount;
        m_RectTransform.sizeDelta = size;
    }

    private void SetGridSizeByRT()
    {
        m_ContainerSize = m_ContainerRect.size;
        m_CellWidth = (m_ContainerSize.x / m_CellCount.x) - m_GridLayout.spacing.x;
        m_CellHeight = (m_ContainerSize.y / m_CellCount.y) - m_GridLayout.spacing.y;
        m_GridLayout.cellSize = new Vector2(m_CellWidth, m_CellHeight);
    }




}
