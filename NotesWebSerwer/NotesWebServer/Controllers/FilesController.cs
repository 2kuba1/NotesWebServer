using System.Runtime.Intrinsics.X86;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace NotesWebServer.Controllers;

[ApiController]
[Route("/api/files")]
public class FilesController : ControllerBase
{
    [HttpGet]
    public ActionResult GetFile([FromQuery] string file)
    {
        var files = Directory.GetCurrentDirectory();
        var filePath = $"{files}/Notes/{file}";

        var fileExist = System.IO.File.Exists(filePath);

        if (!fileExist)
        {
            return NotFound();
        }

        var content = System.IO.File.ReadAllBytes(filePath);

        var contentProvider = new FileExtensionContentTypeProvider();
        contentProvider.TryGetContentType(file, out string type);

        return File(content,type ,file);
    }
}