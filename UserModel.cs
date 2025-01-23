using System.Text.Json.Serialization;

public class UserModel
{
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;
} 