using Unity.Networking.Transport;
using UnityEngine;


// lớp trừu tượng, ảo để cho những lớp khác xài
public class NetMessage
{
    // OpCode là mã enum định nghĩa trong Netunity
    public OpCode Code { set; get; }

    //chuyển message thành byte để truyền đi
    public virtual void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)this.Code);
    }

    // dùng cho bên nhận thì deserialize 
    public virtual void Deserialize(DataStreamReader reader)
    {

    }

    public virtual void ReceivedOnClient()
    {

    }
    public virtual void ReceivedOnServer(NetworkConnection cnn)
    {

    }
    protected virtual void Porcessing_Data(NetworkConnection cnn, DataStreamReader red)
    {

    }
}
