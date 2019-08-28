using Newtonsoft.Json;
using PushServer.Abstractions;
using PushServer.Models;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PushServer.Firebase
{
    public class FirebaseHttpClient : IFirebaseHttpClient
    {
        private readonly HttpClient httpClient;

        public FirebaseHttpClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        private class FirebasePushRequestAndroidOptions
        {
            public string Priority { get; set; }
        }

        private class FirebasePushRequestWebPushOptions
        {
            public FirebasePushRequestWebPushHeaders Headers { get; set; }
        }

        private class FirebasePushRequestWebPushHeaders
        {
            [JsonProperty("Urgency")]
            public string Urgency { get; set; }
        }

        private class FirebasePushRequest
        {
            public object Data { get; set; }
            [JsonProperty("registration_ids")]
            public string[] RegistrationIds { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public int? Priority { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public FirebasePushRequestAndroidOptions Android { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public FirebasePushRequestWebPushOptions Webpush { get; set; }
        }

        public async Task Push(string serverKey, string[] registrationIds, object data, PushUrgency? urgency = PushUrgency.Normal)
        {
            var request = new FirebasePushRequest
            {
                RegistrationIds = registrationIds,
                Data = data
            };
            if (PushUrgency.High == (urgency ?? PushUrgency.Normal))
            {
                request.Android = new FirebasePushRequestAndroidOptions()
                {
                    Priority = "high"
                };
                request.Priority = 10;
                request.Webpush = new FirebasePushRequestWebPushOptions()
                {
                    Headers = new FirebasePushRequestWebPushHeaders()
                    {
                        Urgency = "high"
                    }
                };
            }
            httpClient.DefaultRequestHeaders.Add("Authorization", $"key={serverKey}");
            var res = await httpClient.PostAsync("fcm/send", new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
            if (HttpStatusCode.OK != res.StatusCode)
            {
                throw new PushException($"Delivery resulted in {res.StatusCode}, {await res.Content?.ReadAsStringAsync()}, {res.ReasonPhrase}");
            }
        }
    }
}
