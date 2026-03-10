using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Infrastructure.FileStorage
{
    public class FileStorageImage : IFileStorageImage
    {
        public async Task<string> SaveFile(string container, IFormFile file, string webRootPath, string scheme, string host)
        {
            var fileName = $"{Guid.NewGuid()}.jpg";
            string folder = Path.Combine(webRootPath, container);

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string path = Path.Combine(folder, fileName);

            try
            {
                using (var image = await Image.LoadAsync(file.OpenReadStream()))
                {
                    if (image.Width > 400)
                        image.Mutate(x => x.Resize(400, 0));

                    await image.SaveAsJpegAsync(path, new JpegEncoder { Quality = 80 });
                }

                var currentUrl = $"{scheme}://{host}";
                var pathDb = Path.Combine(currentUrl, container, fileName).Replace("\\", "/");
                //var pathDb = $"{currentUrl}/{container}/{fileName}";
                return pathDb;
            }
            catch
            {
                if (File.Exists(path))
                    File.Delete(path);

                throw;
            }
        }

        public async Task<string> EditFile(string container, IFormFile file, string route, string webRootPath, string scheme, string host)
        {
            string? newPath = null;

            try
            {
                newPath = await SaveFile(container, file, webRootPath, scheme, host);

                if (!string.IsNullOrEmpty(route))
                {
                    await RemoveFile(route, container, webRootPath);
                }

                return newPath;
            }
            catch
            {
                if (!string.IsNullOrEmpty(newPath))
                {
                    var newFileName = Path.GetFileName(newPath);
                    var newFilePath = Path.Combine(webRootPath, container, newFileName);
                    if (File.Exists(newFilePath))
                        File.Delete(newFilePath);
                }

                throw;
            }
        }

        public Task RemoveFile(string route, string container, string webRootPath)
        {
            if (string.IsNullOrEmpty(route))
                return Task.CompletedTask;

            try
            {
                var fileName = Path.GetFileName(route);
                var directoryFile = Path.Combine(webRootPath, container, fileName);

                if (File.Exists(directoryFile))
                    File.Delete(directoryFile);

                return Task.CompletedTask;
            }
            catch
            {
                throw;
            }
        }
    }
}
