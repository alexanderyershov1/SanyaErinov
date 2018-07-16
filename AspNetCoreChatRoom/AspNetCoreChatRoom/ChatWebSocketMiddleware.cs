using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;

namespace AspNetCoreChatRoom
{
    public class ChatWebSocketMiddleware
    {
        //private static ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();
        static List<Client> _sockets = new List<Client>();
        int currentID = 1;

        class Client
        {
            public int ID { get; set; }
            public string StationID { get; set; }
            public WebSocket Socket { get; set; }
        }


        private readonly RequestDelegate _next;

        public ChatWebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next.Invoke(context);
                return;
            }

            CancellationToken ct = context.RequestAborted;
            WebSocket currentSocket = await context.WebSockets.AcceptWebSocketAsync();
            Client currentClient = new Client { ID = currentID++, Socket = currentSocket, StationID = context.Request.Query["stationID"] };
            Console.WriteLine("stationID " + currentClient.StationID);
            _sockets.Add(currentClient);

            while (true)
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }

                var response = await ReceiveStringAsync(currentSocket, ct);
                if (string.IsNullOrEmpty(response))
                {
                    if (currentSocket.State != WebSocketState.Open)
                    {
                        break;
                    }

                    continue;
                }

                if (response == "open")
                    continue;

                response = "User " + currentClient.ID + ": " + response + " " + DateTime.Now.ToLongTimeString();

                foreach (var socket in _sockets)
                {
                    if (socket.Socket.State != WebSocketState.Open)
                    {
                        continue;
                    }

                    //if (socket.StationID == "1")
                        await SendStringAsync(socket.Socket, response, ct);
                    //else await SendStringAsync(socket.Socket, "Вы не подписаны на станцию 1", ct);
                }
            }

            _sockets.Remove(currentClient);
            await currentClient.Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", ct);
            currentClient.Socket.Dispose();
        }
        
        private static Task SendStringAsync(WebSocket socket, string data, CancellationToken ct = default(CancellationToken))
        {
            var buffer = Encoding.UTF8.GetBytes(data);
            var segment = new ArraySegment<byte>(buffer);
            return socket.SendAsync(segment, WebSocketMessageType.Text, true, ct);
        }

        private static async Task<string> ReceiveStringAsync(WebSocket socket, CancellationToken ct = default(CancellationToken))
        {
            var buffer = new ArraySegment<byte>(new byte[8192]);
            using (var ms = new MemoryStream())
            {
                WebSocketReceiveResult result;
                do
                {
                    ct.ThrowIfCancellationRequested();

                    result = await socket.ReceiveAsync(buffer, ct);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                ms.Seek(0, SeekOrigin.Begin);
                if (result.MessageType != WebSocketMessageType.Text)
                {
                    return null;
                }

                // Encoding UTF8: https://tools.ietf.org/html/rfc6455#section-5.6
                using (var reader = new StreamReader(ms, Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }
    }
}
