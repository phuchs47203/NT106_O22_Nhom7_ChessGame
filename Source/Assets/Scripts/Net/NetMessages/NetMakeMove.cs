using Unity.Networking.Transport;

// biến enum đê xem di chuyển có tiêu diệt quân đối phương, hay là chỉ di chuyển thôi

public enum KillConfirm
{
    Kill,
    Move
}

public class NetMakeMove : NetMessage
{
    public int NextX { set; get; }
    public int NextY { set; get; }
    public KillConfirm killConfirm;

    // constructor khơi tạo di chuyển mới
    public NetMakeMove(int x, int y, KillConfirm killConfirm)
    {
        this.Code = OpCode.MAKE_MOVE;

        this.NextX = x;
        this.NextY = y;
        this.killConfirm = killConfirm; // đổi enum là kill
    }

    public NetMakeMove(DataStreamReader reader)
    {
        this.Code = OpCode.MAKE_MOVE;

        this.Deserialize(reader); // giả dữ liệu được mã hóa
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer); // shi chữ liệu của tin nhắn vào bộ ghi luồng

        writer.WriteInt(this.NextX);
        writer.WriteInt(this.NextY);
        writer.WriteInt((int)this.killConfirm);
    }

    // đọpc từ một luồng và gán các gía trị
    public override void Deserialize(DataStreamReader reader)
    {
        this.NextX = reader.ReadInt();
        this.NextY = reader.ReadInt();
        this.killConfirm = (KillConfirm)reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_MAKE_MOVE?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_MAKE_MOVE?.Invoke(this, cnn);
    }
}
