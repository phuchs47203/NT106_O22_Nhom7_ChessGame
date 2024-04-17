using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
// Unity Networking Transport hỗ trợ cả TCP và UDP,
// Tùy thuộc vào cách  cấu hình NetworkDriver.
// Mặc định, nó sử dụng UDP. Để sử dụng TCP thay vì UDP,  cần cấu hình NetworkDriver để sử dụng TCP thay vì UDP.

// keep alive được gửi liên tục, để đmả bảo ràng kết nối vẫn còn hoạt động, gói kêp alive không cần có dữ liệu, chỉ đẻ nhận beiets thôi
// Nếu một kêt snoois bị gián đonạ, có thhể kịp thời phát hiện và duy trì 

// thuộc Volumn MenuUIManager. Ngoài ra, Server và Client script cũng được add vào trong volumn MenuUI

public class Client : MonoBehaviour
{
    public static Client Singleton { get; private set; }

    private void Awake()
    {
        Singleton = this;
    }

    public NetworkDriver driver;
    private NetworkConnection connection;

    private bool isActive = false; // theo dõi trạng thái của client còn hoạt động hay không

    public Action connectionDropped;

    // Methods
    public void Init(string ip, ushort port)
    {
        this.driver = NetworkDriver.Create();
        NetworkEndPoint endPoint = NetworkEndPoint.Parse(ip, port); // ip và cổng được truyền từ MenuUIManager

        this.connection = this.driver.Connect(endPoint); // gán cho biết connection một kêt snoois mới đến máy chủ với endpoit vừa được tạo

        Debug.Log($"Attemping to connect to Server on {endPoint.Address}");

        this.isActive = true; 

        this.RegisterToEvent();
    }

    public void Shutdown()
    {
        if (this.isActive)
        {
            this.UnregisterToEvent();
            this.driver.Dispose();
            this.isActive = false;
            connection = default(NetworkConnection);
        }
    }

    public void OnDestroy()
    {
        this.Shutdown(); // khi client bị hủy thì shutdown
    }

    public void Update()
    {
        // nếu không active thì update làm gì
        if (!this.isActive) return;

        this.driver.ScheduleUpdate().Complete();
        this.CheckAlive();
        this.UpdateMessagePump();
    }

    private void CheckAlive()
    {
        // nếu không haojt động thì shutdown
        if (!this.connection.IsCreated && this.isActive)
        {
            Debug.Log("Something went wrong, lost connection to server!");
            this.connectionDropped?.Invoke();
            this.Shutdown();
        }
    }

    private void UpdateMessagePump()
    {
        DataStreamReader streamReader;
        NetworkEvent.Type cmd;

        // this.connection.PopEvent(this.driver, out streamReader) trả về sự kiện cho một kết nối cụ thế, kết quả có thể là data hoặc disconnect
        // trả về loại sự keienj .type, và luồng dữ liệu datastream  , nó trrer về type là empty => đã ngắt kết nối
        while ((cmd = this.connection.PopEvent(this.driver, out streamReader)) != NetworkEvent.Type.Empty)
        {
            switch (cmd)
            {
                case NetworkEvent.Type.Connect:
                    this.SendToServer(new NetWelcome()); // nếu mesage là connect thì gửi mesage welcome đén server , bắt đàu chơi game
                    Debug.Log("Connected");
                    break;

                case NetworkEvent.Type.Data:
                    NetUtility.OnData(streamReader, default(NetworkConnection)); // xử lí các sự keienj khi nó trả về data 
                    break;

                case NetworkEvent.Type.Disconnect:
                    Debug.Log("Client got disconnected from server");
                    this.connection = default(NetworkConnection);
                    this.connectionDropped?.Invoke();
                    this.Shutdown();
                    break;
            }
        }
    }

    public void SendToServer(NetMessage msg)
    {
        DataStreamWriter writer;
        this.driver.BeginSend(this.connection, out writer); // khởi tạo dữ liệu
        msg.Serialize(ref writer); // mã hóa dữ liệu được gửi
        this.driver.EndSend(writer); // đóng gói và gửi đi
        // đảm bảo tính nhất quán và an toàn khi gửi dữ liệu
    }

    #region Network Received
    private void RegisterToEvent()
    {
        NetUtility.C_KEEP_ALIVE += this.OnKeepAlive; // gán keep alive
    }

    private void UnregisterToEvent()
    {
        NetUtility.C_KEEP_ALIVE -= this.OnKeepAlive;
    }

    private void OnKeepAlive(NetMessage keepAliveMessage)
    {
        // Send it back, to keep both side alive
        // Gửi nó trở lại, để giữ cho cả hai bên sống sót
        this.SendToServer(keepAliveMessage); // gửi  keepp alive đến server
    }
    #endregion

}
