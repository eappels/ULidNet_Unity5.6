using Lidgren.Network;

public class NetworkClientInfo
{

    public readonly byte host_id = 0;
    public readonly NetConnection netconnection = null;

    public NetworkClientInfo(byte id, NetConnection netconnection)
    {
        host_id = id;
        this.netconnection = netconnection;
        this.netconnection.Tag = this;
    }
}