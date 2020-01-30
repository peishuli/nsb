using AutoFixture;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Lhi.NsbDemo.Orders.UAT
{
    class Program
    {
        static void Main(string[] args)
        {
            CallApi();

            Console.WriteLine("Completed ");
        }

        private static void CallApi()
        {
            var baseUri = "http://localhost:8080/";
            ////running inside docker container on the same bridge network. we can use the service name (api) with expoed port (80)
            //var baseUri = "http://api:80/"; 

            var url = "api/v1/orders";

            var client = new HttpClient();
            client.BaseAddress = new Uri(baseUri);

            var fixture = new Fixture();
            var orderReqs = fixture.CreateMany<CreateOrderRequestDto>(5000);
            foreach (var req in orderReqs)
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(req);

                var content = new StringContent(json);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = client.PostAsync(url, content).Result;

                response.EnsureSuccessStatusCode();
                string resp = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(resp);
                System.Threading.Thread.Sleep(100);
            }

            Console.WriteLine("Test completed.");
            Console.ReadKey();
        }
    }
}
