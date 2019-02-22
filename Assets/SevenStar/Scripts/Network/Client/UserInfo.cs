using System;

public class UserInfo
{
    public int UserIdx;
    public UInt64 UserMoney;
    public string UserName;
    public int Avatar;
    public UInt64 bankMoney;

    public byte[] GetBytes()
    {
        ByteDataMaker d = new ByteDataMaker();
        d.Init(200);
        d.Add(UserIdx);
        d.Add(UserMoney);
        d.Add(Avatar);
        d.Add(UserName);
        return d.GetBytes();
    }

    public void SetDataBytes(byte[] data, int pos)
    {
        ByteDataParser p = new ByteDataParser();
        p.Init(data);
        p.SetPos(pos);
        UserIdx = p.GetInt();
        UserMoney = p.GetUInt64();
        Avatar = p.GetInt();
        UserName = p.GetString();
    }
}
