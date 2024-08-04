using VideoChatApp.Application.Contracts.Services;

namespace NerdCritica.Application.Services.ImageServiceConfiguration;

public class ImageServiceConfiguration : IImageServiceConfiguration
{
    public string ApiRootDirectory { get; }

    public ImageServiceConfiguration(string baseDirectory)
    {
        ApiRootDirectory = FindApiRootDirectory(baseDirectory);
    }

    private static string FindApiRootDirectory(string baseDirectory)
    {
        string apiFolderName = "VideoChatApp.Api";

        var currentDirectory = baseDirectory;

        while (!string.IsNullOrWhiteSpace(currentDirectory))
        {
            var subdirectories = Directory.GetDirectories(currentDirectory);

            foreach (var subdirectory in subdirectories)
            {
                if (Path.GetFileName(subdirectory).Equals(apiFolderName, StringComparison.OrdinalIgnoreCase))
                {
                    return subdirectory;
                }
            }

            currentDirectory = Directory.GetParent(currentDirectory)?.FullName;
        }

        throw new DirectoryNotFoundException($"The directory '{apiFolderName}' was not found under '{baseDirectory}'.");
    }
}

