using Microsoft.EntityFrameworkCore;
using PushServer.PushConfiguration.Abstractions.Models;
using PushServer.PushConfiguration.Abstractions.Services;
using PushServer.PushConfiguration.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PushServer.PushConfiguration.EntityFramework
{
    public class PushConfigurationStore : IPushConfigurationStore
    {
        private readonly ConfigurationDbContext configurationContext;

        public PushConfigurationStore(ConfigurationDbContext configurationContext)
        {
            this.configurationContext = configurationContext;
        }

        public async Task<bool> DeleteAsync(string userId, string configurationId)
        {
            var channel = configurationContext.PushChannelConfigurations.Where(v => v.UserId == userId && v.Id == configurationId).SingleOrDefault();
            if (null == channel)
            {
                return false;
            }
            configurationContext.PushChannelConfigurations.Remove(channel);
            await configurationContext.SaveChangesAsync();
            return true;
        }

        public async Task<Abstractions.Models.PushChannelConfiguration[]> GetAllAsync(string userId)
        {
            return await configurationContext.PushChannelConfigurations.Include(v => v.Options).Where(v => v.UserId == userId)
                .Select(v => new Abstractions.Models.PushChannelConfiguration()
                {
                    EndpointInfo = v.EndpointInfo,
                    Id = v.Id,
                    Options = new PushChannelOptions(v.Options.ToDictionary(d => d.Key, d => d.Value)),
                    ChannelType = v.Type
                }).ToArrayAsync();
        }

        private async Task CreateOptionsAsync(PushChannelOptions options, string channelId)
        {
            if (null == options)
            {
                return;
            }
            foreach (var pair in options)
            {
                var option = new PushChannelOption()
                {
                    ID = Guid.NewGuid().ToString(),
                    Key = pair.Key,
                    Value = pair.Value,
                    PushChannelConfigurationID = channelId
                };
                await configurationContext.PushChannelOptions.AddAsync(option);
            }
        }

        public async Task<Abstractions.Models.PushChannelConfiguration> RegisterAsync(string userId, AzureNotificationHubPushChannelRegistration registration)
        {
            var channel = new Entities.PushChannelConfiguration()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Endpoint = registration.Endpoint,
                EndpointInfo = registration.DeviceInfo,
                Type = PushChannelType.AzureNotificationHub
            };
            await configurationContext.PushChannelConfigurations.AddAsync(channel);
            await CreateOptionsAsync(registration.Options, channel.Id);
            await configurationContext.SaveChangesAsync();
            return new Abstractions.Models.PushChannelConfiguration()
            {
                EndpointInfo = channel.EndpointInfo,
                Id = channel.Id,
                ChannelType = channel.Type,
                Options = registration.Options
            };
        }

        public async Task<Abstractions.Models.PushChannelConfiguration> RegisterAsync(string userId, WebPushChannelRegistration registration)
        {
            var channel = new Entities.PushChannelConfiguration()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Endpoint = registration.Endpoint,
                EndpointInfo = registration.BrowserInfo,
                Type = PushChannelType.Web,
                AuthKey = registration.AuthKey,
                ExpirationTime = registration.ExpirationTime,
                P256dhKey = registration.P256dhKey,
            };
            await configurationContext.PushChannelConfigurations.AddAsync(channel);
            await CreateOptionsAsync(registration.Options, channel.Id);
            await configurationContext.SaveChangesAsync();
            return new Abstractions.Models.PushChannelConfiguration()
            {
                EndpointInfo = channel.EndpointInfo,
                Id = channel.Id,
                ChannelType = channel.Type,
                Options = registration.Options
            };
        }

        public async Task UpdateAsync(string userId, string configurationId, WebPushChannelRegistration registration)
        {
            var channel = configurationContext.PushChannelConfigurations.Include(v => v.Options).Where(v => v.UserId == userId && v.Id == configurationId).Single();
            foreach (var opt in channel.Options)
            {
                configurationContext.PushChannelOptions.Remove(opt);
            }
            channel.Endpoint = registration.Endpoint;
            channel.ExpirationTime = registration.ExpirationTime;
            channel.P256dhKey = registration.P256dhKey;
            channel.AuthKey = registration.AuthKey;
            channel.EndpointInfo = registration.BrowserInfo;
            await CreateOptionsAsync(registration.Options, channel.Id);
            await configurationContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(string userId, string configurationId, AzureNotificationHubPushChannelRegistration registration)
        {
            var channel = configurationContext.PushChannelConfigurations.Include(v => v.Options).Where(v => v.UserId == userId && v.Id == configurationId).Single();
            foreach (var opt in channel.Options)
            {
                configurationContext.PushChannelOptions.Remove(opt);
            }
            channel.Endpoint = registration.Endpoint;
            channel.EndpointInfo = registration.DeviceInfo;
            await CreateOptionsAsync(registration.Options, channel.Id);
            await configurationContext.SaveChangesAsync();
        }

        public async Task<Abstractions.Models.PushChannelConfiguration> GetAsync(string configurationId)
        {
            return await configurationContext.PushChannelConfigurations.Include(v => v.Options).Where(v => v.Id == configurationId)
               .Select(v => new Abstractions.Models.PushChannelConfiguration()
               {
                   EndpointInfo = v.EndpointInfo,
                   Id = v.Id,
                   Options = new PushChannelOptions(v.Options.ToDictionary(d => d.Key, d => d.Value)),
                   ChannelType = v.Type
               }).SingleAsync();
        }

        public async Task<PushEndpoint> GetEndpointAsync(string configurationId)
        {
            return await configurationContext.PushChannelConfigurations.Include(v => v.Options).Where(v => v.Id == configurationId)
               .Select(v => new PushEndpoint()
               {
                   AuthKey = v.AuthKey,
                   ChannelType = v.Type,
                   Endpoint = v.Endpoint,
                   P256dhKey = v.P256dhKey
               }).SingleAsync();
        }

        public async Task<Abstractions.Models.PushChannelConfiguration> GetForOptionsAsync(string userId, IDictionary<string, string> configurationOptions)
        {
            var query = configurationContext.PushChannelConfigurations.Include(v => v.Options).Where(v => v.UserId == userId);
            foreach (var option in configurationOptions)
            {
                if (null != option.Value)
                {
                    query = query.Where(v => v.Options.Any(d => d.Key == option.Key && d.Value == option.Value));
                }
                else
                {
                    query = query.Where(v => v.Options.Any(d => d.Key == option.Key));
                }
            }
            return await query.Select(v => new Abstractions.Models.PushChannelConfiguration()
            {
                EndpointInfo = v.EndpointInfo,
                Id = v.Id,
                Options = new PushChannelOptions(v.Options.ToDictionary(d => d.Key, d => d.Value)),
                ChannelType = v.Type
            }).FirstOrDefaultAsync();
        }
    }
}
