using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace ThumbnailCoverter
{
    public class DirectMethodHelper : IDirectMethodHelper
    {
        private readonly ILogger<DirectMethodHelper> logger;
        private readonly IModuleClientProxy moduleClientProxy;

        public DirectMethodHelper(ILogger<DirectMethodHelper> logger, IModuleClientProxy moduleClientProxy)
        {
            this.logger = logger;
            this.moduleClientProxy = moduleClientProxy;
        }
        
        public Task<MethodResponse> Print(MethodRequest methodRequest)
        {
            this.logger.LogInformation("Started processing Print Direct Method");
            this.logger.LogInformation("Processing.....");
            this.logger.LogInformation("Processing Print Direct Method");

            string result = $"{{\"result\":\"Executed direct method: Successful\"}}";
            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
        }
    }
}