﻿
using System.Text.Json.Serialization;

namespace RWS_LBE_Transaction.DTOs.Auth.Responses
{
    public class AuthResponseData
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }
    }
}
