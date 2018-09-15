using Lidgren.Network;
using System;

public class LidServer : LidPeer
{

    public readonly NetServer netserver;
    public event Action<string> OnNetworkDebugMessage = null;

    public LidServer()
    : base()
    {
        instance = this;
        isserver = true;
        isclient = false;

        var config = CreateConfig();
        config.Port = APPPORT;
        netpeer = netserver = new NetServer(config);
    }

    public void StartServer()
    {
        netserver.Start();
    }

    public void StopServer()
    {
        netserver.Shutdown(string.Empty);
    }

    protected override void OnDebugMessage(NetIncomingMessage nim)
    {
        if (OnNetworkDebugMessage != null) OnNetworkDebugMessage(nim.ReadString());
    }
}