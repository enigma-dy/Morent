using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace MoRent_V2.Services;

public class CloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IConfiguration configuration)
    {
        var cloudName = configuration["Cloudinary:CloudName"];
        var apiKey = configuration["Cloudinary:ApiKey"];
        var apiSecret = configuration["Cloudinary:ApiSecret"];

        if (string.IsNullOrEmpty(cloudName) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
        {
            throw new ArgumentNullException("Cloudinary configuration values are missing.");
        }

        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadImageAsync(IFormFile image)
    {
        using var stream = image.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(image.FileName, stream),
            Folder = "profile_pictures"
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        return uploadResult.SecureUrl.AbsoluteUri;
    }

    public async Task<List<string>> UploadImagesAsync(List<IFormFile> images)
    {
        var uploadedUrls = new List<string>();

        foreach (var image in images)
        {
            uploadedUrls.Add(await UploadImageAsync(image));
        }

        return uploadedUrls;
    }
}