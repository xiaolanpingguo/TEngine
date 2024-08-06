using System;
using System.Collections.Generic;
using Google.Protobuf;
using kcp2k;
using System.Net;
using C2DS;
using TEngine;
using Log = TEngine.Log;


namespace Lockstep.Game
{
    public static class KCPLogHelper
    {
        public static void Info(string str)
        {
            TEngine.Log.Info(str);
        }

        public static void Warning(string str)
        {
            TEngine.Log.Warning(str);
        }

        public static void Error(string str)
        {
            TEngine.Log.Error(str);
        }
    }

    public class NetworkModule
    {
        public bool IsConnected { get; private set; }
        private KcpClient m_network = null!;
        private ClientMessageDispatcher m_messageDispatcher = null!;
        private IPEndPoint m_endPoint;

        public NetworkModule(string ip, ushort port)
        {
            m_endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        }

        public bool Init()
        {
            kcp2k.Log.Info = KCPLogHelper.Info;
            kcp2k.Log.Warning = KCPLogHelper.Warning;
            kcp2k.Log.Error = KCPLogHelper.Error;
            KcpConfig config = new KcpConfig(
               NoDelay: true,
               DualMode: false,
               Interval: 1,
               Timeout: 10000,
               SendWindowSize: Kcp.WND_SND * 1000,
               ReceiveWindowSize: Kcp.WND_RCV * 1000,
               CongestionWindow: false,
               MaxRetransmits: Kcp.DEADLINK * 2
            );

            IsConnected = false;
            m_network = new KcpClient(OnConnected, OnDataReceived, OnDisconnect, OnError, config);
            m_network.Connect(m_endPoint.Address.ToString(), (ushort)m_endPoint.Port);

            m_messageDispatcher = new ClientMessageDispatcher(m_network);
            Log.Info($"client is connecting to server, ip:{m_endPoint}");
            return true;
        }

        public bool Update(long timeNow)
        {
            m_network.Tick();
            m_messageDispatcher.Update(timeNow);
            return true;
        }

        public bool Shutdown()
        {
            return true;
        }

        public bool RegisterMessage(ushort messageId, Type type, Action<ushort, int, IMessage> handler = null)
        {
            if (!m_messageDispatcher.IsMessageRegistered(messageId))
            {
                return m_messageDispatcher.RegisterMessage(messageId, type, handler);
            }

            return false;
        }

        public void Send(IMessage message, ushort msgId)
        {
            m_messageDispatcher.Send(message, msgId);
        }

        private void OnConnected()
        {
            Log.Info($"a client has connected to dedicated server.");
            IsConnected = true;
        }

        private void OnDataReceived(ArraySegment<byte> data, KcpChannel channel)
        {
            m_messageDispatcher.DispatchMessage(data, channel);
        }

        private void OnDisconnect()
        {
            Log.Warning($"client has disconnected to server.");
            IsConnected = false;
        }

        private void OnError(ErrorCode ec, string reason)
        {
            Log.Info($"a server error has occurred, error:{ec}, reason:{reason}");
        }
    }
}
