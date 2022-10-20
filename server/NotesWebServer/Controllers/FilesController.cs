using System.Runtime.Intrinsics.X86;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using NotesWebServer.Models;
using NotesWebServer.Services;

namespace NotesWebServer.Controllers;

[ApiController]
[Route("/files")]
public class FilesController : ControllerBase
{
    private readonly IFilesService _filesService;

    public FilesController(IFilesService filesService)
    {
        _filesService = filesService;
    }
    
    [HttpGet]
    [Authorize(Roles = "User,Admin")]
    public async Task<ActionResult<string>>  GetFileAsync([FromQuery] string file)
    {
        var result = await _filesService.GetFileAsync(file);
        return result;
    }

    [HttpPost]
    [Authorize(Roles = "User,Admin")]
    public async Task<ActionResult> UploadFilesAsync([FromForm] IFormFile file)
    {
        await _filesService.UploadImgAsync(file);
        return Ok();
    }

    [HttpGet]
    [Route("getFilesList")]
    public ActionResult<IEnumerable<string>> GetFilesList()
    {
        var results = _filesService.GetFilesList();
        return Ok(results);
    }
}