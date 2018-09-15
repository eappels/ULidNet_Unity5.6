﻿using Lidgren.Network;

public abstract class LidPeer
{

    public NetPeer netpeer;
    public static LidPeer instance;
    public static bool isserver, isclient;
    public const string APPID = "MyGame";
    public const int APPPORT = 25000;

    public LidPeer()
    : base()
    {
    }

    public void MessagePump()
    {
        NetIncomingMessage nim;
        while ((nim = netpeer.ReadMessage()) != null)
        {
            switch (nim.MessageType)
            {
                case NetIncomingMessageType.Data:
                    OnDataMessage(nim);
                    break;
                case NetIncomingMessageType.StatusChanged:
                    OnStatusChanged(nim);
                    break;
                case NetIncomingMessageType.VerboseDebugMessage:
                case NetIncomingMessageType.DebugMessage:
                case NetIncomingMessageType.WarningMessage:
                case NetIncomingMessageType.ErrorMessage:
                case NetIncomingMessageType.Error:
                    OnDebugMessage(nim);
                    break;
            }
            netpeer.Recycle(nim);
        }
    }

    public NetPeerConfiguration CreateConfig()
    {
        return new NetPeerConfiguration(APPID);
    }

    public virtual NetOutgoingMessage CreateMessage()
    {
        return netpeer.CreateMessage();
    }

    protected virtual void OnDataMessage(NetIncomingMessage nim)
    {
    }

    protected virtual void OnStatusChanged(NetIncomingMessage nim)
    {
    }

    protected virtual void OnDebugMessage(NetIncomingMessage nim)
    {
    }
}