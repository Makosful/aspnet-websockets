using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebsocketTesting.Middleware
{
    public class WebSocketServerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly WebSocketConnectionManager _manager;

        public WebSocketServerMiddleware(RequestDelegate next, WebSocketConnectionManager manager)
        {
            _next = next;
            _manager = manager;
        }

        /// <summary>
        /// Method called by UseMiddleware in Startup.Configure
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                WebSocket socket = await context.WebSockets.AcceptWebSocketAsync();
                string connectionId = _manager.LogChannel.AddSocket(socket);
                
                Console.WriteLine("Connected");
                ConcurrentDictionary<string,WebSocket> sockets = _manager.LogChannel.GetSockets();
                foreach (var keyValuePair in sockets)
                {
                    Console.WriteLine(keyValuePair.Key);
                }

                await Receive(socket, async (result, buffer) =>
                {
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, result.Count));
                        return;
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine("Disconnecting");
                        _manager.LogChannel.RemoveSocket(connectionId);
                        ConcurrentDictionary<string,WebSocket> sockets = _manager.LogChannel.GetSockets();
                        foreach (var keyValuePair in sockets)
                        {
                            Console.WriteLine(keyValuePair.Key);
                        }
                        return;
                    }
                });
            }
            else
            {
                await _next(context);
            }
        }

        private static async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await socket.ReceiveAsync(
                    buffer: new ArraySegment<byte>(buffer),
                    cancellationToken: CancellationToken.None);

                handleMessage(result, buffer);
            }
        }
    }
}