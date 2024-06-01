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

                context.Response.OutputStream.Close();

                return request.Url.AbsolutePath;
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
