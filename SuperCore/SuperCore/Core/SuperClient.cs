using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SuperCore.Core
{
    public class SuperClient : SuperNet
    {
        private Socket mClient;

        public Task Connect(string ip, int port, CancellationToken stop = default(CancellationToken))
        {
            mClient = new Socket(SocketType.Stream, ProtocolType.Tcp);
            mClient.Connect(ip, port);
            return ReadClient(mClient, stop);
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
