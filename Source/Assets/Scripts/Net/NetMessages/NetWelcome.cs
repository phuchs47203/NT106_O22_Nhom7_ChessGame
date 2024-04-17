using Unity.Networking.Transport;

// dùng trong local game, nhấp vòa thì ra màn hình welcom, gửi mesage là team anof đã vào màn hình  welcom
public class NetWelcome : NetMessage
{
    public Team AssignedTeam { set; get; }
    public NetWelcome()
    {
        this.Code = OpCode.WELCOME;
    }

    public NetWelcome(DataStreamReader reader)
    {
        this.Code = OpCode.WELCOME;
        this.Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteInt((int)this.AssignedTeam);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        this.AssignedTeam = (Team)reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_WELCOME?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_WELCOME?.Invoke(this, cnn);
    }
    private void Porcessing_date(NetworkConnection cnn, DataStreamReader rd)
    {

        this.Code = OpCode.WELCOME;
        DataStreamWriter wr = (new DataStreamWriter());
        base.Serialize(ref wr);
        base.ReceivedOnServer(cnn);

        wr.WriteInt((int)OpCode.WELCOME);
    }
}
