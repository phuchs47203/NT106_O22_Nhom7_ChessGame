using Unity.Networking.Transport;

public class NetPieceSelected : NetMessage
{
    public int currentX { set; get; }
    public int currentY { set; get; }
    public NetPieceSelected(int x, int y)
    {
        this.Code = OpCode.PIECE_SELECTED;
        this.currentX = x;
        this.currentY = y;
    }

    public NetPieceSelected(DataStreamReader reader)
    {
        this.Code = OpCode.PIECE_SELECTED;
        this.Deserialize(reader);
    }

    // ghi dữ liệu vào luồng mạng đó, sau đó gửi nó đi
    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteInt(this.currentX);
        writer.WriteInt(this.currentY);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        this.currentX = reader.ReadInt();
        this.currentY = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_PIECE_SELECTED?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_PIECE_SELECTED?.Invoke(this, cnn);
    }
    public void Porcessing(NetworkConnection cnn, DataStreamReader rd)
    {
        this.currentY = rd.ReadInt();
        this.currentX = rd.ReadInt();
        this.Code = OpCode.PIECE_SELECTED;
        DataStreamWriter wr = (new DataStreamWriter());
        base.Serialize(ref wr);

        wr.WriteInt(this.currentX);
        wr.WriteInt(this.currentX);
        wr.WriteInt((int)OpCode.PIECE_SELECTED);
    }
}
