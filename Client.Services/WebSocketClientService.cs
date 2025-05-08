using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace ToDoListBlazor.Client.Services
{
    public class WebSocketClientService : IAsyncDisposable
    {
        private readonly ClientWebSocket _webSocket;
        public event Action<string>? OnMessageReceived;

        public WebSocketClientService()
        {
            _webSocket = new ClientWebSocket();
        }

        public async Task ConnectAsync(string url)
        {
            if (_webSocket.State != WebSocketState.Open)
            {
                await _webSocket.ConnectAsync(new Uri(url), CancellationToken.None);
                _ = ListenAsync(); // «апускаем прослушивание сообщений
            }
        }

        private async Task ListenAsync()
        {
            var buffer = new byte[1024 * 4];
            while (_webSocket.State == WebSocketState.Open)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    OnMessageReceived?.Invoke(message);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
                    break;
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_webSocket.State == WebSocketState.Open)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
            }
            _webSocket.Dispose();
        }
    }
}