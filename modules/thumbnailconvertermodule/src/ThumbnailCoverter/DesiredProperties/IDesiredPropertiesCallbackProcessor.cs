using System.Threading.Tasks;
using Microsoft.Azure.Devices.Shared;

namespace ThumbnailCoverter
{
    public interface IDesiredPropertiesCallbackProcessor
    {
        Task OnDesiredPropertiesUpdate(TwinCollection desiredProperties);
    }
}
