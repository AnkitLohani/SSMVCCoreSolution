
using Microsoft.AspNetCore.Mvc;
using System;

namespace SSMVCCoreApp.Controllers
{
  public class AboutController : Controller
  {
    public IActionResult Index() => View();
    public IActionResult Throw()
    {
      throw new EntryPointNotFoundException("This is a user thrown exception");
    }
  }
}
