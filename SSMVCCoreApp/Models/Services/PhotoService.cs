using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SSMVCCoreApp.Models.Abstract;

namespace SSMVCCoreApp.Models.Services
{
    public class PhotoService : IPhotoService
    {
        private CloudStorageAccount _storageAccount;
        private readonly ILogger<PhotoService> _logger;

        public PhotoService(IOptions<StorageUtility> storageUtility, ILogger<PhotoService> logger)
        {
            _storageAccount = storageUtility.Value.StorageAccount;
            _logger = logger;
        }

        public async Task<string> UploadPhotoAsync(string category, IFormFile photoToUpload)
        {
            if (photoToUpload == null || photoToUpload.Length == 0)
                return null;
            string categoryLowerCase = category.ToLower().Trim();
            string fullPath = null;
            try
            {
                //Code to create blobclient
                CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();
                CloudBlobContainer blobContainer = blobClient.GetContainerReference(categoryLowerCase);

                if (await blobContainer.CreateIfNotExistsAsync())
                {
                    await blobContainer.SetPermissionsAsync(new
                        BlobContainerPermissions
                    {
                        PublicAccess =
                        BlobContainerPublicAccessType.Blob
                    });
                    _logger.LogInformation($"Successfully created Blob Storage Container '{blobContainer.Name}'" +
                        $" and made it public");
                }

                //Code to check if duplicate photoname is taken or not
                string imagename = $"productphoto{Guid.NewGuid().ToString()}{Path.GetExtension(photoToUpload.FileName.Substring(photoToUpload.FileName.LastIndexOf("/") + 1))}";

                //code to upload photo in the form of block blob as photo size is small
                CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(imagename);
                blockBlob.Properties.ContentType = photoToUpload.ContentType;
                await blockBlob.UploadFromStreamAsync(photoToUpload.OpenReadStream());

                //Code to get the full path uri of blobpath
                fullPath = blockBlob.Uri.ToString();
                _logger.LogInformation($"Blob service, PhotoService.UploadPhoto, imagePath='{fullPath}'");

                }

            catch (Exception ex)
            {

                _logger.LogError(ex, "Error Uploading the photo blob to storage");
                throw;
            }
            return fullPath;

        }

        public async Task<bool> DeletePhotoAsync(string category, string photoUrl)
        {
            if (string.IsNullOrEmpty(photoUrl)) return true;

            string categoryLowerCase = category.ToLower().Trim();
            bool deletedFlag = false;

            try
            {
                CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();
                CloudBlobContainer blobContainer = blobClient.GetContainerReference(categoryLowerCase);

                if(blobContainer.Name==categoryLowerCase)
                {
                    string blobName = photoUrl.Substring(photoUrl.LastIndexOf("/") + 1);
                    CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blobName);
                    deletedFlag = await blockBlob.DeleteIfExistsAsync();
                    _logger.LogInformation($"Blob Service, PhotoService.DeletedPhoto, deletedImagePath='{photoUrl}'");

                    //assignent delete the container if it is empty

                    if (blobContainer == null)
                    {
                        deletedFlag = await blobContainer.DeleteIfExistsAsync();
                        _logger.LogInformation($"Blob container, BlobService.DeletedBlob, deletedBlob={blobContainer}");
                    }



                }

            }
            catch (Exception ex)
            {

                _logger.LogError(ex,"Error in deleting the photo from Blob Storage");
                throw;
            }
            return deletedFlag;
        }
    }
}
