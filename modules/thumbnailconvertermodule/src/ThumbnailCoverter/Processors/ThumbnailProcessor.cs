using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace ThumbnailCoverter
{
    public class ThumbnailProcessor : IThumbnailProcessor
    {
        private readonly ILogger<ThumbnailProcessor> logger;

        public ThumbnailProcessor(ILogger<ThumbnailProcessor> logger)
        {
            this.logger = logger;
        }
        public async Task ProcessImages(CancellationToken cancellationToken)
        {
            try
            {
                this.logger.LogInformation("Thumbnail processor started");
                while (!cancellationToken.IsCancellationRequested)
                {
                    var path = "/var/input";
                    var thumbnailPath = "/var/thumbnails";
                    string[] fileEntries = Directory.GetFiles(path);
                    foreach (string fileName in fileEntries)
                    {
                        var image = Image.FromFile(fileName);
                        this.logger.LogInformation($"Generating thumbnail for {fileName}");
                        var thumbnail = image.GetThumbnailImage(60, 60, () => false, IntPtr.Zero);
                        var fileStartIndex = fileName.LastIndexOf('/');
                        var fileEndIndex = fileName.LastIndexOf('.');
                        var thumbnailName = $"{thumbnailPath}/{fileName.Substring(fileStartIndex + 1, fileEndIndex - fileStartIndex -1)}-thumbnail.png";
                        this.logger.LogInformation($"Saving thumbnail for {thumbnailName}");
                        thumbnail.Save(thumbnailName, ImageFormat.Png);
                        this.logger.LogInformation($"Deleting file {fileName}");
                        File.Delete(fileName);
                        this.logger.LogInformation($"Deleted file {fileName}");
                        this.logger.LogInformation(fileName);
                    }
                    await Task.Delay(10000).ConfigureAwait(false);
                }
            }
            catch(Exception ex)
            {
                this.logger.LogError(ex.Message);
            }
        }
    }
}
