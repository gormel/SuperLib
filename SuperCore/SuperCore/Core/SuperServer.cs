using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SuperCore.Core
{
    public class SuperServer : SuperNet
    {
        private readonly ConcurrentBag<Socket> mClients = new ConcurrentBag<Socket>(); 
        public void StartListen(int port)
        {
            var serverSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            serverSocket.Listen(100);
            Task.Run(new Action(async () =>
            {
                while (true)
                {
                    var client = await Task.Factory.FromAsync(serverSocket.BeginAccept, new Func<IAsyncResult, Socket>(serverSocket.EndAccept), null);
                    mClients.Add(client);
                    StartReadClient(client);
                }
            }));
        }
        
        internal async override void SendData(object info)
        {
            var data = GetBytes(info);
            
            await Task.WhenAll(mClients.Select(async c =>
            {
                await c.SendBytes(BitConverter.GetBytes(data.Length));
                await c.SendBytes(data);
            }));
        }
    }
}
