// See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");


using System;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

public class ClientAI
{
    
    private readonly string baseUrl = "http://localhost:8000/";
    private readonly string getMoveBodyUrl = "http://localhost:8000/get-move-body";
    private readonly string getMoveHeadUrl = "http://localhost:8000/get-move-header";
    static readonly HttpClient client = new HttpClient();



    public async Task<string> getMoveBody( string gameState )
    {
        var content = new StringContent(gameState);
        HttpResponseMessage response = await client.PostAsync( getMoveBodyUrl , content );
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        return responseBody;
    }


    public async Task<string> getMoveHead(string gameState)
    {
        var content = new StringContent(gameState);
        HttpResponseMessage response = await client.PostAsync(getMoveHeadUrl, content);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        return responseBody;
    }


}

