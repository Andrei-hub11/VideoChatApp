using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Runtime.Serialization;

namespace VideoChatApp.Contracts.Models;

public class UserMapping
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string ProfileImageUrl { get; set; } = string.Empty;

    [JsonExtensionData]
    private IDictionary<string, JToken> _additionalData;

    [OnDeserialized]
    internal void OnDeserialized(StreamingContext context)
    {
        if (_additionalData != null && _additionalData.TryGetValue("attributes", out var attributesToken))
        {
            var attributes = attributesToken.ToObject<Dictionary<string, JToken>>();
            if (attributes != null && attributes.TryGetValue("profile-image-url", out var profileImageUrlToken))
            {
                if (profileImageUrlToken.Type == JTokenType.Array)
                {
                    ProfileImageUrl = profileImageUrlToken.FirstOrDefault()?.ToString() ?? string.Empty;
                }
                else
                {
                    ProfileImageUrl = profileImageUrlToken.ToString();
                }
            }
        }
    }
}


