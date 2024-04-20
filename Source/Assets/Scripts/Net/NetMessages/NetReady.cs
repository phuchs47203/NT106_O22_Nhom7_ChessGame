using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

// tương tự như các sự kiện kia
// gửi message là team nào đã sẵn sàng
// ex: team red sẵn sàng, đag chờ người ta chơi với nos
public class NetReady : NetMessage
{
    public Team ReadyTeam { set; get; }
    public NetReady(Team readyTeam)
    {
        this.Code = OpCode.READY;
        this.ReadyTeam = readyTeam;
    }

    public NetReady(DataStreamReader reader)
    {
        this.Code = OpCode.READY;
        this.Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteInt((int)this.ReadyTeam);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        this.ReadyTeam = (Team)reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_READY?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_READY?.Invoke(this, cnn);
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
