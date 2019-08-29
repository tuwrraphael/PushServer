using Moq;
using Newtonsoft.Json;
using PushServer.Abstractions;
using PushServer.Abstractions.Services;
using PushServer.Firebase;
using PushServer.Impl;
using PushServer.Models;
using PushServer.PushConfiguration.Abstractions.Models;
using PushServer.PushConfiguration.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PushServer.Tests
{
    public class PushServiceTest
    {
        private const string TestChannelType = "Test";

        private static IPushProviderFactory MockByProvider(IDictionary<string, IPushProvider> map)
        {
            var providerFactoryMock = new Mock<IPushProviderFactory>(MockBehavior.Strict);
            providerFactoryMock.Setup(v => v.CreateProvider(It.IsAny<PushChannelConfiguration>()))
            .Returns<PushChannelConfiguration>(config => Task.FromResult(map[config.Id]));
            providerFactoryMock.SetupGet(v => v.PushChannelType).Returns(TestChannelType);
            return providerFactoryMock.Object;
        }


        private static IPushConfigurationStore MockByConfigs(PushChannelConfiguration[] configs)
        {
            var pushConfigurationMock = new Mock<IPushConfigurationStore>(MockBehavior.Strict);
            pushConfigurationMock.Setup(v => v.GetForOptionsAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
                .Returns(Task.FromResult(configs));
            return pushConfigurationMock.Object;
        }

        private static IPushConfigurationStore MockByConfig(PushChannelConfiguration config)
        {
            var pushConfigurationMock = new Mock<IPushConfigurationStore>(MockBehavior.Strict);
            pushConfigurationMock.Setup(v => v.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(config));
            return pushConfigurationMock.Object;
        }

        public class Exceptions
        {
            [Fact]
            public async void SingleConfiguration_Direct_PushFailed()
            {
                var config = new PushChannelConfiguration() { ChannelType = TestChannelType, Id = "TestId" };
                var ex = new PushException("test");

                var providerMock = new Mock<IPushProvider>(MockBehavior.Strict);
                providerMock.Setup(v => v.PushAsync(It.IsAny<string>(), It.IsAny<PushOptions>()))
                    .Throws(ex);

                var pushService = new PushService(MockByConfig(config),
                                  new[] { MockByProvider(new Dictionary<string, IPushProvider> {
                                      { config.Id, providerMock.Object } }) });

                var res = await Assert.ThrowsAsync<PushFailedException>(async () =>
                    await pushService.Push("configId", "payload", null));
                Assert.Single(res.Failures);
                Assert.Same(config, res.Failures[0].Configuration);
                Assert.Same(ex, res.Failures[0].Exception);
                Assert.Same(ex, res.InnerException);
            }

            [Fact]
            public async void SingleConfiguration_Options_PushFailed()
            {
                var config = new PushChannelConfiguration() { ChannelType = TestChannelType, Id = "TestId" };
                var ex = new PushException("test");

                var providerMock = new Mock<IPushProvider>(MockBehavior.Strict);
                providerMock.Setup(v => v.PushAsync(It.IsAny<string>(), It.IsAny<PushOptions>()))
                    .Throws(ex);

                var pushService = new PushService(MockByConfigs(new[] { config }),
                                  new[] { MockByProvider(new Dictionary<string, IPushProvider> {
                                      { config.Id, providerMock.Object } }) });

                var res = await Assert.ThrowsAsync<PushFailedException>(async () =>
                await pushService.Push("userid", new Dictionary<string, string>(), "payload", null));
                Assert.Single(res.Failures);
                Assert.Same(config, res.Failures[0].Configuration);
                Assert.Same(ex, res.Failures[0].Exception);
                Assert.Same(ex, res.InnerException);
            }

            [Fact]
            public async void MultiConfiguration_All_PushFailed()
            {
                var config1 = new PushChannelConfiguration() { ChannelType = TestChannelType, Id = "1" };
                var config2 = new PushChannelConfiguration() { ChannelType = TestChannelType, Id = "2" };
                var ex1 = new PushException("test");
                var ex2 = new PushException("test");

                var providerMock1 = new Mock<IPushProvider>(MockBehavior.Strict);
                providerMock1.Setup(v => v.PushAsync(It.IsAny<string>(), It.IsAny<PushOptions>()))
                    .Throws(ex1);
                var providerMock2 = new Mock<IPushProvider>(MockBehavior.Strict);
                providerMock2.Setup(v => v.PushAsync(It.IsAny<string>(), It.IsAny<PushOptions>()))
                    .Throws(ex2);

                var pushService = new PushService(MockByConfigs(new[] { config1, config2 }),
                                  new[] { MockByProvider(new Dictionary<string, IPushProvider> {
                                      { config1.Id, providerMock1.Object },
                                        { config2.Id, providerMock2.Object }}) });

                var res = await Assert.ThrowsAsync<PushFailedException>(async () =>
                await pushService.Push("userid", new Dictionary<string, string>(), "payload", null));
                Assert.Equal(2, res.Failures.Length);
                Assert.Collection(res.Failures,
                    v =>
                    {
                        Assert.Same(config1, v.Configuration);
                        Assert.Same(ex1, v.Exception);
                    },
                    v =>
                    {
                        Assert.Same(config2, v.Configuration);
                        Assert.Same(ex2, v.Exception);
                    });
                Assert.Collection(res.InnerExceptions, v => Assert.Same(ex1, v), v => Assert.Same(ex2, v));
            }

            [Fact]
            public async void MultiConfiguration_PartiallyFailed()
            {
                var config1 = new PushChannelConfiguration() { ChannelType = TestChannelType, Id = "1" };
                var config2 = new PushChannelConfiguration() { ChannelType = TestChannelType, Id = "2" };
                var config3 = new PushChannelConfiguration() { ChannelType = TestChannelType, Id = "3" };
                var ex1 = new PushException("test");
                var ex2 = new PushException("test");

                var providerMock1 = new Mock<IPushProvider>(MockBehavior.Strict);
                providerMock1.Setup(v => v.PushAsync(It.IsAny<string>(), It.IsAny<PushOptions>()))
                    .Throws(ex1);
                var providerMock2 = new Mock<IPushProvider>(MockBehavior.Strict);
                providerMock2.Setup(v => v.PushAsync(It.IsAny<string>(), It.IsAny<PushOptions>()))
                    .Throws(ex2);
                var providerMock3 = new Mock<IPushProvider>(MockBehavior.Strict);
                providerMock3.Setup(v => v.PushAsync(It.IsAny<string>(), It.IsAny<PushOptions>()))
                    .Returns(Task.CompletedTask);

                var pushService = new PushService(MockByConfigs(new[] { config1, config2, config3 }),
                                  new[] { MockByProvider(new Dictionary<string, IPushProvider> {
                                      { config1.Id, providerMock1.Object },
                                      { config2.Id, providerMock2.Object },
                                      { config3.Id, providerMock3.Object }
                                  }) });

                var res = await Assert.ThrowsAsync<PushPartiallyFailedException>(async () =>
                await pushService.Push("userid", new Dictionary<string, string>(), "payload", null));
                Assert.Equal(2, res.Failures.Length);
                Assert.Collection(res.Failures,
                    v =>
                    {
                        Assert.Same(config1, v.Configuration);
                        Assert.Same(ex1, v.Exception);
                    },
                    v =>
                    {
                        Assert.Same(config2, v.Configuration);
                        Assert.Same(ex2, v.Exception);
                    });
                Assert.Collection(res.InnerExceptions, v => Assert.Same(ex1, v), v => Assert.Same(ex2, v));
                Assert.Single(res.Succeeded);
                Assert.Collection(res.Succeeded, v =>
                {
                    Assert.Same(config3, v.Configuration);
                    Assert.Null(v.Exception);
                });
            }
        }
    }
}
