using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThumbnailCoverter
{
    public interface IThumbnailProcessor
    {
        Task ProcessImages(CancellationToken cancellationToken);
    }
}
