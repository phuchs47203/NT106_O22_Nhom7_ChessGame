using System;
using Unity.Networking.Transport;

public class NetKeepAlive : NetMessage
{


    public NetKeepAlive()
    {
        this.Code = OpCode.KEEP_ALIVE;// gán mã code của keep alive
    }
    public NetKeepAlive(DataStreamReader reader)
    {
        this.Code = OpCode.KEEP_ALIVE;
        this.Deserialize(reader); // gọi phương thwucs giả mã dữ liệu nhận được
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        // chuyển dữ liệu thành byte để truyền đi
        base.Serialize(ref writer);  // gọi serialize của NetMessage
    }

    public override void Deserialize(DataStreamReader reader)
    {

    }

    // gọi khi tin nhắn nhận từ phía client
    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_KEEP_ALIVE?.Invoke(this);
    }

    // gọi khi tin nhắn nhận từ phía server
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);
        // kiểm tra xem delegate có null hay không trước khi truyền đối số cho biến Actrion đó
        NetUtility.S_KEEP_ALIVE?.Invoke(this, cnn);

    }
    // Trong NetUnity
    // public static Action<NetMessage, NetworkConnection> S_KEEP_ALIVE;
    //Action<NetMessage, NetworkConnection> S_KEEP_ALIVE; là biến delegate chứa 1 hoặc nhiều hàm hoặc phương thức

}
