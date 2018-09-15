using Lidgren.Network;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LidServer : LidPeer
{

    public readonly NetServer netserver;
    public event Action<string> OnNetworkDebugMessage = null;
    public readonly HashSet<NetworkClientInfo> connected_clients;
    private byte host_id_counter;
    private int actor_id_counter;

    public LidServer()
    : base()
    {
        instance = this;
        isserver = true;
        isclient = false;

        host_id_counter = 0;

        var config = CreateConfig();
        config.Port = APPPORT;
        netpeer = netserver = new NetServer(config);

        connected_clients = new HashSet<NetworkClientInfo>();
    }

    public void StartServer()
    {
        netserver.Start();
    }

    public void StopServer()
    {
        netserver.Shutdown(string.Empty);
    }

    private NetworkClientInfo GetClientInfo(NetIncomingMessage nim)
    {
        var netconnection = nim.SenderConnection;
        if (netconnection.Tag == null) netconnection.Tag = new NetworkClientInfo(++host_id_counter, netconnection);
        return ((NetworkClientInfo)netconnection.Tag);
    }

    protected override void OnStatusChanged(NetIncomingMessage nim)
    {
        switch (nim.SenderConnection.Status)
        {
            case NetConnectionStatus.Connected:
                var newclient = GetClientInfo(nim);
                connected_clients.Add(newclient);
                NetworkRemoteCallSender.CallOnClient(newclient, "RPC_Hello", newclient.host_id);
                break;
            case NetConnectionStatus.Disconnected:
                break;
        }
    }

    protected override void OnDataMessage(NetIncomingMessage nim)
    {
        while (nim.Position < nim.LengthBits)
        {
            switch (nim.ReadByte())
            {
                case LidPeer.REMOTE_CALL_FLAG:
                    NetworkRemoteCallReceiver.ReceiveRemoteCall(nim);
                    break;
                case LidPeer.USER_COMMAND_FLAG:
                    //ReceiveUserCommand(nim);
                    break;
            }
        }
    }

    protected override void OnDebugMessage(NetIncomingMessage nim)
    {
        if (OnNetworkDebugMessage != null) OnNetworkDebugMessage(nim.ReadString());
    }

    public void RPC_RequestObjects(NetIncomingMessage msg)
    {
        var client = GetClientInfo(msg);

        foreach (var obj in NetworkActorRegistry.Objects)
        {
            if (obj != null)
            {
                NetworkRemoteCallSender.CallOnClient(
                    client, "RPC_Spawn",
                    obj.host_id, obj.actor_id, obj.prefab_name,
                    obj.transform.position, obj.transform.rotation
                );
            }
        }
    }

    public void RPC_RequestObjectRegistration(NetIncomingMessage msg, int actor_id)
    {
        var client = GetClientInfo(msg);
        var obj = NetworkActorRegistry.GetById(actor_id);
        client.proximity_set.Add(obj);
    }

    public void RPC_RequestSpawn(NetIncomingMessage msg, string prefab_name, Vector3 pos, Quaternion rot)
    {
        var client_info = GetClientInfo(msg);
        var game_object = (GameObject)GameObject.Instantiate(Resources.Load(prefab_name), pos, rot);
        var obj = game_object.GetComponent<NetworkActor>();

        obj.host_id = client_info.host_id;
        obj.actor_id = actor_id_counter++;
        obj.is_owner = false;
        obj.owner = client_info;
        obj.prefab_name = prefab_name;

        client_info.has_spawned = true;

        NetworkRemoteCallSender.CallOnAllClients(
            "RPC_Spawn",
            obj.host_id, obj.actor_id, obj.prefab_name,
            obj.transform.position, obj.transform.rotation
        );
    }
}