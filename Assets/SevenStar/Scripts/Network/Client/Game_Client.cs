using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Game_Client : Game_ClientAction
{
    //public TexasHoldemClient m_Client = null;


    public void OnRecvData(Protocols protocol, byte[] data)
    {
        /*
         RoomInPlayer,//방에 새로들어온 사용자(RoomIn과 구분이 필요해졌음)
    RoomReady,//게임준비 알림
    PlayerMoneyChange,//플레이어 돈 변경 되었음 (서버가 일방적으로 플레이어에게 쏴주는것 - 배팅할때마다 쏨)
    PlayerBetting,//배팅(서버가 유저에게 배팅을 보내면 그 유저가 배팅한다 - 순서대로)
    PlayerFold,//폴드
    Play_HoleCard,//처음 플레이어에게 2장 주는카드 각각 플레이어에게 준다.
    Play_HoleCardEnd,//다주었다~
    Play_Flop,//3장카드를 오픈
    Play_Turn,//4번째카드 오픈
    Play_River,//5번째카드 오픈
         */
        switch (protocol)
        {
            case Protocols.RoomInPlayer:
                RecvRoomInPlayer(data);
                break;
            case Protocols.RoomOutPlayer:
                RecvRoomOutPlayer(data);
                break;
            case Protocols.NewGameStart:
                RecvNewGameStart();
                break;
            case Protocols.RoomReady:
                RecvGameReady(data);
                break;//*/
            case Protocols.PlayerBetting:
                RecvBettingUser(data);
                break;
            case Protocols.PlayerCall:
                RecvCall(data);
                break;
            case Protocols.PlayerFold:
                RecvFold(data);
                break;
            case Protocols.Play_HoleCard:
                RecvHoldCard(data);
                break;
            case Protocols.Play_Flop:
                RecvOnTableCard(1, data);
                break;
            case Protocols.Play_Turn:
                RecvOnTableCard(2, data);
                break;
            case Protocols.Play_River:
                RecvOnTableCard(3, data);
                break;
            case Protocols.Play_OnCardAll:
                RecvOnTableCard(0, data);
                break;
            case Protocols.Play_ResultCard:
                RecvResultCard(data);
                break;
            case Protocols.Play_Result:
                //RecvResultWinner(data);
                RecvResultRank(data);
                break;
            case Protocols.PlayerNowBettingMoney:

                break;
            case Protocols.Play_PotMoney:
                RecvPotMoney(data);
                break;
            case Protocols.Play_Blind:
                RecvBlindUser(data);
                break;
            case Protocols.Play_ButtonUser:
                RecvButtonUser(data);
                break;
            case Protocols.PlayInfo:
                RecvPlayInfo(data);
                break;
            case Protocols.UserBonusEvent:
                RecvBonusEvent(data);
                break;
            case Protocols.UserBonusEventAll:
                RecvBonusEventAll(data);
                break;
            default:
                break;
        }
    }
    
    
    public void RecvRoomInPlayer(byte[] data)
    {
        ByteDataParser p = new ByteDataParser();
        p.Init(data);
        int UserIdx = p.GetInt();
        if (OnRoomInPlayer != null)
            OnRoomInPlayer(UserIdx);
    }

    public void RecvRoomOutPlayer(byte[] data)
    {
        ByteDataParser p = new ByteDataParser();
        p.Init(data);
        int UserIdx = p.GetInt();
        if (OnRoomOutPlayer != null)
            OnRoomOutPlayer(UserIdx);
    }

    public void RecvNewGameStart()
    {
        if (OnNewGameStart == null)
            return;
        OnNewGameStart();
    }

    public void RecvGameReady(byte[] data)
    {
        //m_Client.Send_int(Protocols.RoomReady, 0);
        TexasHoldemClient.Instance.Send_int(Protocols.RoomReady, 0);
        if (OnGameReady != null)
            OnGameReady();
    }

    public void RecvBetting(byte[] data)
    {
        int idx = BitConverter.ToInt32(data, 0);
        if (OnRecvBettingPlayer != null)
            OnRecvBettingPlayer(idx);
    }

    public void RecvCall(byte[] data)
    {
        if (OnRecvPlayerCall == null) return;
        ByteDataParser p = new ByteDataParser();
        p.Init(data);
        int UserIdx = p.GetInt();
        UInt64 Money = p.GetUInt64();
#if !(UNITY_EDITOR)
        System.Diagnostics.Debug.WriteLine("User : " + UserIdx + " / Money - " + Money);
#endif
        OnRecvPlayerCall(UserIdx, Money);
    }

    public void RecvFold(byte[] data)
    {
        if (OnRecvPlayerFold == null)
            return;
        int idx = BitConverter.ToInt32(data, 0);
        OnRecvPlayerFold(idx);
        
    }

    public void RecvHoldCard(byte[] data)
    {
        ByteDataParser p = new ByteDataParser();
        p.Init(data);
        int i, j;
        j = p.GetInt();
        int idx;
        byte c1, c2;
        for (i = 0; i < j; i++)
        {
            idx = p.GetInt();
            c1 = p.GetByte();
            c2 = p.GetByte();
            if (idx == -1)
                continue;

            if (OnHoldCard != null)
                OnHoldCard(idx, c1, c2);
        }
        /*
        int userIdx = p.GetInt();
        byte card1, card2;
        card1 = p.GetByte();
        card2 = p.GetByte();

        
            */
    }

    public void RecvOnTableCard(int type, byte[] data)
    {
        /// <summary>
        /// 공개되는 카드를 알려준다.
        /// int 종류 byte 카드
        /// 0 모든카드(5장)
        /// 1 Flob (3장)
        /// 2 Turn (1장)
        /// 3 River(1장)
        /// </summary>
        if (OnTableCard == null)
            return;
        int CheckCount = 5;
        switch(type)
        {
            case 0:
                CheckCount = 5;
                break;
            case 1:
                CheckCount = 3;
                break;
            case 2:
            case 3:
                CheckCount = 1;
                break;
            default:
                return;
        }
        if (data.Length < CheckCount)
            return;
        byte[] s = new byte[CheckCount];
        Array.Copy(data, s, CheckCount);
        OnTableCard(type, s);
    }

    public void RecvResultCard(byte[] data)
    {
        if (OnUserResultCard == null)
            return;
        int UserIndex = 0;
        int CardLen;
        ByteDataParser p = new ByteDataParser();
        p.Init(data);
        UserIndex = p.GetInt();
        CardLen = (int)p.GetByte();
        if (CardLen != 7)
            return;
        byte[] card = new byte[7];
        for (int i = 0; i < 7; i++)
            card[i] = p.GetByte();
        OnUserResultCard(UserIndex, card);
    }

    public void RecvResultWinner(byte[] data)
    {
        ByteDataParser p = new ByteDataParser();
        p.Init(data);
        int i, j;
        j = p.GetInt();
        if (j == 0)
            return;
        if (j == 1)
        {
            if (OnWinner == null)
                return;
            OnWinner(p.GetInt(), p.GetUInt64());
        }
        else
        {
            if (OnDraw == null)
                return;
            for (i = 0; i < j; i++)
            {
                OnDraw(p.GetInt(), p.GetUInt64());
            }
        }
    }
    

    public void RecvResultRank(byte[] data)
    {
        if (OnRank == null)
            return;
        ByteDataParser p = new ByteDataParser();
        p.Init(data);
        int i, j;
        j = p.GetInt();
        if (j == 0)
            return;
        List<Game_Client_RankData> r = new List<Game_Client_RankData>();
        for (i = 0; i < j; i++)
        {
            Game_Client_RankData node = new Game_Client_RankData();
            node.UserIndex = p.GetInt();
            node.Rank = p.GetInt();
            node.DividendsMoney = p.GetUInt64();
            r.Add(node);
        }
        OnRank(r);
    
    }

    public void RecvBlindUser(byte[] data)
    {
        ByteDataParser p = new ByteDataParser();
        p.Init(data);
        int small, big;
        small = p.GetInt();
        big = p.GetInt();
        if (OnRecvBlindUser != null)
            OnRecvBlindUser(small, big);
    }

    public void RecvPlayerNowBettingMoney(byte[] data)
    {
        if (OnRecvNowBettingMoney == null) return;
        ByteDataParser p = new ByteDataParser(data);
        int UserIdx = p.GetInt();
        UInt64 Money = p.GetUInt64();
        OnRecvNowBettingMoney(UserIdx, Money);
    }


    public void RecvPotMoney(byte[] data)
    {
        ByteDataParser p = new ByteDataParser(data);
        UInt64 PotMoney = p.GetUInt64();
        UInt64 CallMoney = p.GetUInt64();
        if (OnRecvPotMoney != null)
            OnRecvPotMoney(PotMoney, CallMoney);
    }

    public void RecvBettingUser(byte[] data)
    {
        ByteDataParser p = new ByteDataParser(data);
        int type = p.GetInt();
        int idx = p.GetInt();
        if (type == 0)
        {
            if (OnRecvBettingPlayer != null)
                OnRecvBettingPlayer(idx);
        }
        else if (type == 1)
        {
            UInt64 money = p.GetUInt64();
            if (OnRecvPlayerBetting != null)
                OnRecvPlayerBetting(idx, money);
        }
    }

    public void RecvButtonUser(byte[] data)
    {
        if (OnRecvButtonUser == null)
            return;
        ByteDataParser p = new ByteDataParser(data);
        int UserIdx = p.GetInt();
        OnRecvButtonUser(UserIdx);
    }

    public void RecvPlayInfo(byte[] data)
    {
        if (OnRecvPlayInfo == null)
            return;
        ByteDataParser p = new ByteDataParser(data);
        int i, j;
        j = p.GetInt();
        int UserIdx;
        UInt64 tot, now;
        byte c1, c2;
        for (i = 0; i < j; i++)
        {
            UserIdx = p.GetInt();
            if (UserIdx == -1)
                continue;
            tot = p.GetUInt64();
            now = p.GetUInt64();
            c1 = p.GetByte();
            c2 = p.GetByte();
            OnRecvPlayInfo(UserIdx, tot, now, c1, c2);
        }
    }
    public void RecvBonusEvent(byte[] data)
    {
        if (OnRecvBonusEvent == null)
            return;
        ByteDataParser p = new ByteDataParser(data);
        int type = p.GetInt();
        string str = p.GetString();
        UInt64 Money = p.GetUInt64();
        OnRecvBonusEvent(type, str, Money);
    }

    public void RecvBonusEventAll(byte[] data)
    {
        ByteDataParser p = new ByteDataParser(data);
        int i, j;
        List<int> arrIdx = new List<int>();
        j = p.GetInt();
        if (j == 0)
            return;
        for (i = 0; i < j; i++)
        {
            arrIdx.Add(p.GetInt());
        }
        int type = p.GetInt();
        string str = p.GetString();
        UInt64 Money = p.GetUInt64();
        OnRecvBonusEvent(type, str, Money);
    }
}
