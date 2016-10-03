using SuperCore.Async;
using SuperCore.Async.SyncContext;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SuperCore.Core
{
    public class SuperServer : SuperNet
    {
        private class Client
        {
            public Socket Socket { get; }
            public AsyncLock SendLock { get; } = new AsyncLock();

            public bool IsConnected => Socket.Connected;

            public Client(Socket socket)
            {
                Socket = socket;
            }
        }

        private readonly ConcurrentBag<Client> mClients = new ConcurrentBag<Client>();
        
        public SuperServer(SuperSyncContext context = null)
            : base(context)
        {
        }

        public async Task Listen(int port, CancellationToken stop = default(CancellationToken))
        {
            var serverSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            serverSocket.Listen(100);

            stop.Register(() => serverSocket.Close());
            
            while (true)
            {
                stop.ThrowIfCancellationRequested();

                Socket client = await Task.Factory.FromAsync(
                    serverSocket.BeginAccept,
                    new Func<IAsyncResult, Socket>(serverSocket.EndAccept), null);

                mClients.Add(new Client(client));

                var yieldedRead = ReadClient(client, stop).ContinueWith(
                    t => { Trace.TraceWarning(t.Exception.ToString()); },
                    stop,
                    TaskContinuationOptions.OnlyOnFaulted,
                    TaskScheduler.Default);
            }
        }
        
        internal override async void SendData(object info)
        {
            byte[] data = GetBytes(info);
            
            await Task.WhenAll(mClients.Select(async c =>
            {
                using (await c.SendLock.Lock())
                {
                    if (!c.IsConnected)
                        return;
                    await SendByteArray(c.Socket, data);
                }
            }));
        }

        protected override void ClientDisconnected(Socket client)
        {
            
        }
    }
}
