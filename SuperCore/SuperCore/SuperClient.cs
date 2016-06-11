using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SuperCore
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
        
        protected async override void SendData(CallInfo info)
        {
            if (mClient == null)
            {
                return;
            }
            
            var data = GetBytes(info);
            await mClient.SendBytes(BitConverter.GetBytes(data.Length));
            await mClient.SendBytes(data);
        }
    }
}
