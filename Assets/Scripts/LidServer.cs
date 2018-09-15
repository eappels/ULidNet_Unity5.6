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
        Debug.Log("RPC_RequestObjects");
    }
}