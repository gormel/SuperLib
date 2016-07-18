using System;
using System.Net.Sockets;

namespace SuperCore.Core
{
    public class SuperClient : SuperNet
    {
        private Socket mClient;

        public void Connect(string ip, int port)
        {
            mClient = new Socket(SocketType.Stream, ProtocolType.Tcp);
            mClient.Connect(ip, port);
            StartReadClient(mClient);
        }
        
        internal async override void SendData(object info)
        {
            if (mClient == null)
            {
                return;
            }
            
            var data = GetBytes(info);
            await mClient.SendBytes(BitConverter.GetBytes(data.Length));
            await mClient.SendBytes(data);
        }

        protected override void ClientDisconnected(Socket client)
        {
            mClient = null;
        }
    }
}
