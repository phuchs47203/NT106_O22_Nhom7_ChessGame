using System;
using Unity.Networking.Transport;
using UnityEngine;

// định nghĩa enum cho các loại tin nhắn
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

// Client khi gửi đến server thì không cần biết về networkconenction của nó
// Server là trung tâm xử lí chính --> cần biết về các ketes nối từ client để
// có thể gửi dữ liệu đến đúng client đó
// Client gửi dữ liệu mạng qua phuowgn thứ trong tầng transport  được xung cấp trong Unity.networking.Transport

//Network connection beietur diễn kêt snoois giữa hai hay nhiều thiết bị

// nỗi khi kết nối được tạo (server to client or client to server ) thì nó sẽ tạo ra một NetworkConenction 


public static class NetUtility
{
    // Net messages
    // C_ => sự kiện gắn với loại tin nhắn mà client nhận từ server, sự kiện này được triển khai trong các chesboard
    public static Action<NetMessage> C_KEEP_ALIVE;
    public static Action<NetMessage> C_WELCOME;
    public static Action<NetMessage> C_START_GAME;
    public static Action<NetMessage> C_PIECE_SELECTED;
    public static Action<NetMessage> C_MAKE_MOVE;
    public static Action<NetMessage> C_VICTORY_CLAIM;
    public static Action<NetMessage> C_READY;
    public static Action<NetMessage> C_SWITCH_TEAM;
    public static Action<NetMessage> C_REMATCH;
    // S_ => sự kiện gắn với loại tin nhắn mà server  nhận từ client, sự kiện này được triển khai trong các chesboard
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
        var OpCode = (OpCode)streamReader.ReadByte(); // lãy mã từ đầu vào streamreader
        switch (OpCode)
        {
            case OpCode.KEEP_ALIVE:
                msg = new NetKeepAlive(streamReader); // trạng thái còn sống, còn hoạt động hay không. để biets là kết nối bị đống thì kịp thời ngắt
                break;

            case OpCode.WELCOME:
                msg = new NetWelcome(streamReader); // mesage welcome khi kết nối thành công
                break;

            case OpCode.START_GAME:
                msg = new NetStartGame(streamReader); //msg  bắt đầu game
                break;

            case OpCode.PIECE_SELECTED:
                msg = new NetPieceSelected(streamReader); //msg   khi có quân cờ được chọn
                break;

            case OpCode.MAKE_MOVE:
                msg = new NetMakeMove(streamReader); // msg di chuyển
                break;

            case OpCode.VICTORY_CLAIM:
                msg = new NetVictoryClaim(streamReader); //msg  thông báo chiến thắng
                break;

            case OpCode.READY:
                msg = new NetReady(streamReader); // msg người chơi trnajg thái game sẵn sàng, chwof đợi người chơi
                break;

            case OpCode.SWITCH_TEAM:
                msg = new NetSwitchTeam(streamReader); // msg đổi team
                break;

            case OpCode.REMATCH:
                msg = new NetRematch(streamReader); //msg  reload lại game
                break;

            default:
                Debug.LogError("Message received has no OpCode");
                break;
        }

        // xử lí bên NetMessage.cs
        if (server != null)
            msg.ReceivedOnServer(cnn); // nếu là máy chủ thì xử lí nhận phía máy chủ, có đối số cnn(network connection)
        else
            msg.ReceivedOnClient();
    }
}
