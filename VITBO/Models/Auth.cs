using System.Text.Json.Serialization;

namespace VITBO.Models
{
    public class PostModel
    {
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
    }

    public class PostModelResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;

        [JsonPropertyName("expires")]
        public DateTime ExpirationTime { get; set; }
    }
}
