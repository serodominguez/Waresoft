using Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Test.Api
{
    public class TestFileStorageImageService : IFileStorageImageService
    {
        public string GetBaseUrl() => "http://localhost";

        public Task<string> SaveFile(string container, IFormFile file)
            => Task.FromResult("fake/path/image.jpg");

        public Task<string> EditFile(string container, IFormFile file, string route)
            => Task.FromResult("fake/path/image_edited.jpg");

        public Task RemoveFile(string route, string container)
            => Task.CompletedTask;
    }
}
