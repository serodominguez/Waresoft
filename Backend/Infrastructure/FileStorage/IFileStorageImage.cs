using Microsoft.AspNetCore.Http;

namespace Infrastructure.FileStorage
{
    public interface IFileStorageImage
    {
        Task<string> SaveFile(string container, IFormFile file, string webRootPath, string scheme, string host);
        Task<string> EditFile(string container, IFormFile file, string route, string webRootPath, string scheme, string host);
        Task RemoveFile(string route, string container, string webRootPath);
    }
}
