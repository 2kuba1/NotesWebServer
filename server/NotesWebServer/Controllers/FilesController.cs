using System.Runtime.Intrinsics.X86;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace NotesWebServer.Controllers;

[ApiController]
[Route("/files")]
public class FilesController : ControllerBase
{

    [HttpGet]
    [Authorize(Roles = "User,Admin")]
    public ActionResult<Task<FileContentResult>> GetFileAsync([FromQuery] string file)
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

        return File(content, type, file);
    }

    [HttpPost]
    [Authorize(Roles = "User,Admin")]
    public ActionResult<Task> UploadFilesAsync([FromForm] IFormFile file)
    {
        if (file != null || file.Length > 0)
        {
            var filePath = $"{Directory.GetCurrentDirectory()}/Notes/{file.FileName}";

            var fileExist = System.IO.File.Exists(filePath);
            if (fileExist)
            {
                return Conflict();
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return Ok();
        }

        return BadRequest();
    }
}