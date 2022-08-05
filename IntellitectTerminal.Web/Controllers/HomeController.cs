using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace IntellitectTerminal.Web.Controllers;

public class HomeController : Controller
{
    /// <summary>
    /// Spa route for vue-based parts of the app
    /// </summary>
    // Prevent caching of this route.
    // The served file will contain the links to compiled js/css that include hashes in the filenames.
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Index(
        [FromServices] IWebHostEnvironment hostingEnvironment
    )
    {
        var fileInfo = hostingEnvironment.WebRootFileProvider.GetFileInfo("index.html");
        if (!fileInfo.Exists) return NotFound($"{hostingEnvironment.WebRootPath}/index.html was not found");

        return File(fileInfo.CreateReadStream(), "text/html");
    }

    public IActionResult Error()
    {
        ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        return View();
    }
}
