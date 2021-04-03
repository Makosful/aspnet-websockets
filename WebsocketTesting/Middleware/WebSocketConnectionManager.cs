using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace WebsocketTesting.Middleware
{
    public class WebSocketConnectionManager
    {
        public SocketChannel LogChannel { get; set; } = new();
    }
}