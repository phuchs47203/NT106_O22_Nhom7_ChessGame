﻿using System;
using Unity.Networking.Transport;
using UnityEngine;

public enum OpCode
{
    KEEP_ALIVE = 1,
    WELCOME = 2,
    START_GAME = 3,
    PIECE_SELECTED = 4,
    MAKE_MOVE = 5,
    VICTORY_CLAIM = 6,
    READY = 7,
    SWITCH_TEAM = 8,
    REMATCH = 9
}

public static class NetUtility
{
    // Net messages
    public static Action<NetMessage> C_KEEP_ALIVE;
    public static Action<NetMessage> C_WELCOME;
    public static Action<NetMessage> C_START_GAME;
    public static Action<NetMessage> C_PIECE_SELECTED;
    public static Action<NetMessage> C_MAKE_MOVE;
    public static Action<NetMessage> C_VICTORY_CLAIM;
    public static Action<NetMessage> C_READY;
    public static Action<NetMessage> C_SWITCH_TEAM;
    public static Action<NetMessage> C_REMATCH;
    public static Action<NetMessage, NetworkConnection> S_KEEP_ALIVE;
    public static Action<NetMessage, NetworkConnection> S_WELCOME;
    public static Action<NetMessage, NetworkConnection> S_START_GAME;
    public static Action<NetMessage, NetworkConnection> S_PIECE_SELECTED;
    public static Action<NetMessage, NetworkConnection> S_MAKE_MOVE;
    public static Action<NetMessage, NetworkConnection> S_VICTORY_CLAIM;
    public static Action<NetMessage, NetworkConnection> S_READY;
    public static Action<NetMessage, NetworkConnection> S_SWITCH_TEAM;
    public static Action<NetMessage, NetworkConnection> S_REMATCH;

    public static void OnData(DataStreamReader streamReader, NetworkConnection cnn, Server server = null)
    {
        NetMessage msg = null;
        var OpCode = (OpCode)streamReader.ReadByte();
        switch (OpCode)
        {
            case OpCode.KEEP_ALIVE:
                msg = new NetKeepAlive(streamReader); // trạng thái còn sống, chưa thua
                break;

            case OpCode.WELCOME:
                msg = new NetWelcome(streamReader); // màn hình welcome ban đầu
                break;

            case OpCode.START_GAME:
                msg = new NetStartGame(streamReader); // bắt đầu game
                break;

            case OpCode.PIECE_SELECTED:
                msg = new NetPieceSelected(streamReader); // xử lí khi có quân cờ được chọn
                break;

            case OpCode.MAKE_MOVE:
                msg = new NetMakeMove(streamReader); // di chuyển, otnrg net message
                break;

            case OpCode.VICTORY_CLAIM:
                msg = new NetVictoryClaim(streamReader); // gọi thông báo chiến thắng
                break;

            case OpCode.READY:
                msg = new NetReady(streamReader); // trnajg thái game sẵn sàng, chwof đợi người chơi
                break;

            case OpCode.SWITCH_TEAM:
                msg = new NetSwitchTeam(streamReader); // đổi team
                break;

            case OpCode.REMATCH:
                msg = new NetRematch(streamReader); // reload lại game
                break;

            default:
                Debug.LogError("Message received has no OpCode");
                break;
        }

        if (server != null)
            msg.ReceivedOnServer(cnn);
        else
            msg.ReceivedOnClient();
    }
}
