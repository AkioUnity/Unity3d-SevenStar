using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class ClientBase
{

    public class WorkBuffer
    {
        public const int BufferSize = 8192;
        public byte[] buffer = new byte[BufferSize];
        public int BufferPos = 0;

        public void Clear()
        {
            BufferPos = 0;
        }

        public void AddBuffer(byte[] data, int length)
        {
            Array.Copy(data, 0, buffer, BufferPos, length);
            BufferPos += length;
        }

        public byte[] Work()
        {
            if (BufferPos < 12)
                return null;
            int workPos = 0;
            while (BufferPos >= workPos + 12)
            {
                int len = BitConverter.ToInt32(buffer, workPos);
                if (len > ClientBase.RecvBufferSize)
                {
                    workPos += 1;
                    continue;
                }
                if (BufferPos < workPos + len)
                    break;
                byte[] rData = new byte[len];
                Array.Copy(buffer, workPos, rData, 0, len);
                workPos += len;
                PopFront(workPos);
                return rData;
            }
            PopFront(workPos);
            return null;
        }

        void PopFront(int length)
        {
            int LeftLen = BufferPos - length;
            if (LeftLen <= 0)
            {
                BufferPos = 0;
                return;
            }
            Array.Copy(buffer, length, buffer, 0, LeftLen);
            BufferPos -= length;
        }
    }

    const int RecvBufferSize = 2048;
    public class RecvData
    {
        public byte[] data;
        public int protocol;
    }

    Socket m_Sock;
    int m_PacketNumber = 0;
    //int m_ServerPort = 12300;
    int m_ServerPort = 13300;

    byte[] m_RecvBuffer = new byte[RecvBufferSize];
    WorkBuffer m_WorkBuf = new WorkBuffer();
    IAsyncResult m_RecvAsync = null;
    public Action m_OnDisconnect = null;

    List<RecvData> m_RecvData = new List<RecvData>();
    static public Action<string> LogFunc = null;


    public bool IsConnect = true;
    
    static public void Log(string logMessage)
    {
        if (LogFunc == null)
            return;
        LogFunc(logMessage);
    }

    public bool Connect(string ServerIP)
    {
        m_PacketNumber = 0;
        m_WorkBuf.Clear();
        return true;
    }

    public bool Send(Protocols protocol, byte[] data)
    {
        string sendStr = Protocol.GetName(protocol);
        if (sendStr == "noProtocol")
            return false;
        sendStr=sendStr+Encoding.UTF8.GetString(data);
        Debug.Log(sendStr);
        Ws.ws.SendString(sendStr);
        return true;
    }

    public bool Send_int(Protocols protocol, int v)
    {
        if (v <= 0)
        {
            Debug.LogError(protocol+"  user id:"+v);
            return false;
        }
            
        string str="<useridx>" + v.ToString() + "</useridx>";
        byte[] data =Encoding.UTF8.GetBytes(str);
        return Send(protocol, data);
    }

    public bool Send_UInt64(Protocols protocol, UInt64 v)
    {
        byte[] data = BitConverter.GetBytes(v);
        return Send(protocol, data);
    }

    static public byte[] StringArrayToByte(string[] strArr)
    {
        byte[] rData;
        List<byte[]> tmp = new List<byte[]>();
        int totLen = 0;
        int i, j;
        j = strArr.Length;
        for (i = 0; i < j; i++)
            tmp.Add(Encoding.UTF8.GetBytes(strArr[i]));
        totLen = j;
        for (i = 0; i < j; i++)
            totLen += tmp[i].Length;
        rData = new byte[totLen];
        int pos = 0;
        for (i = 0; i < j; i++)
        {
            rData[pos] = (byte)tmp[i].Length;
            Array.Copy(tmp[i], 0, rData, pos + 1, tmp[i].Length);
            pos += tmp[i].Length + 1;
        }
        return rData;
    }

    public virtual void Update()
    {
        while (true)
        {
            try
            {
                RecvData d = GetRecvData();
                if (d == null)
                    break;
                RecvDataWork(d);
            }
            catch (Exception e)
            {
                Log(e.ToString());
            }
        }
    }

    public void AddRecvData(int pro,byte[] data0)
    {
        RecvData d = new RecvData();
        d.protocol = pro;
        d.data = data0;
        m_RecvData.Add(d);
    }

    RecvData GetRecvData()
    {
            try
            {
                while (m_RecvData.Count > 0)
                {
                    RecvData d = m_RecvData[0];
                    m_RecvData.RemoveAt(0);
                    return d;
                }
            }
            catch(Exception e)
            {
                Log(e.ToString());
            }
        return null;
    }

    protected virtual void RecvDataWork(RecvData data)
    {

    }
}
