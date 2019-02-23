﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityUtil;

public class LobbyLogic_back : UtilHalfSingleton<LobbyLogic_back>
{
    public TexasHoldemClient m_Client = null;
    
    // room setting
    [Header("Room Setting")]
    public GameObject m_RoomPrefab;
    public RoomPanelContainer m_RoomScrollContainer;
    public List<LobbyRoomData> m_RoomList=new List<LobbyRoomData>();
    public GameObject m_LoadingPanel;
    public bool m_IsRoomCreating = false;

    // room page
    [Header("Room Page Setting")]
    public BlindValueType m_ShowBlindType;
    public int m_RoomPageShowCount;
    public int m_RoomNowPage;
    public int m_RoomPageCount;
    public bool m_IsOldViewRoom6R;
    public Toggle m_ViewRoomToggle_6R;
    public Toggle m_ViewRoomToggle_1R;
    public Toggle m_ViewBetToggle_200;
    public Toggle m_ViewBetToggle_500;
    public Toggle m_CreateRoomBetToggle_200;
    public Toggle m_CreateRoomBetToggle_500;
    public bool m_IsViewRoom6ROnly = false;

    // my info
    [Header("User Info")]
    public UserInfoSet m_UserInfo;

    [Header("Room Refresh Time")]
    public float m_RefreshTimeLimit = 3;
    private float m_RefreshTime=0;
    private bool m_RefeshWorking = false;

    [Header("Messege Box")]
    public GameObject m_MessagePanel;
    public MessageBox m_MessageBox;
    public GameObject m_MessageBoxBadge;
    public int m_MessageCount = 0;

    [Header("ScreenShot")]
    public Image m_ScreenShotFilter;

    private void OnEnable()
    {
        m_Client = ClientObject.Instance.m_Client;
    }

    private void OnDisable()
    {
    }

    private void Start()
    { 
        m_Client = ClientObject.Instance.m_Client;
        m_UserInfo.UserIndex = ClientObject.Instance.m_UserIdx;

        m_IsOldViewRoom6R = true;
        if (m_RefeshWorking == false)
            StartCoroutine(RefreshRoom());
    }

    public void OnClick_ScreenShot()
    {
        // screen shot
        StartCoroutine(TakeScreenShot());
        SoundMgr.Instance.PlaySoundFx(SoundFXType.Capture);
    }

    private IEnumerator FlashWork()
    {
        if (m_ScreenShotFilter == null)
            yield break;
        m_ScreenShotFilter.color = Color.white;
        Color col = m_ScreenShotFilter.color;
        float per = 0;
        while (per < 1)
        {
            per += Time.deltaTime*4;
            col.a = 1 - per;
            m_ScreenShotFilter.color = col;
            yield return null;
        }
    }

    private IEnumerator TakeScreenShot()
    {
        yield return FlashWork();
        yield return new WaitForEndOfFrame();

        string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
        string defaultLocation = Application.persistentDataPath + "/" + fileName;
        ScreenCapture.CaptureScreenshot(fileName);

        string folderLocation = "/storage/emulated/0/DCIM/SevenStar/";
        string screenShotLocation = folderLocation + fileName;
        if (!Directory.Exists(folderLocation))
        {
            Directory.CreateDirectory(folderLocation);
        }

        while (!File.Exists(defaultLocation))
            yield return null;
        //Debug.Log("file exist!");
        File.Move(defaultLocation, screenShotLocation);

        //REFRESHING THE ANDROID PHONE PHOTO GALLERY
        AndroidJavaClass classPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject objActivity = classPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass classUri = new AndroidJavaClass("android.net.Uri");
        AndroidJavaObject objIntent = new AndroidJavaObject("android.content.Intent", new object[2] { "android.intent.action.MEDIA_SCANNER_SCAN_FILE", classUri.CallStatic<AndroidJavaObject>("parse", "file://" + screenShotLocation) });
        objActivity.Call("sendBroadcast", objIntent);
    }

    public void OnClick_MessageBox()
    {
        if (m_MessagePanel == null || m_MessagePanel.activeSelf == true)
            return;
        ActivePopupWork(m_MessagePanel);

        // refresh messagebox        
        m_MessageBox.m_SeletedMessageIdx = -1;
        RefreshMessageBox();
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
    }

    public void OnClick_MessageBoxClose()
    {
        if (m_MessagePanel == null)
            return;
        m_MessagePanel.SetActive(false);
        SoundMgr.Instance.PlaySoundFx(SoundFXType.ButtonClick);
    }

    private void Update()
    {
        m_RefreshTime += Time.deltaTime;
        if(m_RefreshTime > m_RefreshTimeLimit)
        {
            m_RefreshTime -= m_RefreshTimeLimit;
            // refresh room
            if(m_RefeshWorking == false)
                StartCoroutine(RefreshRoom());

            // check message box
            CheckMessageBoxBadge();
        }
    }

    ///////////////////////////////////////////////////  Room Method /////////////////////////////////////////////

    private bool GetViewRoomType6R_ViewToggle()
    {
        if (m_IsViewRoom6ROnly)
            return true;
        if (m_ViewRoomToggle_6R.isOn)
            return true;
        return false;
    }

    public int GetBlindType_ViewToggle()
    {
        if (m_ViewBetToggle_200.isOn)
            return 1;
        else if (m_ViewBetToggle_500.isOn)
            return 2;
        return 0;
    }

    public int GetBlindType_CRToggle()
    {
        if (m_CreateRoomBetToggle_200.isOn)
            return 1;
        else if (m_CreateRoomBetToggle_500.isOn)
            return 2;
        return 0;
    }

    

    public IEnumerator CreateRoom(string name)
    {
        int blindType = GetBlindType_CRToggle();
        m_Client.SendCreateRoom(name, blindType);
        Debug.Log("방생성중");
        RecvPacketObject obj = null;
        while(obj==null)
        {
            obj = m_Client.PopPacketObject(Protocols.RoomCreate);
            yield return null;
        }
        int RoomIndex = -1;
        ParserLobby.GetRoomCreate(obj, ref RoomIndex);
        ClientObject.Instance.m_RoomIdx = RoomIndex;
        ClientObject.Instance.m_BlindType = blindType;
        //SceneManager.LoadScene("3_Play5");
        SceneManager.LoadSceneAsync("3_Play9");
        yield return null;
    }

    public void RoomIn(int roomIdx)
    {
        UInt64 leastMoney = 400;
        if(GetBlindType_ViewToggle() == 2)
            leastMoney = 1000;
        if (m_UserInfo.m_Money == 0 || m_UserInfo.m_Money < leastMoney)
        {
            AlertPanel.Instance.StartAlert(2, AlertType.NoMoney);
            return;
        }
        StartCoroutine("RoomInWork", roomIdx);
    }

    IEnumerator RoomInWork(int roomIdx)
    {
        if (m_LoadingPanel)
            m_LoadingPanel.SetActive(true);

        Debug.Log("방입장중");
        m_Client.SendInRoom(roomIdx);
        RecvPacketObject obj = null;
        while (obj == null)
        {
            obj = m_Client.PopPacketObject(Protocols.RoomIn);
            yield return null;
        }
        ParserLobby.RoomInResult r = ParserLobby.RoomInResult.None;
        ParserLobby.GetRoomIn(obj, ref r);

        bool success = false;
        switch (r)
        {
            case ParserLobby.RoomInResult.Success:
                success = true;
                break;
            case ParserLobby.RoomInResult.Fail_NoRoom:
                Debug.Log("방없음");
                AlertPanel.Instance.StartAlert(2, AlertType.NoRoom);
                break;
            case ParserLobby.RoomInResult.Fail_FullRoom:
                Debug.Log("방꽉참");
                AlertPanel.Instance.StartAlert(2, AlertType.FullRoom);
                break;
            case ParserLobby.RoomInResult.Fail_Error:
                Debug.Log("알수없는오류");
                break;
        }

        if (success == false)
        {
            yield return new WaitForSeconds(1);
            if (m_LoadingPanel)
                m_LoadingPanel.SetActive(false);
            yield break;
        }
        Debug.Log("방입장성공");
        ClientObject.Instance.m_RoomIdx = roomIdx;
        ClientObject.Instance.m_BlindType = GetBlindType_ViewToggle();
        //SceneManager.LoadScene("3_Play5");
        SoundMgr.Instance.PlaySoundFx(SoundFXType.RoomIn);
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync("3_Play9");
    }

    IEnumerator RefreshRoom()
    {
        m_RefeshWorking = true;
        RecvPacketObject obj = null;
        int blindType = GetBlindType_ViewToggle(); // get blindtype from toggle btn
        m_Client.SendGetRoomCount(blindType);

        while (obj == null)
        {
            obj = m_Client.PopPacketObject(Protocols.RoomCount);
            yield return null;
        }

        ParserLobby.RoomList roomcount = new ParserLobby.RoomList();
        roomcount.cou = 0;
        ParserLobby.GetRoomCount(obj, ref roomcount);
        // Clear rooms if room count zero
        if (roomcount.cou == 0)
        {
            for (int j = 0; j < m_RoomList.Count; j++)
                Destroy(m_RoomList[j].gameObject);
            m_RoomList.Clear();
            m_RefeshWorking = false;
            yield break;
        }

        // Clear rooms according to view toggle mode
        bool isViewRoom6R = GetViewRoomType6R_ViewToggle();
        if (m_IsOldViewRoom6R != isViewRoom6R)
        {
            for (int j = 0; j < m_RoomList.Count; j++)
                Destroy(m_RoomList[j].gameObject);
            m_RoomList.Clear();
            for (int i = 0; i < m_RoomScrollContainer.m_RoomPanelList.Count; i++)
                Destroy(m_RoomScrollContainer.m_RoomPanelList[i].gameObject);
            m_RoomScrollContainer.SetRoomView(isViewRoom6R);
            m_IsOldViewRoom6R = isViewRoom6R;
        }

        int[] roomIdxArr = new int[roomcount.cou];

        for (int i = 0; i < roomcount.cou; i++)
        {
            m_Client.SendGetRoomInfo(roomcount.roomIds[i]);

            obj = null;
            while (obj == null)
            {
                obj = m_Client.PopPacketObject(Protocols.RoomData);
                yield return null;
            }
            ParserLobby.RoomInfo_Robby data = null;
            ParserLobby.GetRoomData(obj, ref data);
            
            string masterName = "";
            int avata = 0;
            if (data == null)
            {
                //Debug.LogError("ParserLobby.RoomInfo_Robby Data is Null");
                continue;
            }
            if (data.reader != null)
            {
                masterName = data.reader.UserName;
                avata = data.reader.Avatar;
            }

            // Check lobby room
            CheckRoom(data.name, i, data.idx, masterName, avata, data.memberCou, data.blindType);
            roomIdxArr[i] = data.idx;
        }

        // Check empty room
        yield return CheckEmptyRoom(roomIdxArr);

        m_RefeshWorking = false;
        yield return null;
    }

    private IEnumerator CheckEmptyRoom(int[] roomIdxArr)
    {
        List<LobbyRoomData> removeList = new List<LobbyRoomData>();
        for (int i = 0; i < m_RoomList.Count; i++)
        {
            bool isExist = false;
            for (int j = 0; j < roomIdxArr.Length; j++)
            {
                if (roomIdxArr[j] == m_RoomList[i].m_RoomIdx)
                {
                    isExist = true;
                    break;
                }
            }
            if (isExist == false)
                removeList.Add(m_RoomList[i]);
        }

        // remove
        for (int i = 0; i < removeList.Count; i++)
        {
            int idx = m_RoomList.IndexOf(removeList[i]);
            Destroy(m_RoomList[idx].gameObject);
            m_RoomList.RemoveAt(idx);
        }
        removeList.Clear();
        removeList = null;

        yield return new WaitForEndOfFrame();

        // Check RoomPanelContainer
        yield return m_RoomScrollContainer.CheckRoomPanelContainer();
    }

    private void CheckRoom(string name, int num, int idx, string hostName,int hostAvata, int member, int blindType)
    {
        for (int i = 0; i < m_RoomList.Count; i++)
        {
            if (idx == m_RoomList[i].m_RoomIdx)//update
            {
                m_RoomList[i].Set(name, num, idx, hostName, hostAvata, member, blindType);
                return;
            }
        }
        // add room item
        AddRoom(name, num, idx, hostName, hostAvata, member, blindType);
    }

    private void AddRoom(string name, int num, int idx, string hostName, int hostAvata, int member, int blindType)
    {
        GameObject roomObj = Instantiate(m_RoomPrefab);

        // add room list item
        LobbyRoomData room = roomObj.GetComponent<LobbyRoomData>();
        room.Set(name, num, idx, hostName, hostAvata, member, blindType);
        m_RoomList.Add(room);

        // add room to room container
        m_RoomScrollContainer.AddRoom(roomObj);
    }

    ///////////////////////////////////////////////////  Message ////////////////////////////////////////////////


    public void RefreshMessageBox()
    {
        TexasHoldemClient.Instance.SendGetMessageList();
        StartCoroutine(RefreshMessageBoxRoutine());
    }

    private IEnumerator RefreshMessageBoxRoutine()
    {
        List<MessageData> arr = TexasHoldemClient.Instance.PopMessageList();
        while (arr == null)
        {
            arr = TexasHoldemClient.Instance.PopMessageList();
            yield return null;
        }

        m_MessageBox.RefreshMessageItems(arr);
    }

    public void CheckMessageBoxBadge()
    {
        if (m_MessageBoxBadge == null)
            return;
        StartCoroutine(MessageBoxBadgeRoutine());
    }

    private IEnumerator MessageBoxBadgeRoutine()
    {
        TexasHoldemClient c = TexasHoldemClient.Instance;
        c.SendGetMessageCount();

        RecvPacketObject obj = null;
        while (obj == null)
        {
            obj = c.PopPacketObject(Protocols.UserMessageCount);
            yield return null;
        }

        m_MessageCount = 0;
        ParserUserInfo.GetMessageCount(obj, ref m_MessageCount);

        m_MessageBoxBadge.SetActive(false);
        if(m_MessageCount > 0)
            m_MessageBoxBadge.SetActive(true);
    }

    ///////////////////////////////////////////////////  etc Method /////////////////////////////////////////////

    private void ActivePopupWork(GameObject obj)
    {
        //obj.transform.localScale = Vector3.zero;
        obj.SetActive(true);
        //StartCoroutine(PopupWork(obj));
    }

    private IEnumerator PopupWork(GameObject obj)
    {
        Vector3 scale = obj.transform.localScale;
        float per = 0;
        while (per < 1)
        {
            per += Time.deltaTime;
            scale = Vector3.Lerp(scale, Vector3.one, per);
            obj.transform.localScale = scale;
            yield return null;
        }
    }
}
