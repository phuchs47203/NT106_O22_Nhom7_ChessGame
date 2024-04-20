using Unity.Networking.Transport;


// thiết lập lại game
public class NetRematch : NetMessage
{
    public NetRematch()
    {
        this.Code = OpCode.REMATCH;
    }

    public NetRematch(DataStreamReader reader)
    {
        this.Code = OpCode.REMATCH;
        this.Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);
    }

    public override void Deserialize(DataStreamReader reader)
    {
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_REMATCH?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_REMATCH?.Invoke(this, cnn);
    }
    private void Porcessing_date(NetworkConnection cnn, DataStreamReader rd)
    {

        this.Code = OpCode.REMATCH;
        DataStreamWriter wr = (new DataStreamWriter());
        base.Serialize(ref wr);
        base.ReceivedOnServer(cnn);

        wr.WriteInt((int)OpCode.REMATCH);
    }
}

