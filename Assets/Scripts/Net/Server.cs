﻿using System;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class Server : MonoBehaviour
{
    public static Server Singleton { get; private set; }
    private void Awake()
    {
        Singleton = this;
    }

    public NetworkDriver driver; // đối tượng đưuọc khởi tạo để quản lí một kết nối mạng
    private NativeList<NetworkConnection> connections; // lưu trữ các dnah sách kêt nối mạng

    private bool isActive = false; // keiemr tra coi server còn hoạt động hay không
    private float keepAliveTickRate = 20f; // tần suawts gửi goi keep alive
    private float lastKeepAlive; // lần gửi gói keep alive gần nhất

    public Action connectionDropped; // gọi sự keienj khi kết nối mạng bị ngắt

    // Methods
    // khởi tạo và lắng nghe từ client
    public void Init(ushort port)
    {
        this.driver = NetworkDriver.Create();
        // khởi tạo chấp nhận bất kì địa chỉ ipv4, cùng mạng. Thiêt lập cổng , lằng nghe với mọi địa chỉ Ip
        NetworkEndPoint endPoint = NetworkEndPoint.AnyIpv4;
        endPoint.Port = port;

        if (this.driver.Bind(endPoint) != 0)
        {
            Debug.Log($"Unable to bind to port {endPoint.Port}");
            return;
        }
        else
        {
            this.driver.Listen();
            Debug.Log($"Currently listening on port {endPoint.Port}");
        }

        this.connections = new NativeList<NetworkConnection>(2, Allocator.Persistent);
        this.isActive = true;
    }

    //khi server đóng thì giải phóng tìa nguyên, xóa các dnah sách kết nối
    public void Shutdown()
    {
        if (this.isActive)
        {
            this.driver.Dispose();
            this.connections.Dispose();
            this.isActive = false;
        }
    }

    public void OnDestroy()
    {
        this.Shutdown();
    }

    // cập nhật liên tục
    // duy rtif kết nốt
    // cháp nahanj ekest nối
    // xử lí các thông điệp từ các kết nối hiện tại
    public void Update()
    {
        if (!this.isActive) return;

        this.KeepAlive();

        this.driver.ScheduleUpdate().Complete();

        this.CleanupConnections();
        this.AcceptNewConnections();
        this.UpdateMessagePump();
    }

    // duy trì kết nối, đảm bảo không bị gián đoạn, tần số gửi keep alive pahri nahats định đe duy trì kết nối, dùng tỏng việc két nối liên tục
    private void KeepAlive()
    {
        if (Time.time - this.lastKeepAlive > this.keepAliveTickRate)
        {
            this.lastKeepAlive = Time.time; // cập nhật lastkepp alive
            this.BroadCast(new NetKeepAlive()); // gửi đến mọi máy trong mạng
        }
    }

    // loại bro các kêt nối từ dnah sách
    private void CleanupConnections()
    {
        for (int i = 0; i < this.connections.Length; i++)
        {
            if (!this.connections[i].IsCreated)
            {
                this.connections.RemoveAtSwapBack(i);
                --i;
            }
        }
    }

    // khi có client yêu cầu eket snoois thì cái này sẽ chấp nhận kết nối đó và add vào list
    private void AcceptNewConnections()
    {
        NetworkConnection c;
        while ((c = this.driver.Accept()) != default(NetworkConnection))
        {
            this.connections.Add(c);
        }
    }

    private void UpdateMessagePump()
    {
        DataStreamReader streamReader;

        // lặp qua atats cả các kết nối
        for (int i = 0; i < this.connections.Length; i++)
        {
            NetworkEvent.Type cmd;

            // lấy sự keienj mạng tiếp theo cho kết nối hiện tại
            // đọc dữ liệu vào stream reader
            while ((cmd = this.driver.PopEventForConnection(this.connections[i], out streamReader)) != NetworkEvent.Type.Empty)
            {
                switch (cmd)
                {
                    case NetworkEvent.Type.Data:
                        NetUtility.OnData(streamReader, this.connections[i], this); // gọi hàm Ondata trong class netUnity
                        break;

                    case NetworkEvent.Type.Disconnect: // 1 trong 2 người chơi ngắt kết nối thì shutdown server 
                        Debug.Log("Client disconnected from the server");
                        this.connections[i] = default(NetworkConnection);
                        this.connectionDropped?.Invoke();

                        /*
                         *  Shut down the server when 1 of the 2 players disconnect
                         *  Because this is a chess game. It needs 2 people to play
                         */

                        this.Shutdown();
                        break;
                }
            }
        }
    }
    

    private void Process_Netwotk_Events(NetworkEvent.Type eventType, DataStreamReader streamReader, NetworkConnection connection)
    {
        switch (eventType)
        {
            case NetworkEvent.Type.Data:
                NetUtility.OnData(streamReader, connection, this); // Gọi hàm OnData trong class NetUtility
                break;

            case NetworkEvent.Type.Disconnect:
                Handle_Disconnected(connection);
                break;
        }
    }

    private void Handle_Disconnected(NetworkConnection connection)
    {
        Debug.Log("Client disconnected from the server");
        int i = 0;
        // Xóa kết nối khỏi danh sách
        this.connections[i] = default(NetworkConnection);
        // Gọi sự kiện khi có kết nối bị ngắt
        this.connectionDropped?.Invoke();

        this.Shutdown();
    }
    private void UpdateMess()
    {
        DataStreamReader streamReader;

        // Lặp qua tất cả các kết nối
        for (int i = 0; i < this.connections.Length; i++)
        {
            NetworkEvent.Type cmd;

            // Lấy sự kiện mạng tiếp theo cho kết nối hiện tại
            while ((cmd = this.driver.PopEventForConnection(this.connections[i], out streamReader)) != NetworkEvent.Type.Empty)
            {
                Process_Netwotk_Events(cmd, streamReader, this.connections[i]);
            }
        }
    }
    // Server specific
    // gửi thông điệp tới một client cụ thể
    public void SendToClient(NetworkConnection connection, NetMessage msg)
    {
        DataStreamWriter writer;
        this.driver.BeginSend(connection, out writer);
        msg.Serialize(ref writer);
        this.driver.EndSend(writer);
    }

    // gửi thông điệp tới tấ cả các client dndag kết nối
    public void BroadCast(NetMessage msg)
    {
        for (int i = 0; i < this.connections.Length; i++)
        {
            if (this.connections[i].IsCreated)
            {
                Debug.Log($"Sending {msg.Code} to: {this.connections[i].InternalId}");
                this.SendToClient(this.connections[i], msg);
            }
        }
    }
    public void Handle_Move_Request(NetworkConnection connection, int pieceId, Vector2Int targetPosition, NetMessage msg)
    {
        // Kiểm tra tính hợp lệ của nước đi
        bool isValidMove = Check__Validity(pieceId, targetPosition);

        if (isValidMove)
        {
            // Cập nhật trạng thái bàn cờ
            UpdateBoard_State(pieceId, targetPosition);

            
        }
        else
        {
            // Gửi thông báo về việc nước đi không hợp lệ cho client gửi yêu cầu
            SendToClient(connection, msg);
        }
    }
   
    private bool Check__Validity(int pieceId, Vector2Int targetPosition)
    {
        throw new NotImplementedException();
    }

    private void UpdateBoard_State(int pieceId, Vector2Int targetPosition)
    {
        throw new NotImplementedException();
    }
}
