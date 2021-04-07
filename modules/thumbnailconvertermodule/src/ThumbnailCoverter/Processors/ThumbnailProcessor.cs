using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace ThumbnailCoverter
{
    public class ThumbnailProcessor : IThumbnailProcessor
    {
        private readonly ILogger<ThumbnailProcessor> logger;
        private readonly MemoryCache memoryCache;

        public ThumbnailProcessor(ILogger<ThumbnailProcessor> logger, MyMemoryCache memoryCache)
        {
            this.logger = logger;
            this.memoryCache = memoryCache.Cache;
        }
        public async Task ProcessImages(CancellationToken cancellationToken)
        {
            try
            {
                this.logger.LogInformation("Thumbnail processor started");
                while (!cancellationToken.IsCancellationRequested)
                {
                    this.logger.LogInformation("Looking for new images.....");
                    var processIntervalInSeconds = memoryCache.Get<string>("ProcessIntervalInSeconds");
                    var result = int.TryParse(processIntervalInSeconds, out int processIntervalInSecondsValue);
                    var path = "/var/input";
                    var thumbnailPath = "/var/thumbnails";
                    string[] fileEntries = Directory.GetFiles(path);
                    foreach (string fileName in fileEntries)
                    {
                        try
                        {
                            var image = Image.FromFile(fileName);
                            this.logger.LogInformation($"Generating thumbnail for {fileName}");
                            var thumbnail = image.GetThumbnailImage(60, 60, () => false, IntPtr.Zero);
                            var fileStartIndex = fileName.LastIndexOf('/');
                            var fileEndIndex = fileName.LastIndexOf('.');
                            var thumbnailName = $"{ fileName.Substring(fileStartIndex + 1, fileEndIndex - fileStartIndex - 1) }-thumbnail.png";
                            var thumbnailPathWithName = $"{thumbnailPath}/{thumbnailName}";
                            this.logger.LogInformation($"Saving thumbnail for {thumbnailPathWithName}");
                            thumbnail.Save(thumbnailPathWithName, ImageFormat.Png);
                            this.logger.LogInformation($"Saved");
                            this.logger.LogInformation($"Uploading thumbnail to Storage Account");
                            await this.UploadthumbnailToBlob(thumbnailName, thumbnailPathWithName);
                            this.logger.LogInformation($"Deleting file {fileName}");
                            File.Delete(fileName);
                            this.logger.LogInformation($"Deleted file {fileName}");
                            this.logger.LogInformation(fileName);
                        }
                        catch(Exception ex)
                        {
                            this.logger.LogError($"Error while processing file {fileName}: {ex.Message}");
                        }
                    }
                    await Task.Delay(result && processIntervalInSecondsValue  > 0 ? processIntervalInSecondsValue * 1000: 10000).ConfigureAwait(false);
                }
            }
            catch(Exception ex)
            {
                this.logger.LogError(ex.Message);
            }
        }

        private async Task UploadthumbnailToBlob(string thumbnailName, string thumbnailPathWithName)
        {
            var connectionString = this.memoryCache.Get<string>("StorageConnectionString");
            if(!string.IsNullOrWhiteSpace(connectionString))
            {
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                string containerName = "thumbnails";

                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                //you can check if the container exists or not, then determine to create it or not
                bool isExist = containerClient.Exists();
                if (!isExist)
                {
                    // Create the container and return a container client object
                    containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName).ConfigureAwait(false);
                }

                // Get a reference to a blob
                BlobClient blobClient = containerClient.GetBlobClient(thumbnailName);

                Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);

                // Open the file and upload its data
                using FileStream uploadFileStream = File.OpenRead(thumbnailPathWithName);
                await blobClient.UploadAsync(uploadFileStream, true);
                uploadFileStream.Close();
                this.logger.LogInformation($"Upload Success");
            }
            else
            {
                this.logger.LogWarning("Unable to upload to storage account as connection string is empty");
            }
        }
    }
}
