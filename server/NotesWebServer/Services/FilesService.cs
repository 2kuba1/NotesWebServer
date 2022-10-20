using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using NotesWebServer.Entities;
using NotesWebServer.Models;

namespace NotesWebServer.Services;

public interface IFilesService
{
    Task UploadImgAsync(IFormFile file);
    Task<string> GetFileAsync(string fileName);
    IEnumerable<string> GetFilesList();
}

public class FilesService : IFilesService
{
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public FilesService(AppDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public async Task<string> GetFileAsync(string fileName)
    {
        var searchForFile = await _dbContext.Files
            .FirstOrDefaultAsync(f => f.FileName == fileName);

        if (searchForFile is null)
        {
            throw new Exception($"{fileName} not found");
        }

        return searchForFile.FileUrl;
    }

    public async Task UploadImgAsync(IFormFile file)
    {
        var splitedFileName = file.FileName.Split('.');

        var checkIfNameExist = _dbContext.Files.FirstOrDefault(s => s.FileName == splitedFileName[0]);

        if (checkIfNameExist is not null)
        {
            throw new Exception("File with this name already exist");
        }

        var storageConnectionString = _configuration.GetValue<string>("Storage:BlobStorageConnectionString");
        var storageContainerName = _configuration.GetValue<string>("Storage:FilesBlobStorageContainerName");

        var container = new BlobContainerClient(storageConnectionString, storageContainerName);

        var blob = container.GetBlobClient(file.FileName);
        await using (Stream? data = file.OpenReadStream())
        {
            if (await blob.ExistsAsync())
            {
                throw new Exception("File with this name already exist");
            }

            await blob.UploadAsync(data);
        }

        var image = new Files()
        {
            FileUrl = $"yoururl/{file.FileName}",
            FileName = splitedFileName[0]
        };

        await _dbContext.Files.AddAsync(image);
        await _dbContext.SaveChangesAsync();
    }

    public IEnumerable<string> GetFilesList()
    {
        var notes = _dbContext.Files.Select(s => s.FileName).ToList();
        return notes;
    }
}