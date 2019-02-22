using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public partial class TexasHoldemClient : ClientBase
{
    static TexasHoldemClient m_sInstance = null;
    static public TexasHoldemClient Instance
    {
        get
        {
            if (m_sInstance == null)
                m_sInstance = new TexasHoldemClient();
            return m_sInstance;
        }
    }

    PacketParser m_PacketParser = new PacketParser();
    public Action<Protocols, byte[]> m_OnRecvData = null;
    public List<RecvPacketObject> m_PacketObject = new List<RecvPacketObject>();
    object m_PacketLock = new object();

    List<MessageData> m_TempMessageList = new List<MessageData>();
    List<List<MessageData>> m_RecvMessageListArray = new List<List<MessageData>>();
    object m_MessageLock = new object();

    public TexasHoldemClient()
    {
        m_PacketParser.InsertParser(new ParserLobby());
        m_PacketParser.InsertParser(new ParserUserInfo());
        m_PacketParser.InsertParser(new ParserGame());
    }

    public void PacketClear()
    {
        lock (m_PacketLock)
        {
            m_PacketObject.Clear();
        }
    }

    void AddPacketObject(RecvPacketObject obj)
    {
        if (obj == null) return;
        lock (m_PacketLock)
        {
            m_PacketObject.Add(obj);
        }
    }

    public RecvPacketObject PopPacketObject(Protocols protocol)
    {
        lock (m_PacketLock)
        {
            int i, j;
            j = m_PacketObject.Count;
            for (i = 0; i < j; i++)
            {
                if (m_PacketObject[i].protocol == protocol)
                {
                    RecvPacketObject obj = m_PacketObject[i];
                    m_PacketObject.RemoveAt(i);
                    return obj;
                }
            }
            return null;
        }
    }


    protected override void RecvDataWork(RecvData data)
    {
        try
        {
            AddPacketObject(m_PacketParser.Parser((Protocols)data.protocol, data.data));
            if (m_OnRecvData != null)
                m_OnRecvData((Protocols)data.protocol, data.data);
        }
        catch(Exception e)
        {
            Log(e.ToString());
        }

    }

    public override void Update()
    {
        base.Update();
        RecvPacketObject obj = PopPacketObject(Protocols.UserMessage);
        if (obj != null)
        {
            while (obj != null)
            {
                MessageData d = new MessageData();
                if (ParserUserInfo.GetMessageData(obj, ref d) == false)
                {
                    AddMessageList(m_TempMessageList);
                    m_TempMessageList = new List<MessageData>();
                }
                else
                {
                    m_TempMessageList.Add(d);
                }
                obj = PopPacketObject(Protocols.UserMessage);
            }
        }   
    }

    void AddMessageList(List<MessageData> data)
    {
        lock (m_MessageLock)
        {
            m_RecvMessageListArray.Add(data);
        }
    }

    public List<MessageData> PopMessageList()
    {
        lock (m_MessageLock)
        {
            if (m_RecvMessageListArray.Count == 0)
                return null;
            List<MessageData> d = m_RecvMessageListArray[0];
            m_RecvMessageListArray.RemoveAt(0);
            return d;
        }
    }
}
