using System;
using System.Collections;
using System.Collections.Generic;

public class UserMgr
{
    static UserMgr m_sInstance = null;
    static public UserMgr Instance
    {
        get
        {
            if (m_sInstance == null)
                m_sInstance = new UserMgr();
            return m_sInstance;
        }
    }

    List<UserInfo> m_UserList = new List<UserInfo>();
    object m_Lock = new object();
    
    public void Update()
    {
        RecvPacketObject obj = TexasHoldemClient.Instance.PopPacketObject(Protocols.UserInfo);
        if (obj == null)
            return;
        UserInfo info = null;
        ParserUserInfo.GetUserInfo(obj, ref info);
        AddUserInfo(info);
    }

    void AddUserInfo(UserInfo info)
    {
        if (info == null)
            return;
        lock (m_Lock)
        {
            int i, j;
            j = m_UserList.Count;
            for (i = 0; i < j; i++)
            {
                if (m_UserList[i].UserIdx == info.UserIdx)
                {
                    m_UserList[i] = info;
                    return;
                }
            }
            m_UserList.Add(info);
        }
    }

    public UserInfo GetUserInfo(int UserIdx)
    {
        lock (m_Lock)
        {
            int i, j;
            j = m_UserList.Count;
            for (i = 0; i < j; i++)
            {
                if (m_UserList[i].UserIdx == UserIdx)
                {
                    return m_UserList[i];
                }
            }
            return null;
        }
    }

    public void Clear()
    {
        lock (m_Lock)
        {
            m_UserList.Clear();
        }
    }
}
