using System;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Server;

public class WebSocketServerManager
{
    private WebSocketServer wss;
    private Dictionary<string, WebSocket> clients;

    public bool IsRunning { get; set; }

    public WebSocketServerManager(string url)
    {
        wss = new WebSocketServer(url);
        clients = new Dictionary<string, WebSocket>();
    }

    public void Start()
    {
        wss.Start();
        IsRunning = true;
        Console.WriteLine("WebSocket server started...");
    }

    public void Stop()
    {
        wss.Stop();
        IsRunning = false;
        Console.WriteLine("WebSocket server stopped...");
    }

    public void AddClient(string clientId, WebSocket socket)
    {
        lock (clients)
        {
            if (!clients.ContainsKey(clientId))
            {
                clients.Add(clientId, socket);
                Console.WriteLine($"Client {clientId} connected.");
            }
        }
    }

    public void RemoveClient(string clientId)
    {
        lock (clients)
        {
            if (clients.ContainsKey(clientId))
            {
                clients.Remove(clientId);
                Console.WriteLine($"Client {clientId} disconnected.");
            }
        }
    }

    public int GetConnectedPlayerCount()
    {
        lock (clients)
        {
            return clients.Count;
        }
    }
}

public class GameWebSocketBehavior : WebSocketBehavior
{
    private WebSocketServerManager serverManager;

    public GameWebSocketBehavior(WebSocketServerManager manager)
    {
        serverManager = manager;
    }

    protected override void OnOpen()
    {
        var clientId = ID;
        serverManager.AddClient(clientId, Context.WebSocket);
    }

    protected override void OnMessage(MessageEventArgs e)
    {
        var message = e.Data;
        Console.WriteLine($"Message received from client: {message}");
    }

    protected override void OnClose(CloseEventArgs e)
    {
        var clientId = ID;
        if (serverManager != null)
        {
            serverManager.RemoveClient(clientId);
        }
    }
}
