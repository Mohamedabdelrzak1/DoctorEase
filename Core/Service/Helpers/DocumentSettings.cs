using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace Service.Helpers
{
    public static class ImageSettings
    {
        


        public static string UploadImage(IFormFile image, string folderName)
        {
            if (image == null || image.Length == 0)
                throw new ArgumentException("📷 Image is empty or not provided.");

           
            if (!image.ContentType.StartsWith("image/"))
                throw new ArgumentException("📷 The uploaded file must be an image.");

            
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", folderName);

            
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

           
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";

            
            var filePath = Path.Combine(folderPath, fileName);

            
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                image.CopyTo(stream);
            }

           
            return fileName;
        }

       
        public static void DeleteImage(string fileName, string folderName)
        {
            if (string.IsNullOrEmpty(fileName))
                return;

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", folderName, fileName);

            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
