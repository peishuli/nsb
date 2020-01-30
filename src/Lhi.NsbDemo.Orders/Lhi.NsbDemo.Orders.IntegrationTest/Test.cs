using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Xunit;


namespace Lhi.NsbDemo.Orders.IntegrationTest
{
    public class Test
    {
        [Fact]
        public void IntegrationTest()
        {
            using (var client = new HttpClient())
            {
                // Use Polly retry policy to handle http request, in the example below, we simply
                // retry three times (the interval will be determined by the timeout of the API)
                RetryPolicy retry = Policy
                  .Handle<HttpRequestException>()
                  .Retry(3);

                // Calling the api and capture the tracking id (i.e., a "transaction id")
                var transactionId = retry.Execute(() =>  CallApi(client));

                Console.WriteLine($"TransactionId = {transactionId}");

                // Wait for 5 seconds -- give Platform handler time to complete processing
                Thread.Sleep(TimeSpan.FromSeconds(20));

                // Get all log messages from elasitcsearch for a given transaction id
                var json = GetLogsFromElasticsearch(client, transactionId);
                // Verfiy that the total hit is >= 4
                dynamic result = JsonConvert.DeserializeObject<dynamic>(json);
                int totalHits = result.hits.total;
                Assert.True(totalHits >= 4);

            }
        }

        private static string CallApi(HttpClient client)
        {
            //var url = "http://api:80/api/v1/orders";
            var url = "http://localhost:8080/api/v1/orders";
            var req = new CreateOrderRequestDto();
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(req);
            var content = new StringContent(json);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = client.PostAsync(url, content).Result;

            response.EnsureSuccessStatusCode();
            var resp = response.Content.ReadAsStringAsync().Result;
            dynamic result = JsonConvert.DeserializeObject<dynamic>(resp);
            var transactionId = result.customResponseHeaders.TrackingId;
            return transactionId;
        }

        private static string GetLogsFromElasticsearch(HttpClient client, string transactionId)
        {
            //var url = $"http://elasticsearch:9200/nsbdemo-*/_search?q=TransactionId:{transactionId}";
            var url = $"http://localhost:9200/nsbdemo-*/_search?q=TransactionId:{transactionId}";
            var response = client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();
            var json = response.Content.ReadAsStringAsync().Result;
            return json;
        }
    }
}
