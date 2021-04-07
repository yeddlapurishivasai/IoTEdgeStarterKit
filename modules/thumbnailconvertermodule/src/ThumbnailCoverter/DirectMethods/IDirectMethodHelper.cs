using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;

namespace ThumbnailCoverter
{
    public interface IDirectMethodHelper
    {
        Task<MethodResponse> ReceiveCloudConfigurations(MethodRequest methodRequest);
        Task<MethodResponse> PrintHello(MethodRequest methodRequest);
    }
}
