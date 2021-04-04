namespace ThumbnailCoverter
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Devices.Client;
    using Microsoft.Azure.Devices.Client.Transport.Mqtt;
    using Microsoft.Azure.Devices.Shared;

    public class ModuleClientProxy : IModuleClientProxy
    {
        private readonly ModuleClient moduleClient;

        public ModuleClientProxy()
        {
            this.moduleClient = CreateModuleClient();
        }

        public async Task OpenAsync(CancellationToken cancellationToken)
        {
            await this.moduleClient.OpenAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task CloseAsync(CancellationToken cancellationToken)
        {
            await this.moduleClient.CloseAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task SetDesiredPropertyUpdateCallbackAsync(DesiredPropertyUpdateCallback callback)
        {
            await this.moduleClient.SetDesiredPropertyUpdateCallbackAsync(callback, null).ConfigureAwait(false);
        }

        public async Task SetMethodHandlerAsync(string methodName, MethodCallback methodHandler)
        {
            await this.moduleClient.SetMethodHandlerAsync(methodName, methodHandler, null).ConfigureAwait(false);
        }

        public async Task<Twin> GetTwinAsync()
        {
            return await this.moduleClient.GetTwinAsync().ConfigureAwait(false);
        }

        public async Task UpdateReportedPropertiesAsync(TwinCollection reportProperties)
        {
            await this.moduleClient.UpdateReportedPropertiesAsync(reportProperties).ConfigureAwait(false);
        }

        public async Task SendEventsAsync(string outputPath, Message message)
        {
            await this.moduleClient.SendEventAsync(outputPath, message).ConfigureAwait(false);
        }

        private static ModuleClient CreateModuleClient()
        {
            var mqttSetting = new MqttTransportSettings(TransportType.Mqtt_Tcp_Only);
            ITransportSettings[] settings = { mqttSetting };

            return ModuleClient.CreateFromEnvironmentAsync(settings).GetAwaiter().GetResult();
        }
    }
}
