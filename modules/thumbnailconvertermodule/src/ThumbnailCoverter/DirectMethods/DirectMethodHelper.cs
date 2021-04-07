using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ThumbnailCoverter
{
    public class DirectMethodHelper : IDirectMethodHelper
    {
        private readonly ILogger<DirectMethodHelper> logger;
        private readonly IModuleClientProxy moduleClientProxy;
        private readonly MemoryCache memoryCache;

        public DirectMethodHelper(ILogger<DirectMethodHelper> logger, IModuleClientProxy moduleClientProxy, MyMemoryCache memoryCache)
        {
            this.logger = logger;
            this.moduleClientProxy = moduleClientProxy;
            this.memoryCache = memoryCache.Cache;
        }

        public Task<MethodResponse> ReceiveCloudConfigurations(MethodRequest methodRequest)
        {
            this.logger.LogInformation("Reveive cloud configuration called");
            string methodResponseMessage;
            int httpStatusCode;

            try
            {
                var data = Encoding.UTF8.GetString(methodRequest.Data);
                this.logger.LogInformation("Received Method data : " + data);
                var payloadModel = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
                foreach (var item in payloadModel)
                {
                    this.logger.LogInformation($"Adding {item.Key} to in-memory cache");
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Set cache entry size by extension method.
                    .SetSize(1);
                    this.memoryCache.Set<string>(item.Key, item.Value, cacheEntryOptions);
                }

                methodResponseMessage = "Successful";
                httpStatusCode = 200;
            }
            catch (Exception ex)
            {
                methodResponseMessage = ex.Message;
                httpStatusCode = 400;
            }

            this.logger.LogInformation($"Executed Direct Method ReceiveCloudConfigurations: {methodRequest.Name} Response: {methodResponseMessage} Status Code:{ httpStatusCode}");

            string result = "{\"result\":\"Executed direct method ReceiveCloudConfigurations: " + methodResponseMessage + "\"}";
            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), httpStatusCode));
        }

        public Task<MethodResponse> PrintHello(MethodRequest methodRequest)
        {
            this.logger.LogInformation("Hello from thumbnailconverter module.");
            string result = "{\"result\":\"Executed direct method PrintHello: Successful\"}";
            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
        }
    }
}