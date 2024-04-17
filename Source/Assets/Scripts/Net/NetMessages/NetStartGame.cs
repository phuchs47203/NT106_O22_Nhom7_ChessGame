using Unity.Networking.Transport;

public class NetStartGame : NetMessage
{
    public NetStartGame()
    {
        this.Code = OpCode.START_GAME;
    }

    public NetStartGame(DataStreamReader reader)
    {
        this.Code = OpCode.START_GAME;
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

        NetUtility.C_START_GAME?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_START_GAME?.Invoke(this, cnn);
    }
    private void Porcessing_date(NetworkConnection cnn, DataStreamReader rd)
    {

        this.Code = OpCode.READY;
        DataStreamWriter wr = (new DataStreamWriter());
        base.Serialize(ref wr);
        base.ReceivedOnServer(cnn);

        wr.WriteInt((int)OpCode.READY);
    }
}
