using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public partial class TexasHoldemClient
{
    public void Login(string id, string pass)
    {
        string[] strArr = new string[2] { id, pass };
        byte[] data = StringArrayToByte(strArr);
        Send(Protocols.Login, data);
    }

    public void UserRegister(string id, string pass, string nickname, string name, string phone)
    {
        if (id.Length == 0 || pass.Length == 0 || nickname.Length == 0 || name.Length == 0 || phone.Length == 0)
            return;

        string[] strArr = new string[5] { id, pass, nickname, name, phone };
        byte[] data = StringArrayToByte(strArr);

        Send(Protocols.UserRegister, data);
    }

    public void SendGetUserInfo(int UserIdx)
    {
        Send_int(Protocols.UserInfo, UserIdx);
    }

    public void SendCreateRoom(string roomName, int blindType)
    {
        ByteDataMaker d = new ByteDataMaker();
        d.Init(500);
        d.Add(roomName);
        d.Add(blindType);
        Send(Protocols.RoomCreate, d.GetBytes());
    }

    public void SendGetRoomCount(int BlindType)
    {
        Send_int(Protocols.RoomCount, BlindType);
    }
    /*public void SendGetRoomInfo(bool IsIndex, int n)
    {
        ByteDataMaker d = new ByteDataMaker();
        d.Init(20);
        if (IsIndex)
            d.Add((byte)1);
        else
            d.Add((byte)0);
        d.Add(n);
        Send(Protocols.RoomData, d.GetBytes());
    }*/

    public void SendGetRoomInfo(int blindType, int n)
    {
        ByteDataMaker d = new ByteDataMaker();
        d.Init(20);
        d.Add((byte)0);
        d.Add(blindType);
        d.Add(n);
        Send(Protocols.RoomData, d.GetBytes());
    }

    public void SendGetRoomInfoIndex(int RoomIndex)
    {
        ByteDataMaker d = new ByteDataMaker();
        d.Init(20);
        d.Add((byte)1);
        d.Add(RoomIndex);
        Send(Protocols.RoomData, d.GetBytes());
    }

    public void SendInRoom(int roomIdx)
    {
        Send_int(Protocols.RoomIn, roomIdx);
    }

    public void SendOutRoom()
    {
        Send(Protocols.RoomOut, new byte[0]);
    }

    public void SendBetting(UInt64 money)
    {
        ByteDataMaker m = new ByteDataMaker();
        m.Init(10);
        m.Add(money);
        Send(Protocols.PlayerBetting, m.GetBytes());
    }

	public void SendCall()
    {
        Send_int(Protocols.PlayerCall, 0);
    }

	public void SendFold()
    {
        Send_int(Protocols.PlayerFold, 0);
    }

    public void SendGetBankMoney()
    {
        Send_int(Protocols.GetBankMoney, 0);
    }

    public void SendBankInMoney(UInt64 Money)
    {
        Send_UInt64(Protocols.BankIn, Money);
    }

    public void SendBankOutMoney(UInt64 Money)
    {
        Send_UInt64(Protocols.BankOut, Money);
    }

    public void SendPlayInfo(int RoomIndex)
    {
        Send_int(Protocols.PlayInfo, RoomIndex);
    }

    /*
    public void SendDepositRequest(UInt64 Money)
    {
        ByteDataMaker m = new ByteDataMaker();
        m.Init(10);
        m.Add(Money);
        Send(Protocols.DepositRequest, m.GetBytes());
    }
    */
    public void SendChargeRequest(UInt64 Money,string AccountName)
    {
        ByteDataMaker m = new ByteDataMaker();
        m.Init(200);
        m.Add(Money);
        m.Add(AccountName);
        Send(Protocols.ChargeRequest, m.GetBytes());
    }

    public void SendWithdrawal(UInt64 Money, string BankName, string AccountName, string AccountNumber)
    {
        ByteDataMaker m = new ByteDataMaker();
        m.Init(500);
        m.Add(Money);
        m.Add(AccountName);
        m.Add(BankName);
        m.Add(AccountNumber);
        Send(Protocols.Withdrawal, m.GetBytes());
    }

    public void SendGiftMoney(string RecvUserNickName, UInt64 Money)
    {
        ByteDataMaker m = new ByteDataMaker();
        m.Init(30);
        m.Add(RecvUserNickName);
        m.Add(Money);
        Send(Protocols.MoneyGift, m.GetBytes());
    }

    public void SendGetMessageList()
    {
        Send_int(Protocols.UserMessage, 0);
    }

    public void SendMessageRead(int MessageIdx)
    {
        Send_int(Protocols.UserMessageReceive, MessageIdx);
    }

    public void SendGetMessageCount()
    {
        Send_int(Protocols.UserMessageCount, 0);
    }

    public void SendCheckIDName(int type,string str)//type 0 - id   //  type 1 - Nickname
    {
        ByteDataMaker m = new ByteDataMaker();
        m.Init(100);
        m.Add(type);
        m.Add(str);
        Send(Protocols.CheckIDName, m.GetBytes());
    }

    public void SendChangeNickname(string Nickname)
    {
        ByteDataMaker m = new ByteDataMaker();
        m.Init(100);
        m.Add(Nickname);
        Send(Protocols.ChangeNickname, m.GetBytes());
    }

    public void SendChangePhonenumber(string phoneNumber)
    {
        ByteDataMaker m = new ByteDataMaker();
        m.Init(100);
        m.Add(phoneNumber);
        Send(Protocols.ChangePhonenumber, m.GetBytes());
    }

    public void SendChangePassword(string oldPass, string newPass)
    {
        ByteDataMaker m = new ByteDataMaker();
        m.Init(200);
        m.Add(oldPass);
        m.Add(newPass);
        Send(Protocols.ChangePassword, m.GetBytes());
    }

    public void SendChangeUserName(string Name)
    {
        ByteDataMaker m = new ByteDataMaker();
        m.Init(100);
        m.Add(Name);
        Send(Protocols.ChangeName, m.GetBytes());
    }
    public void SendGetUserNamePhonenumber()
    {
        Send_int(Protocols.GetUserNamePhonenumber, 0);
    }
    public void SendLogOut()
    {
        Send_int(Protocols.LogOut, 0);
    }

}
