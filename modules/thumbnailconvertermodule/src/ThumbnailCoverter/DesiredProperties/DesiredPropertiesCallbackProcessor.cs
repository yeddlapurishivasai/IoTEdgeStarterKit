using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ThumbnailCoverter
{
    public class DesiredPropertiesCallbackProcessor : IDesiredPropertiesCallbackProcessor
    {
        private readonly MemoryCache memoryCache;
        private readonly IModuleClientProxy moduleClientProxy;
        private readonly ILogger logger;

        public DesiredPropertiesCallbackProcessor(ILogger<DesiredPropertiesCallbackProcessor> logger, MyMemoryCache memoryCach, IModuleClientProxy moduleClientProxy)
        {
            this.memoryCache = memoryCach.Cache;
            this.moduleClientProxy = moduleClientProxy;
            this.logger = logger;
        }

        public async Task OnDesiredPropertiesUpdate(TwinCollection desiredProperties)
        {
            this.logger.LogInformation("Desired properties updated.");
            DesiredProperties desiredPropertiesModel = JsonConvert.DeserializeObject<DesiredProperties>(desiredProperties.ToJson());
            TwinCollection reportedProperties = new TwinCollection();

            if (!string.IsNullOrWhiteSpace(desiredPropertiesModel.ProcessIntervalInSeconds))
            {
                var key = "ProcessIntervalInSeconds";
                this.logger.LogInformation("Updating ProcessIntervalInSeconds in cache");
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                // Set cache entry size by extension method.
                .SetSize(1);
                this.memoryCache.Set<string>(key, desiredPropertiesModel.ProcessIntervalInSeconds, cacheEntryOptions);
                reportedProperties[key] = desiredPropertiesModel.ProcessIntervalInSeconds;
                this.logger.LogInformation("Updating reported properties");
                await this.moduleClientProxy.UpdateReportedPropertiesAsync(reportedProperties).ConfigureAwait(false);
                this.logger.LogInformation("Updating reported properties success");
            }
        }
    }
}
