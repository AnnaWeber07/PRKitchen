using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AnnaWebKitchenFin.Models;
using AnnaWebKitchenFin.Utils;

namespace AnnaWebKitchenFin.Server
{
    public class KitchenServer
    {
        private static HttpListener listener;
        private static string receiveUrl = "http://localhost:8081/";
        private static string sendUrl = "http://localhost:8082/";

        private Kitchen kitchen;

        public async Task HandleIncomingConnections()
        {
            bool isRunning = true;

            while (isRunning)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/order")
                {
                    using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
                    string input = reader.ReadToEnd();
                    Order order = JsonSerializer.Deserialize<Order>(input);
                    LogsWriter.Log($"Attention to all staff, we got a new order numbered {order.Id}");
                    kitchen.ReceiveOrder(order);
                }

                if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/shutdown")
                {
                    Console.WriteLine("Shutdown of server");
                    isRunning = false;
                }

                response.StatusCode = 200;
                response.Close();
            }
        }

        public void ReturnDoneOrder(Order order)
        {
            using var client = new HttpClient();
            var content = JsonSerializer.Serialize(order);
            string mediaType = "application/json";
            var response = client.PostAsync(sendUrl + "ready", new StringContent(content, Encoding.UTF8, mediaType))
                                 .GetAwaiter()
                                 .GetResult();

            if (response.IsSuccessStatusCode)
            {
                LogsWriter.Log($"Order {order.Id} is sent to Dining Hall");
                kitchen.Orders.Remove(order);
            }
            else
                LogsWriter.Log("The waiters are gone.");
        }

        public async void Start(Kitchen kitchen)
        {
            this.kitchen = kitchen;

            listener = new HttpListener();
            listener.Prefixes.Add(receiveUrl);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", receiveUrl);

            await HandleIncomingConnections();

            listener.Close();
        }
    }
}