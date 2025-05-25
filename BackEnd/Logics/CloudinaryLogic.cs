using System.Text.RegularExpressions;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DotNetEnv;
using BackEnd.Utils.Const;

namespace BackEnd.Logics;

public class CloudinaryLogic
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryLogic()
        {
            Env.Load();
            var account = new Account(Environment.GetEnvironmentVariable(EnvConst.CloudinaryCloudName), 
                Environment.GetEnvironmentVariable(EnvConst.CloudApiKey), 
                Environment.GetEnvironmentVariable(EnvConst.CloudApiSecret));
            _cloudinary = new Cloudinary(account);
        }

        /// <summary>
        /// Upload images to Cloudinary
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<string> UploadImageAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File not valid");
            }

            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream)
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            // Check for error
            if (uploadResult.Error != null)
            {
                throw new Exception($"Cloudinary upload failed: {uploadResult.Error.Message}");
            }

            // Return the URL
            return uploadResult.SecureUrl?.ToString() ?? throw new Exception("Upload failed");
        }
        
        /// <summary>
        /// Delete image 
        /// </summary>
        /// <returns></returns>
        public bool DeleteImage(string url)
        {
            var deletionParams = new DeletionParams(ExtractPublicId(url))
            {
                ResourceType = ResourceType.Image
            };

            var result = _cloudinary.Destroy(deletionParams);
            return result.Result == "ok"; 
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <returns></returns>
        private string ExtractPublicId(string imageUrl)
        {
            // Find Public Id
            var match = Regex.Match(imageUrl, @"/upload/v\d+/(.*)\..+$");
            return match.Success ? match.Groups[1].Value : null;
        }
    }