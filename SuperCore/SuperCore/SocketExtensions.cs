using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SuperCore
{
    public static class SocketExtensions
    {
        public static async Task<byte[]> ReadBytes(this Socket s, int byteCount)
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

        public static async Task SendBytes(this Socket s, byte[] data)
        {
            var sended = 0;
            while (sended < data.Length)
            {
                sended += await s.SendTaskAsync(data, sended, data.Length - sended, SocketFlags.None);
            }
        }

        public static Task ConnectTaskAsync(
            this Socket socket, IPAddress[] adresses, int port)
        {
            var tcs = new TaskCompletionSource<bool>(socket);
            socket.BeginConnect(adresses, port, iar =>
            {
                var t = (TaskCompletionSource<bool>)iar.AsyncState;
                var s = (Socket)t.Task.AsyncState;
                try
                {
                    s.EndConnect(iar);
                    t.TrySetResult(true);
                }
                catch (Exception exc) { t.TrySetException(exc); }
            }, tcs);
            return tcs.Task;
        }

        public static Task DisconnectTaskAsync(
            this Socket socket, bool reuseSocket)
        {
            var tcs = new TaskCompletionSource<bool>(socket);
            socket.BeginDisconnect(reuseSocket, iar =>
            {
                var t = (TaskCompletionSource<bool>)iar.AsyncState;
                var s = (Socket)t.Task.AsyncState;
                try
                {
                    s.EndDisconnect(iar);
                    t.TrySetResult(true);
                }
                catch (Exception exc) { t.TrySetException(exc); }
            }, tcs);
            return tcs.Task;
        }

        public static Task<int> ReceiveTaskAsync(
            this Socket socket, byte[] buffer, int offset, int size, SocketFlags socketFlags)
        {
            var tcs = new TaskCompletionSource<int>(socket);
            socket.BeginReceive(buffer, offset, size, socketFlags, iar =>
            {
                var t = (TaskCompletionSource<int>)iar.AsyncState;
                var s = (Socket)t.Task.AsyncState;
                try { t.TrySetResult(s.EndReceive(iar)); }
                catch (Exception exc) { t.TrySetException(exc); }
            }, tcs);
            return tcs.Task;
        }

        public static Task<int> SendTaskAsync(
            this Socket socket, byte[] buffer, int offset, int size, SocketFlags socketFlags)
        {
            var tcs = new TaskCompletionSource<int>(socket);
            socket.BeginSend(buffer, offset, size, socketFlags, iar =>
            {
                var t = (TaskCompletionSource<int>)iar.AsyncState;
                var s = (Socket)t.Task.AsyncState;
                try { t.TrySetResult(s.EndSend(iar)); }
                catch (Exception exc) { t.TrySetException(exc); }
            }, tcs);
            return tcs.Task;
        }
    }
}