using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ThumbnailCoverter
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IModuleClientProxy moduleClientProxy;
        private readonly IServiceProvider serviceProvider;

        public Worker(ILogger<Worker> logger, IModuleClientProxy moduleClientProxy, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.moduleClientProxy = moduleClientProxy;
            this.serviceProvider = serviceProvider;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await this.moduleClientProxy.CloseAsync(cancellationToken).ConfigureAwait(false);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                if (!stoppingToken.IsCancellationRequested)
                {
                    this.logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    await this.Initialize(stoppingToken).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Exception while starting the management worker {ex.Message}");
            }
        }

        private async Task Initialize(CancellationToken stoppingToken)
        {
            this.logger.LogInformation("Module Initialize called");
            await this.moduleClientProxy.OpenAsync(stoppingToken).ConfigureAwait(false);

            // Direct Methods
            var directMethodHelper = this.serviceProvider.GetRequiredService<IDirectMethodHelper>();
            await this.moduleClientProxy.SetMethodHandlerAsync("PrintDirectMethod", (methodRequest, usetContext) => directMethodHelper.Print(methodRequest)).ConfigureAwait(false);

            // Send event to get secrets
            var deviceName = Environment.GetEnvironmentVariable("IOTEDGE_DEVICEID");
            var moduleName = Environment.GetEnvironmentVariable("IOTEDGE_MODULEID");
            var eventDetails = new EventDetails
            {
                DirectMethod = "PrintDirectMethod",
                ModuleId = moduleName,
                DeviceId = deviceName
            };
            var eventMessageString = JsonConvert.SerializeObject(eventDetails);
            using var eventMessage = new Message(Encoding.UTF8.GetBytes(eventMessageString))
            {
                ContentType = "application/json"
            };

            this.logger.LogInformation("Sending event to fetch secrets");
            await this.moduleClientProxy.SendEventsAsync("eventqueue", eventMessage).ConfigureAwait(false);
            this.logger.LogInformation("Finished Sending event to fetch secrets");

            // Thumbnail Converter
            var thumbnailProcessor = this.serviceProvider.GetRequiredService<IThumbnailProcessor>();
            await thumbnailProcessor.ProcessImages(stoppingToken).ConfigureAwait(false);

        }
    }
}
