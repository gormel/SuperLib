using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SuperCore
{
    public class SuperServer : SuperNet
    {
        JsonSerializer mSerializer = JsonSerializer.CreateDefault(new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All
        });
        private ConcurrentBag<Socket> mClients = new ConcurrentBag<Socket>(); 
        public void StartListen()
        {
            var serverSocket = new Socket(SocketType.Stream, ProtocolType.IPv4);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 666));
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

        private void StartReadClient(Socket client)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    var lenght = BitConverter.ToInt32(await ReadBytes(client, 4), 0);
                    var packageData = Encoding.UTF8.GetString(await ReadBytes(client, lenght));
                    //todo
                }
            });
        }

        private async Task<byte[]> ReadBytes(Socket s, int byteCount)
        {
            var recived = 0;
            var buffer = new byte[byteCount];
            while (recived < byteCount)
            {
                var nowRecived = await s.ReceiveTaskAsync(buffer, recived, byteCount - recived, SocketFlags.None);
                if (nowRecived == 0)
                    throw new IOException("Disconnected!");
                recived += nowRecived;
            }
            return buffer;
        }

        private async Task SendBytes(Socket s, byte[] data)
        {
            var sended = 0;
            while (sended < data.Length)
            {
                sended += await s.SendTaskAsync(data, sended, data.Length - sended, SocketFlags.None);
            }
        }

        protected async override void SendData(CallInfo info)
        {
            var stringBuilder = new StringBuilder();
            mSerializer.Serialize(new StringWriter(stringBuilder), info);
            var data = Encoding.UTF8.GetBytes(stringBuilder.ToString());
            await Task.WhenAll(mClients.Select(c => SendBytes(c, data)));
        }
    }
}
