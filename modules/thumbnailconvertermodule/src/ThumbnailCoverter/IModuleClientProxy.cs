namespace ThumbnailCoverter
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Devices.Client;
    using Microsoft.Azure.Devices.Shared;
    public interface IModuleClientProxy
    {
        Task OpenAsync(CancellationToken cancellationToken);

        Task CloseAsync(CancellationToken cancellationToken);

        Task SetMethodHandlerAsync(string methodName, MethodCallback methodHandler);

        Task<Twin> GetTwinAsync();

        Task SetDesiredPropertyUpdateCallbackAsync(DesiredPropertyUpdateCallback callback);

        Task UpdateReportedPropertiesAsync(TwinCollection reportProperties);

        Task SendEventsAsync(string outputPath, Message message);
    }
}
