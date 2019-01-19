using FCM.Net;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PushServer.Abstractions;
using PushServer.Abstractions.Services;
using PushServer.Models;
using PushServer.PushConfiguration.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace PushServer.Firebase
{
    public class FirebasePushProvider : IPushProvider
    {
        private readonly PushChannelConfiguration config;
        private readonly PushEndpoint endpoint;
        private FirebaseConfig options;

        public FirebasePushProvider(IOptions<FirebaseConfig> optionsAccessor, PushChannelConfiguration config, PushEndpoint endpoint)
        {
            options = optionsAccessor.Value;
            this.config = config;
            this.endpoint = endpoint;
        }

        public async Task InitializeAsync()
        {

        }

        public async Task PushAsync(string payload, PushOptions opts)
        {
            using (var sender = new Sender(options.ServerKey))
            {
                try
                {
                    object objData = null;
                    try
                    {
                        objData = JsonConvert.DeserializeObject(payload);
                    }
                    catch
                    {

                    }
                    var res = await sender.SendAsync(new Message()
                    {
                        RegistrationIds = new List<string>() { endpoint.Endpoint },
                        Data = objData ?? payload,
                        Priority = null != opts && opts.Urgency == PushUrgency.High ? Priority.High : Priority.Normal
                    });
                    if (HttpStatusCode.OK != res.StatusCode)
                    {
                        throw new PushException($"Delivery resulted in {res.StatusCode}, {res?.MessageResponse?.InternalError}, {res.ReasonPhrase}");
                    }
                }
                catch (Exception e)
                {
                    throw new PushException($"Error while attempting firebase delivery.", e);
                }
            }
        }
    }
}
