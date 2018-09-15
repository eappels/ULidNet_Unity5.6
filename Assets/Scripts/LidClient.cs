using Lidgren.Network;
using System;
using UnityEngine;

public class LidClient : LidPeer
{

    public readonly NetClient netclient;
    public event Action<string> OnNetworkDebugMessage = null;
    public event Action Connected = null, Disconnected = null;


    public LidClient()
    : base()
    {
        instance = this;
        isserver = false;
        isclient = true;

        var config = CreateConfig();
        netpeer = netclient = new NetClient(config);
        netclient.Start();
    }


    public void StartClient()
    {
        netclient.Start();
    }

    public void StopClient()
    {
        netclient.Shutdown(string.Empty);
    }

    public void Connect(string connectionstring)
    {
        if (connectionstring.Contains(":"))
        {
            string[] tmpstringarray = connectionstring.Split(':');
            netclient.Connect(tmpstringarray[0], int.Parse(tmpstringarray[1]));
        }
        else
        {
            netclient.Connect(connectionstring, LidPeer.APPPORT);
        }
    }

    public void Disconnect()
    {
        netclient.Disconnect("Client disconnect");
    }

    protected override void OnStatusChanged(NetIncomingMessage nim)
    {
        switch (nim.SenderConnection.Status)
        {
            case NetConnectionStatus.Connected:
                if (Connected != null) Connected();
                break;
            case NetConnectionStatus.Disconnected:
                if (Disconnected != null) Disconnected();
                break;
        }
    }

    protected override void OnDataMessage(NetIncomingMessage nim)
    {
        switch (nim.ReadByte())
        {
            case LidPeer.REMOTE_CALL_FLAG:
                NetworkRemoteCallReceiver.ReceiveRemoteCall(nim);
                break;
            case LidPeer.ACTOR_EVENT_FLAG:
                //ReceiveObjectState(nim);
                break;
        }
    }

    protected override void OnDebugMessage(NetIncomingMessage nim)
    {
        if (OnNetworkDebugMessage != null) OnNetworkDebugMessage(nim.ReadString());
    }

    public void RPC_Hello(NetIncomingMessage nim, byte host_id)
    {
        this.host_id = host_id;
        NetworkRemoteCallSender.CallOnServer("RPC_RequestObjects");
    }

    public void RPC_Spawn(NetIncomingMessage msg, byte hostId, int objectId, string prefabName, Vector3 pos, Quaternion rot)
    {
        var game_object = (GameObject)GameObject.Instantiate(Resources.Load(prefabName), pos, rot);
        var net_actor = game_object.GetComponent<NetworkActor>();
        net_actor.host_id = hostId;
        net_actor.actor_id = objectId;
        net_actor.is_owner = LidPeer.instance.host_id == hostId;
        net_actor.prefab_name = prefabName;

        NetworkRemoteCallSender.CallOnServer("RPC_RequestObjectRegistration", net_actor.actor_id);
        NetworkActorRegistry.RegisterActor(net_actor);
    }
}