using System;
using System.Net;
using System.Threading.Tasks;
using dune_library.Utils;

namespace dune_library.server
{
    public class NetworkInputProvider : I_Input_Provider, IDisposable
    {
        private readonly int port;
        private HttpListener listener;

        public NetworkInputProvider(int port)
        {
            this.port = port;
        }

        public async Task<string> GetInputAsync()
        {
            Console.WriteLine("Game has made a input request, waiting from the network");
            EnsureListenerCreated();

            try
            {
                HttpListenerContext context = await listener.GetContextAsync();
                HttpListenerRequest request = context.Request;

                Console.WriteLine($"Received a {request.HttpMethod} request! at {request.Url.AbsolutePath}");

                string requestData;

                if (request.HttpMethod == "GET" && request.Url.AbsolutePath.StartsWith("/gamestate"))   //cerere catre api pentru un gamestate deci o trimit mai departe
                {
                    requestData = request.Url.AbsolutePath;
                }
                else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/gamestate") //aici banuiesc ca se primeste un gamestate care trebuie vallidat(eventual o mutare)
                {
                    using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        requestData = await reader.ReadToEndAsync();
                    }
                }
                else if (request.Url.AbsolutePath.Contains("phase")) //daca primeste input de la gui il trimit mai departe la API
                {
                    using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        requestData = await reader.ReadToEndAsync();
                    }
                }
                context.Response.OutputStream.Close();
                //idk it didnt work
                //return requestData;
                //am pus asta doar ca sa dea run
                return "";
            }
            catch (HttpListenerException ex)
            {
                Console.WriteLine($"HttpListenerException: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }

        private void EnsureListenerCreated()
        {
            if (listener == null)
            {
                listener = new HttpListener();
                listener.Prefixes.Add($"http://localhost:{port}/");
                listener.Start();
                Console.WriteLine($"Listening on port {port}");
            }
        }

        public void Dispose()
        {
            listener?.Stop();
            listener?.Close();
        }
    }
}
