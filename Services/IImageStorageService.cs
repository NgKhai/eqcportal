using Microsoft.AspNetCore.Http;

namespace eqcportal.Services
{
    public interface IImageStorageService
    {
        bool IsConfigured { get; }
        Task<string?> UploadEmployeeAvatarAsync(IFormFile file, CancellationToken cancellationToken = default);
        Task DeleteEmployeeAvatarAsync(string? imageUrl, CancellationToken cancellationToken = default);
    }
}
