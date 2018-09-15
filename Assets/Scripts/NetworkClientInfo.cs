using Lidgren.Network;
using System.Collections.Generic;

public class NetworkClientInfo
{

    public bool has_spawned;
    public readonly byte host_id = 0;
    public readonly NetConnection netconnection = null;
    public readonly HashSet<NetworkActor> proximity_set = new HashSet<NetworkActor>();

    public NetworkClientInfo(byte id, NetConnection netconnection)
    {
        host_id = id;
        this.netconnection = netconnection;
        this.netconnection.Tag = this;
    }
}