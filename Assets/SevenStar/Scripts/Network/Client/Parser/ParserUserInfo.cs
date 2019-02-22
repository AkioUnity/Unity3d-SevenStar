using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class MessageData
{
    public int MessageIdx = 0;
    public UInt64 Money = 0;
    public string DateTime = "";
    public string Message = "";
    public int Type = 0;
}

public class UserNamePhoneData
{
    public string UserName = "";
    public string UserPhoneNumber = "";
}
public class ParserUserInfo : ParserBase
{
    public enum RegisterResult
    {
        None,
        Success,
        Fail_ID,
        Fail_Nickname,
    }

    public enum LoginResult
    {
        None,
        Success,
        Fail,
        NetError,
        Overlap,
        Banned
    }

    public enum MoneyGiftResult
    {
        Success,
        NoUser,
        NoHaveMoney,
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
            case Protocols.Login:
                obj.obj = RecvLogin(data);
                break;
            case Protocols.UserRegister:
                obj.obj = UserRegister(data);
                break;
            case Protocols.UserInfo:
                obj.obj = RecvUserInfo(data);
                break;
            case Protocols.GetBankMoney:
                obj.obj = RecvBankMoney(data);
                break;
            case Protocols.MoneyGift:
                obj.obj = RecvGiftMoneyResult(data);
                break;
            case Protocols.UserMessage:
                obj.obj = RecvUserMessage(data);
 				break;
            case Protocols.UserMessageCount:
                obj.obj = RecvUserMessageCount(data);
                break;
            case Protocols.CheckIDName:
                obj.obj = RecvCheckIDName(data);
                break;
            case Protocols.ChangeNickname:
                obj.obj = RecvChangeNickname(data);
                break;
            case Protocols.ChangePhonenumber:
                obj.obj = RecvChangePhonenumber(data);
                break;
            case Protocols.ChangePassword:
                obj.obj = RecvChangePassword(data);
                break;
            case Protocols.ChangeName:
                obj.obj = RecvChangePhonenumber(data);
                break;
            case Protocols.GetUserNamePhonenumber:
                obj.obj = RecvUserNamePhonenumber(data);
                break;
            default:
                return null;
        }
        return obj;
    }

    object RecvLogin(byte[] data)
    {
        int Result = -2;
        if (dic.ContainsKey("useridx"))
            Result = Int32.Parse(dic["useridx"]);
        else
        {
            string res = dic["result"];
            if (res == "idpass failed")
                Result = -1;
            else if (res == "overlap")
                Result = -2;
            else
            {
                Debug.LogError(res);
            }
        }
//        Debug.Log(Result);
        return Result;
    }

    object UserRegister(byte[] data)
    {
        int Result = -2;
        if (dic.ContainsKey("useridx"))
            Result = Int32.Parse(dic["useridx"]);
        Debug.Log(Result);
        if (Result>0)
            return RegisterResult.Success;
        string res = dic["result"];
        if (res=="overlap nick")
            return RegisterResult.Fail_Nickname;
        return RegisterResult.Fail_ID;
    }

    object RecvUserInfo(byte[] data)
    {
        UserInfo info = null;
        info = new UserInfo();
        info.UserIdx = Int32.Parse(dic["useridx"]);
        info.UserMoney = Convert.ToUInt64(dic["money"]);
        info.Avatar = Int32.Parse(dic["avatar"]);
        info.UserName = dic["id"];
        info.bankMoney=Convert.ToUInt64(dic["bank"]);
        return info;
//            <nick>a</nick>
    }

    object RecvBankMoney(byte[] data)
    {
        UInt64 Money = BitConverter.ToUInt64(data, 0);
        return Money;
    }

    object RecvGiftMoneyResult(byte[] data)
    {
        int r = BitConverter.ToInt32(data, 0);
        return r;
    }
    
    object RecvUserMessage(byte[] data)
    {
        ByteDataParser p = new ByteDataParser(data);
        int MessageIdx = p.GetInt();
        if (MessageIdx == 0)
            return null;
        MessageData d = new MessageData();
        d.MessageIdx = MessageIdx;
        d.Money = p.GetUInt64();
        d.DateTime = p.GetString();
        d.Message = p.GetString();
        d.Type = p.GetInt();
        return d;
    }

    object RecvUserMessageCount(byte[] data)
    {
        int r = BitConverter.ToInt32(data, 0);
        return r;
    }

    object RecvCheckIDName(byte[] data)
    {
        ByteDataParser p = new ByteDataParser(data);
        int r = p.GetInt();
        return r;
    }
    object RecvChangeNickname(byte[] data)
    {
        ByteDataParser p = new ByteDataParser(data);
        int r = p.GetInt();
        return r;
    }

    object RecvChangePhonenumber(byte[] data)
    {
        ByteDataParser p = new ByteDataParser(data);
        int r = p.GetInt();
        return r;
    }

    object RecvChangePassword(byte[] data)
    {
        ByteDataParser p = new ByteDataParser(data);
        int r = p.GetInt();
        return r;
    }
    object RecvUserNamePhonenumber(byte[] data)
    {
        ByteDataParser p = new ByteDataParser(data);
        UserNamePhoneData d = new UserNamePhoneData();
        d.UserName = p.GetString();
        d.UserPhoneNumber = p.GetString();
        return d;
    }
    static public LoginResult CheckLoginResult(int idx)
    {
        if (idx > 0)
            return LoginResult.Success;
        else if (idx == -2)
            return LoginResult.Overlap;
        else if(idx==-3)
            return LoginResult.Banned;

        return LoginResult.Fail;  
    }

    static public bool GetLoginResult(RecvPacketObject obj, ref int UserIndex)
    {
        if (obj.protocol != Protocols.Login)
            return false;
        UserIndex = (int)obj.obj;
        return true;
    }

    static public bool GetUserRegisterResult(RecvPacketObject obj, ref RegisterResult result)
    {
        if (obj.protocol != Protocols.UserRegister) 
            return false;
        if (obj.obj == null)
            return false;
        result = (RegisterResult)obj.obj;
        return true;
    }

    static public bool GetUserInfo(RecvPacketObject obj, ref UserInfo result)
    {
        if (obj.protocol != Protocols.UserInfo)
            return false;
        if (obj.obj == null)
            return false;
        result = (UserInfo)obj.obj;
        return true;
    }

    static public bool GetBankMoney(RecvPacketObject obj,ref UInt64 money)
    {
        if (obj.protocol != Protocols.GetBankMoney)
            return false;
        if (obj.obj == null)
            return false;
        money = (UInt64)obj.obj;
        return true;
    }

    static public bool GetMoneyGiftReusult(RecvPacketObject obj, ref MoneyGiftResult result)
    {
        if (obj.protocol != Protocols.MoneyGift)
            return false;
        if (obj.obj == null)
            return false;

        int r = (int)obj.obj;
        if (r == 0)
            result = MoneyGiftResult.NoUser;
        else if (r == -1)
            result = MoneyGiftResult.NoHaveMoney;
        else
            result = MoneyGiftResult.Success;
        return true;
    }

    static public bool GetMessageData(RecvPacketObject obj, ref MessageData data)
    {
        if (obj.protocol != Protocols.UserMessage)
            return false;
        if (obj.obj == null)
            return false;

        data = (MessageData)obj.obj;
        return true;
    }
    static public bool GetMessageCount(RecvPacketObject obj, ref int count)
    {
        if (obj.protocol != Protocols.UserMessageCount)
            return false;
        if (obj.obj == null)
            return false;
        count = (int)obj.obj;
        return true;
    }
    static public bool CheckIDName(RecvPacketObject obj, ref bool result)
    {
        if (obj.protocol != Protocols.CheckIDName)
            return false;
        if (obj.obj == null)
            return false;
        int r = (int)obj.obj;
        if (r == 0)
            result = false;
        else
            result = true;
        return true;
    }

    static public bool CheckChangeNickname(ref bool result)
    {
        RecvPacketObject obj= TexasHoldemClient.Instance.PopPacketObject(Protocols.ChangeNickname);
        if (obj == null)
            return false;
        int r = (int)obj.obj;
        if (r == 0)
            result = false;
        else
            result = true;
        return true;
    }

    static public bool CheckChangePhonenumber(ref bool result)
    {
        RecvPacketObject obj = TexasHoldemClient.Instance.PopPacketObject(Protocols.ChangePhonenumber);
        if (obj == null)
            return false;
        int r = (int)obj.obj;
        if (r == 0)
            result = false;
        else
            result = true;
        return true;
    }

    static public bool CheckChangePassword(ref bool result)
    {
        RecvPacketObject obj = TexasHoldemClient.Instance.PopPacketObject(Protocols.ChangePassword);
        if (obj == null)
            return false;
        int r = (int)obj.obj;
        if (r == 0)
            result = false;
        else
            result = true;
        return true;
    }

    static public bool CheckChangeUsername(ref bool result)
    {
        RecvPacketObject obj = TexasHoldemClient.Instance.PopPacketObject(Protocols.ChangeName);
        if (obj == null)
            return false;
        int r = (int)obj.obj;
        if (r == 0)
            result = false;
        else
            result = true;
        return true;
    }

    static public bool CheckUserNamePhonenumber(ref string UserName, ref string UserPhonenumber)
    {
        RecvPacketObject obj = TexasHoldemClient.Instance.PopPacketObject(Protocols.GetUserNamePhonenumber);
        if (obj == null)
            return false;
        UserNamePhoneData d = (UserNamePhoneData)obj.obj;
        UserName = d.UserName;
        UserPhonenumber = d.UserPhoneNumber;
        return true;
    }
}
