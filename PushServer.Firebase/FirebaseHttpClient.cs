using Newtonsoft.Json;
using PushServer.Abstractions;
using PushServer.Models;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PushServer.Firebase
{
    public class FirebasePushRequest
    {
        [JsonProperty("data")]
        public object Data { get; set; }
        [JsonProperty("to")]
        public string To { get; set; }
        [JsonProperty("priority", NullValueHandling = NullValueHandling.Ignore)]
        public int? Priority { get; set; }
    }

    public class FirebaseHttpClient : IFirebaseHttpClient
    {
        private readonly HttpClient httpClient;

        public FirebaseHttpClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        private class FirebasePushResult
        {
            public int Success { get; set; }
            public int Failure { get; set; }
        }

        public async Task Push(string serverKey, string to, object data, PushUrgency? urgency = PushUrgency.Normal)
        {
            var request = new FirebasePushRequest
            {
                To = to,
                Data = data
            };
            if (PushUrgency.High == (urgency ?? PushUrgency.Normal))
            {
                request.Priority = 10;
            }
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={serverKey}");
            var res = await httpClient.PostAsync("fcm/send", new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
            if (HttpStatusCode.OK != res.StatusCode)
            {
                throw new PushException($"Delivery resulted in {res.StatusCode}, {await res.Content?.ReadAsStringAsync()}, {res.ReasonPhrase}");
            }
            else
            {
                string content = await res.Content.ReadAsStringAsync();
                var r = JsonConvert.DeserializeObject<FirebasePushResult>(content);
                if (0 != r.Failure)
                {
                    throw new PushException($"Delivery failure {content}");
                }
            }
        }
    }
}
