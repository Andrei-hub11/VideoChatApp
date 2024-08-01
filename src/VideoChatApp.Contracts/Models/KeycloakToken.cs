﻿using Newtonsoft.Json;

namespace VideoChatApp.Contracts.Models;

public class KeycloakToken
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; } = string.Empty;
    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;
}

