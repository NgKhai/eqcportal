using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using eqcportal.Models;
using Microsoft.Extensions.Options;

namespace eqcportal.Services
{
    public class CloudinaryImageStorageService : IImageStorageService
    {
        private readonly Cloudinary? _cloudinary;
        private readonly CloudinarySettings _settings;

        public CloudinaryImageStorageService(IOptions<CloudinarySettings> options)
        {
            _settings = options.Value;

            if (string.IsNullOrWhiteSpace(_settings.CloudName) ||
                string.IsNullOrWhiteSpace(_settings.ApiKey) ||
                string.IsNullOrWhiteSpace(_settings.ApiSecret))
            {
                return;
            }

            var account = new Account(_settings.CloudName, _settings.ApiKey, _settings.ApiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public bool IsConfigured => _cloudinary != null;

        public async Task<string?> UploadEmployeeAvatarAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            if (_cloudinary == null)
            {
                throw new InvalidOperationException("Cloudinary chưa được cấu hình.");
            }

            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = _settings.Folder,
                PublicId = $"employee_{Guid.NewGuid():N}",
                UseFilename = false,
                UniqueFilename = false,
                Overwrite = false,
                Transformation = new Transformation().Width(400).Height(400).Crop("fill").Gravity("face")
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams, cancellationToken);
            if (uploadResult.Error != null)
            {
                throw new InvalidOperationException(uploadResult.Error.Message);
            }

            return uploadResult.SecureUrl?.ToString();
        }

        public async Task DeleteEmployeeAvatarAsync(string? imageUrl, CancellationToken cancellationToken = default)
        {
            if (_cloudinary == null || string.IsNullOrWhiteSpace(imageUrl))
            {
                return;
            }

            var publicId = ExtractPublicId(imageUrl);
            if (string.IsNullOrWhiteSpace(publicId))
            {
                return;
            }

            var deletionParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Image
            };

            await _cloudinary.DestroyAsync(deletionParams);
        }

        private static string? ExtractPublicId(string imageUrl)
        {
            if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri))
            {
                return null;
            }

            var marker = "/upload/";
            var path = uri.AbsolutePath;
            var markerIndex = path.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (markerIndex < 0)
            {
                return null;
            }

            var publicPath = path[(markerIndex + marker.Length)..];
            var segments = publicPath.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();
            if (segments.Count == 0)
            {
                return null;
            }

            if (segments[0].StartsWith("v", StringComparison.OrdinalIgnoreCase) &&
                segments[0].Length > 1 &&
                segments[0][1..].All(char.IsDigit))
            {
                segments.RemoveAt(0);
            }

            if (segments.Count == 0)
            {
                return null;
            }

            var lastSegment = segments[^1];
            var dotIndex = lastSegment.LastIndexOf('.');
            if (dotIndex > 0)
            {
                segments[^1] = lastSegment[..dotIndex];
            }

            return string.Join('/', segments);
        }
    }
}
