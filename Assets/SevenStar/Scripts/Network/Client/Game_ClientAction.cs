using System;
using System.Collections;
using System.Collections.Generic;

public class Game_Client_RankData
{
    public int UserIndex;
    public int Rank;
    public UInt64 DividendsMoney;
}

public class Game_ClientAction
{

    /// <summary>
    /// 게임이 초기화되었다는것을 알려주는것
    /// </summary>
    public Action OnNewGameStart = null;
    /// <summary>
    /// 게임이 준비되었다는것을 알려주는것
    /// 내부적으로 처리해주기때문에 알필요는 없을듯
    /// </summary>
    public Action OnGameReady = null;
    /// <summary>
    /// 첫번째 두장의 카드를 분배
    /// 모든 유저가 모든 카드를 다 받기 때문에 클라이언트가 알아서 가려야함
    /// int 인덱스 byte 첫번째 카드 byte 두번째카드
    /// </summary>
    public Action<int, byte, byte> OnHoldCard = null;
    /// <summary>
    /// 공개되는 카드를 알려준다.
    /// int 종류 byte 카드
    /// 0 모든카드(5장)
    /// 1 Flob (3장)
    /// 2 Turn (1장)
    /// 3 River(1장)
    /// </summary>
    public Action<int, byte[]> OnTableCard = null;
    /// <summary>
    /// 게임이 끝나고 결과카드를 알려준다.
    /// int 유저인덱스 byte[] 카드5장
    /// </summary>
    public Action<int, byte[]> OnUserResultCard = null;
    /// <summary>
    /// 승리한 사람을 알려준다.
    /// int 유저인덱스 uint64 승리하면서 가져간 금액
    /// </summary>
    public Action<int, UInt64> OnWinner = null;
    /// <summary>
    /// 비긴사람을 알려준다.
    /// int 유저인덱스 uint64 승리하면서 가져간 금액
    /// </summary>
    public Action<int, UInt64> OnDraw = null;
    
    /// <summary>
    /// 게임 결과를 알려준다.
    /// rank 0 은 승리한사람
    /// </summary>
    public Action<List<Game_Client_RankData>> OnRank = null;
    /// <summary>
    /// 유저가 들어왔다는것을 알려주는것
    /// int 인덱스
    /// </summary>
    public Action<int> OnRoomInPlayer = null;
    /// <summary>
    /// 유저가 나갔다는것을 알려주는것
    /// int 인덱스
    /// </summary>
    public Action<int> OnRoomOutPlayer = null;
    /// <summary>
    /// 어느 플레이어가 베팅하는지 알려주는거
    /// int 인덱스
    /// </summary>
    public Action<int> OnRecvBettingPlayer = null;
    /// <summary>
    /// 어느 플레이어가 얼마를 베팅했는지 알려주는거
    /// int 인덱스 uint64 금액
    /// </summary>
    public Action<int, UInt64> OnRecvPlayerBetting = null;
    /// <summary>
    /// 어느 플레이어가 콜했는지를 알려준다.
    /// int 인덱스 uint64 금액
    /// </summary>
    public Action<int, UInt64> OnRecvPlayerCall = null;
    /// <summary>
    /// 어느 플레이어가 폴드했는지를 알려준다.
    /// </summary>
    public Action<int> OnRecvPlayerFold = null;
    /// <summary>
    /// 어느 플레이어가 이번턴에서 현재 누적배팅이 얼마인지 알려준다.(배팅턴이 끝날때마다 초기화)
    /// </summary>
    public Action<int, UInt64> OnRecvNowBettingMoney = null;
    /// <summary>
    /// 게임판돈? 배팅금액 모인거 알려줌
    /// uint64 금액 uint64 콜머니
    /// </summary>
    public Action<UInt64, UInt64> OnRecvPotMoney = null;
    /// <summary>
    /// 스몰블라인드와 빅블라인드가 누군지 알려준다.
    /// int 스몰블라인드 인덱스 int 빅블라인드 인덱스
    /// </summary>
    public Action<int, int> OnRecvBlindUser = null;
    /// <summary>
    /// 버튼유저가 누구인지 알려준다.
    /// int 유저인덱스
    /// </summary>
    public Action<int> OnRecvButtonUser = null;

    /// <summary>
    /// 중간에 유저가 들어왔을때 현재 게임정보를 요청해서 알려준다.
    /// int 유저인덱스 UInt64토탈배팅금액 UInt64현재배팅금액 byte 카드1 byte카드2
    /// </summary>
    public Action<int, UInt64, UInt64, byte, byte> OnRecvPlayInfo = null;
    public delegate void Action<in T1, in T2, in T3, in T4, in T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    /// <summary>
    /// 좋은 패를 잡아서 이벤트가 발생했을때
    /// </summary>
    public Action<int, string, UInt64> OnRecvBonusEvent = null;
}
