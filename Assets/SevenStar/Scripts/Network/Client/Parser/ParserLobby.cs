﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ParserLobby : ParserBase
{
    public class RoomInfo_Robby
    {
        public int idx;
        public string name;
        public int cou;
        public int blindType;
        public int memberCou;
        public UserInfo reader;
        public UserInfo[] member;
    }
    
    public class RoomList
    {
        public int cou;
        public int[] roomIds;
    }

    public enum RoomInResult
    {
        None,
        Success,
        Fail_NoRoom,
        Fail_FullRoom,
        Fail_Error,
    }
    
    private Dictionary<string, string> dic = null;

    public RecvPacketObject Parser(Protocols protocol, byte[] data)
    {
        string res=Encoding.UTF8.GetString (data);
        dic = TinyXmlReader.DictionaryFromXMLString(res);
        
        RecvPacketObject obj = new RecvPacketObject();
        obj.protocol = protocol;
        switch (protocol)
        {
            case Protocols.RoomCount:
                obj.obj = RecvRoomCount();
                break;
            case Protocols.RoomData:
                obj.obj = RecvRoomData();
                break;
            case Protocols.RoomCreate:
                obj.obj = RecvRoomCreate();
                break;
            case Protocols.RoomIn:
                obj.obj = RecvRoomIn(data);
                break;
            default:
                return null;
        }
        return obj;
    }

    object RecvRoomCount()
    {
        RoomList list=new RoomList();
        list.cou = Int32.Parse(dic["cou"]);
        list.roomIds = new int[list.cou];
        for (int i = 0; i < list.cou; i++)
        {
            list.roomIds[i] = Int32.Parse(dic["room" + i.ToString()]);
        }
        return list;
    }

    object RecvRoomData()  //roominfo
    {
        if (dic.ContainsKey("roominfo"))
        {
            Debug.LogWarning(dic["roominfo"]);
        }
        if (dic.ContainsKey("roominfo-name"))
        {
            Debug.LogWarning(dic["roominfo-name"]);
        }
        if (dic.ContainsKey("user0-seat"))
        {
            Debug.LogWarning(dic["user0-seat"]);
        }
        if (dic.ContainsKey("user1-seat"))
        {
            Debug.LogWarning(dic["user1-seat"]);
        }

//        RoomInfo_Robby info = new RoomInfo_Robby();
//        info.idx = p.GetInt();
//        info.name = p.GetString();
//        info.blindType = p.GetInt();
//        info.cou = p.GetInt();
//        info.memberCou = 0;
//        info.reader = null;
//        if (info.cou > 0)
//        {
//            info.member = new UserInfo[info.cou];
//            for (int i = 0; i < info.cou; i++)
//            {
//                byte len = p.GetByte();
//                if (len > 0)
//                {
//                    UserInfo ui = new UserInfo();
//                    ui.SetDataBytes(data, p.pos);
//                    p.pos += len;
//                    info.member[i] = ui;
//                    info.memberCou++;
//                    if (info.reader == null)
//                        info.reader = ui;
//
//                }
//                else
//                {
//                    info.member[i] = null;
//                }
//            }
//
//        }
//        return info;
        return null;
    }

    object RecvRoomCreate()  //return roomID
    {
        int v = 0;
        if (dic.ContainsKey("idx"))
            v = Int32.Parse(dic["idx"]);
        return v;
    }

    object RecvRoomIn(byte[] data)
    {
        int r = 0;
        if (dic.ContainsKey("idx"))
        {
            r = 0;
        }
        switch (r)
        {
            case 0:
                return RoomInResult.Success;
            case 1:
                return RoomInResult.Fail_NoRoom;
            case 2:
                return RoomInResult.Fail_FullRoom;
        }
        return RoomInResult.Fail_Error;
    }

    static public bool GetRoomCount(RecvPacketObject obj, ref RoomList Count)
    {
        if (obj.protocol != Protocols.RoomCount)
            return false;
        Count = (RoomList)obj.obj;
        return true;
    }

    static public bool GetRoomData(RecvPacketObject obj, ref RoomInfo_Robby Data)
    {
        if (obj.protocol != Protocols.RoomData)
            return false;
        Data = (RoomInfo_Robby)obj.obj;
        return true;
    }

    static public bool GetRoomCreate(RecvPacketObject obj, ref int RoomIndex)
    {
        if (obj.protocol != Protocols.RoomCreate)
            return false;
        RoomIndex = (int)obj.obj;
        return true;
    }

    static public bool GetRoomIn(RecvPacketObject obj, ref RoomInResult Result)
    {
        if (obj.protocol != Protocols.RoomIn)
            return false;
        Result = (RoomInResult)obj.obj;
        return true;
    }
}
