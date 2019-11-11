using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SSMVCCoreApp.Models;

namespace SSMVCCoreApp.ViewComponents
{
  public class CategoriesViewComponent : ViewComponent
  {
    private readonly IOptions<StorageUtility> _storageUtility;

    public CategoriesViewComponent(IOptions<StorageUtility> storageUtility)
    {
      _storageUtility = storageUtility;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
      CloudStorageAccount cloudStorageAccount = _storageUtility.Value.StorageAccount;
      CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
      var currentToken = new BlobContinuationToken();
      List<string> categoriesContainerList = new List<string>();
      var result = await cloudBlobClient.ListContainersSegmentedAsync(currentToken);
      if (result.Results.Count() != 0)
      {
        categoriesContainerList = result.Results.Select(c => c.Name).ToList();
      }
      return await Task.FromResult<IViewComponentResult>(View("Default", categoriesContainerList));
    }
  }
}
