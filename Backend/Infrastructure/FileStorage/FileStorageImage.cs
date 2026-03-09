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
            // ✅ Siempre guardamos como .jpg optimizado
            var fileName = $"{Guid.NewGuid()}.jpg";
            string folder = Path.Combine(webRootPath, container);

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string path = Path.Combine(folder, fileName);

            // ✅ Optimización con ImageSharp
            using (var image = await Image.LoadAsync(file.OpenReadStream()))
            {
                // Redimensiona si el ancho es mayor a 800px manteniendo proporción
                if (image.Width > 400)
                    image.Mutate(x => x.Resize(400, 0));

                // Guarda como JPEG con 80% de calidad
                await image.SaveAsJpegAsync(path, new JpegEncoder { Quality = 80 });
            }

            var currentUrl = $"{scheme}://{host}";
            var pathDb = Path.Combine(currentUrl, container, fileName).Replace("\\", "/");
            return pathDb;
        }

        public async Task<string> EditFile(string container, IFormFile file, string route, string webRootPath, string scheme, string host)
        {
            await RemoveFile(route, container, webRootPath);

            return await SaveFile(container, file, webRootPath, scheme, host);
        }

        public Task RemoveFile(string route, string container, string webRootPath)
        {
            if (string.IsNullOrEmpty(route))
                return Task.CompletedTask;

            var fileName = Path.GetFileName(route);

            var directoryFile = Path.Combine(webRootPath, container, fileName);

            if (File.Exists(directoryFile))
                File.Delete(directoryFile);

            return Task.CompletedTask;
        }
    }
}
