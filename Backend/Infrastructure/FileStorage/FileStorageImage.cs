using Microsoft.AspNetCore.Http;
using SkiaSharp;

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
                using var stream = file.OpenReadStream();
                using var original = SKBitmap.Decode(stream);

                SKBitmap bitmap = original;
                bool resized = false;

                if (original.Width > 400)
                {
                    int newHeight = (int)(original.Height * (400.0 / original.Width));
                    bitmap = original.Resize(new SKImageInfo(400, newHeight), new SKSamplingOptions(SKCubicResampler.Mitchell));
                    resized = true;
                }

                using var image = SKImage.FromBitmap(bitmap);
                using var data = image.Encode(SKEncodedImageFormat.Jpeg, 80);

                await using var output = File.OpenWrite(path);
                data.SaveTo(output);

                if (resized) bitmap.Dispose();

                var currentUrl = $"{scheme}://{host}";
                var pathDb = Path.Combine(currentUrl, container, fileName).Replace("\\", "/");
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