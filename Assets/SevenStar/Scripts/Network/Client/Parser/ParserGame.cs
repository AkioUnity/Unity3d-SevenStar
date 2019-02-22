using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GameRoomInfo
{
    public int RoomIndex;
    public string RoomName;
    public int BlindType;
}

public class ParserGame : ParserBase
{
    public RecvPacketObject Parser(Protocols protocol, byte[] data)
    {
        RecvPacketObject obj = new RecvPacketObject();
        obj.protocol = protocol;
        switch (protocol)
        {
            case Protocols.RoomPlayerList:
                obj.obj = RecvPlayerList(data);
                break;
            /*case Protocols.RoomReady:
                obj.obj = null;
                break;*/
            case Protocols.GameRoomInfo:
                obj.obj = RecvGameRoomInfo(data);
                break;
            /*case Protocols.Play_Result:
                break;
            case Protocols.Play_Blind:
                break;
            case Protocols.Play_ButtonUser:
                break;
            case Protocols.TestRoomInComplete:
                break;*/
            case Protocols.GetOnCard:
                obj.obj = RecvGetOnCard(data);
                break;
            default:
                return null;
        }
        return obj;
    }

    object RecvPlayerList(byte[] data)
    {
        List<int> recv = new List<int>();
        ByteDataParser p = new ByteDataParser();
        p.Init(data);
        int r = p.GetInt();
        if (r == 0)
            return null;
        
        int cou = p.GetInt();
        for (int i = 0; i < cou; i++)
        {
            //m_RecvPlayerList.Add(p.GetInt());
            recv.Add(p.GetInt());
        }
        return recv;
    }

    object RecvOnyInt(byte[] data)
    {
        return BitConverter.ToInt32(data, 0);
    }

    object RecvGameRoomInfo(byte[] data)
    {
        ByteDataParser p = new ByteDataParser();
        p.Init(data);
        GameRoomInfo r = new GameRoomInfo();
        r.RoomIndex = p.GetInt();
        r.RoomName = p.GetString();
        r.BlindType = p.GetInt();
        return r;
    }

    object RecvGetOnCard(byte[] data)
    {
        ByteDataParser p = new ByteDataParser();
        p.Init(data);
        int r = p.GetInt();
        byte[] d = new byte[6];
        if (r == 0)
            return d;
        d[0] = 1;
        for (int i = 0; i < 5; i++)
            d[i + 1] = p.GetByte();
        return d;
    }
    
    static public bool GetPlayerList(RecvPacketObject obj, ref List<int> data)
    {
        if (obj.protocol != Protocols.RoomPlayerList)
            return false;
        data = (List<int>)obj.obj;
        return true;
    }
    /*
    static public bool GetRoomInPlayer(RecvPacketObject obj, ref int UserIdx)
    {
        if (obj.protocol != Protocols.RoomInPlayer)
            return false;
        UserIdx = (int)obj.obj;
        return true;
    }

    static public bool GetRoomOutPlayer(RecvPacketObject obj, ref int UserIdx)
    {
        if (obj.protocol != Protocols.RoomOutPlayer)
            return false;
        UserIdx = (int)obj.obj;
        return true;
    }
    */

    static public bool GetGameRoomInfo(RecvPacketObject obj, ref GameRoomInfo info)
    {
        if (obj.protocol != Protocols.GameRoomInfo)
            return false;
        info = (GameRoomInfo)obj.obj;
        return true;
    }

    static public bool GetOnCard(RecvPacketObject obj, ref byte[] data)
    {
        if (obj.protocol != Protocols.GetOnCard)
            return false;
        byte[] d = (byte[])obj.obj;
        if (d[0] == 0)
            data = null;
        Array.Copy(d, 1, data, 0, 5);
        return true;
    }
}
