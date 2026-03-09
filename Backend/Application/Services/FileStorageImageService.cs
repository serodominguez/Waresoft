using Application.Interfaces;
using Infrastructure.FileStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Path = System.IO.Path;

namespace Application.Services
{
    public class FileStorageImageService : IFileStorageImageService
    {
        private readonly IHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFileStorageImage _fileStorageImage;

        public FileStorageImageService(IHostEnvironment env, IHttpContextAccessor httpContextAccessor, IFileStorageImage fileStorageImage)
        {
            _env = env;
            _httpContextAccessor = httpContextAccessor;
            _fileStorageImage = fileStorageImage;
        }

        public async Task<string> SaveFile(string container, IFormFile file)
        {
            var httpContext = _httpContextAccessor.HttpContext
                ?? throw new InvalidOperationException("No HTTP context available");

            var webRootPath = Path.Combine(_env.ContentRootPath, "wwwroot");
            var scheme = httpContext.Request.Scheme;
            var host = httpContext.Request.Host.Value;

            return await _fileStorageImage.SaveFile(container, file, webRootPath, scheme, host);
        }

        public async Task<string> EditFile(string container, IFormFile file, string route)
        {
            var httpContext = _httpContextAccessor.HttpContext
                ?? throw new InvalidOperationException("No HTTP context available");

            var webRootPath = Path.Combine(_env.ContentRootPath, "wwwroot");
            var scheme = httpContext.Request.Scheme;
            var host = httpContext.Request.Host.Value;

            return await _fileStorageImage.EditFile(container, file, route, webRootPath, scheme, host);
        }

        public async Task RemoveFile(string route, string container)
        {
            var webRootPath = Path.Combine(_env.ContentRootPath, "wwwroot");

            await _fileStorageImage.RemoveFile(route, container, webRootPath);
        }
    }
}
