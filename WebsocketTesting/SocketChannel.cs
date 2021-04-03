using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebsocketTesting
{
    public class SocketChannel
    {
        private readonly ConcurrentDictionary<string, WebSocket> _sockets = new();
        
        public string AddSocket(WebSocket socket)
        {
            var connectionId = Guid.NewGuid().ToString();
            _sockets.TryAdd(connectionId, socket);
            return connectionId;
        }
        
        public void RemoveSocket(string guid)
        {
            _sockets.TryRemove(guid, out _);
        }

        public ConcurrentDictionary<string, WebSocket> GetSockets()
        {
            return _sockets;
        }

        public async Task PublishAsync(string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);

            foreach (var socket in _sockets)
            {
                await socket.Value.SendAsync(buffer: buffer, 
                    messageType: WebSocketMessageType.Text, 
                    endOfMessage: true, 
                    cancellationToken: CancellationToken.None);
            }
        }

        public async Task PublishAsync(string guid, string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);

            bool success = _sockets.TryGetValue(guid, out WebSocket socket);
            if (!success) return;
            
            await socket.SendAsync(buffer: buffer,
                messageType: WebSocketMessageType.Text,
                endOfMessage: true,
                cancellationToken: CancellationToken.None);
        }
    }
}