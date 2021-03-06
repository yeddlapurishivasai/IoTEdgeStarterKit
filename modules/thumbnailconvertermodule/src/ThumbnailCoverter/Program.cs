using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ThumbnailCoverter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddSingleton<MyMemoryCache>();
                    services.AddSingleton<IModuleClientProxy, ModuleClientProxy>();
                    services.AddSingleton<IDirectMethodHelper, DirectMethodHelper>();
                    services.AddSingleton<IThumbnailProcessor, ThumbnailProcessor>();
                    services.AddSingleton<IDesiredPropertiesCallbackProcessor, DesiredPropertiesCallbackProcessor>();
                });
    }
}
