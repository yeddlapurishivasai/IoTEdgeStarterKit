using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;

namespace ThumbnailCoverter
{
    public interface IDirectMethodHelper
    {
        Task<MethodResponse> Print(MethodRequest methodRequest);
    }
}
