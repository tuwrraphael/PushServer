using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PushServer.Firebase;
using PushServer.Models;
using PushServer.PushConfiguration.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PushServer.Tests
{
    public class FirebasePushProviderTests
    {

        private IOptions<FirebaseConfig> MockOptions(FirebaseConfig config)
        {
            var mock = new Mock<IOptions<FirebaseConfig>>(MockBehavior.Strict);
            mock.SetupGet(v => v.Value).Returns(config);
            return mock.Object;
        }

        [Fact]
        public async void ConvertsStringToObject()
        {
            var clientMock = new Mock<IFirebaseHttpClient>(MockBehavior.Strict);
            clientMock.Setup(v => v.Push("serverKey", "receipient", It.IsAny<JObject>(), (PushUrgency?)null))
                .Returns(Task.CompletedTask)
                .Verifiable();
            var provider = new FirebasePushProvider(MockOptions(new FirebaseConfig() { ServerKey = "serverKey" }), clientMock.Object,
                new PushChannelConfiguration()
                {

                }, new PushEndpoint()
                {
                    Endpoint = "receipient"
                });
            await provider.PushAsync("{'action' : 'digit.sync'}", new PushOptions());
        }

        [Fact]
        public void SerializesCorrectly()
        {
            var payload = "{'action' : 'digit.sync'}";
            var objectPayload = JsonConvert.DeserializeObject(payload);
            var request = new FirebasePushRequest()
            {
                Data = objectPayload,
                Priority = 10,
                To = "recv"
            };
            var str = JsonConvert.SerializeObject(request);
            Assert.Equal("{\"data\":{\"action\":\"digit.sync\"},\"to\":\"recv\",\"priority\":10}", str);
        }
    }
}
