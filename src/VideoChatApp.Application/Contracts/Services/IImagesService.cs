using VideoChatApp.Contracts.Models;

namespace VideoChatApp.Application.Contracts.Services;

public interface IImagesService
{
    Task<ProfileImage> GetProfileImageAsync(string profileImage);
    Task DeleteProfileImageAsync(string relativeImagePath);
}
