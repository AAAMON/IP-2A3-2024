using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class WebSocketManager
{
    private static readonly int maxClients = 6;
    private static readonly ConcurrentDictionary<string, WebSocket> clients = new ConcurrentDictionary<string, WebSocket>();

    public static int GetNumberOfClients()
    {
        int numberOfClients = clients.Count;
        return numberOfClients;
    }
    public async Task StartWebSocketServerAsync(string url)
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add(url);
        listener.Start();
        Console.WriteLine("WSMANAGER: WebSocket server started");

        try
        {
            while (clients.Count < maxClients)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    Console.WriteLine("WSMANAGER: Someone trying to enter");
                    ProcessWebSocketRequest(context);

                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WSMANAGER: Error: {ex.Message}");
        }
        finally
        {
            listener.Stop();
            Console.WriteLine("WSMANAGER: WebSocket server stopped");
        }
    }

    private async void ProcessWebSocketRequest(HttpListenerContext context)
    {
        HttpListenerWebSocketContext webSocketContext = null;
        try
        {
            webSocketContext = await context.AcceptWebSocketAsync(null);
            WebSocket socket = webSocketContext.WebSocket;

            // Receive username from client
            string username = await ReceiveStringAsync(socket);

            // Add client to dictionary
            clients.TryAdd(username, socket);

            Console.WriteLine($"WSMANAGER: Client connected: {username}");
            int nr = GetNumberOfClients();
            Console.WriteLine($"WSMANAGER: Nr of clients: {nr}");

            // Handle messages from the client
            await HandleClientMessages(socket, username);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WSMANAGER: Error: {ex.Message}");
            webSocketContext?.WebSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Internal Server Error", CancellationToken.None);
        }
    }

    private async Task HandleClientMessages(WebSocket socket, string username)
    {
        byte[] buffer = new byte[1024];
        WebSocketReceiveResult result;
        try
        {
            do
            {
                result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"WSMANAGER: Message received from {username}: {message}");

                    // Echo message back to the client (just an example)
                    await socket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    // Handle WebSocket close frame received
                    await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                }
            } while (!result.CloseStatus.HasValue);
        }
        catch (WebSocketException ex) when (ex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
        {
            // Client closed the WebSocket connection abruptly without completing the close handshake
            Console.WriteLine($"WSMANAGER: WebSocket connection closed abruptly by {username}");

            // Remove client from dictionary
            WebSocket removedSocket;
            clients.TryRemove(username, out removedSocket);

            Console.WriteLine($"WSMANAGER: Client disconnected: {username}");
            int nr = GetNumberOfClients();
            Console.WriteLine($"WSMANAGER: Nr of clients: {nr}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WSMANAGER: Error: {ex.Message}");
        }

    }

    private async Task<string> ReceiveStringAsync(WebSocket socket)
    {
        byte[] buffer = new byte[1024];
        WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        return Encoding.UTF8.GetString(buffer, 0, result.Count);
    }
}
