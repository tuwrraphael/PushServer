﻿using System.Net.Http;
using System.Threading.Tasks;
using PushServer.Abstractions;
using PushServer.Abstractions.Services;
using PushServer.Models;
using PushServer.WebPushApiClient;

namespace PushServer.WebPush
{
    public class WebPushProvider : IPushProvider
    {
        private PushSubscription pushSubscription;
        private readonly IWebPushClient webPushClient;

        public WebPushProvider(PushSubscription pushSubscription,
            IWebPushClient webPushClient)
        {
            this.pushSubscription = pushSubscription;
            this.webPushClient = webPushClient;
        }

        public Task InitializeAsync() { return Task.CompletedTask; }


        public async Task PushAsync(string payload, PushOptions options)
        {
            var webPushOptions = WebPushOptions.Defaults;
            if (null != options && options.TimeToLive.HasValue)
            {
                webPushOptions.TimeToLive = options.TimeToLive.Value;
            }
            HttpResponseMessage res;
            if (null != payload)
            {
                res = await webPushClient.SendNotificationAsync(pushSubscription, payload, webPushOptions);
            }
            else
            {
                res = await webPushClient.SendNotificationAsync(pushSubscription, webPushOptions);
            }
            if (!res.IsSuccessStatusCode)
            {
                throw new PushException($"Attempted delivery resulted in {res.StatusCode}.", res);
            }
        }
    }
}