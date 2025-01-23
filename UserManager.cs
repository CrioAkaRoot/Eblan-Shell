using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

public class UserManager
{
    private const string USER_FILE = "user_config.json";

    public async Task<string> GetOrCreateUserAsync()
    {
        
        if (File.Exists(USER_FILE))
        {
            try
            {
                string jsonString = await File.ReadAllTextAsync(USER_FILE);
                var userModel = JsonSerializer.Deserialize<UserModel>(jsonString);
                if (!string.IsNullOrEmpty(userModel?.Username))
                {
                    ConsoleHelper.WriteColored($"Welcome ", ConsoleColor.White);
                    ConsoleHelper.WriteColored(userModel.Username, ConsoleColor.Magenta);
                    ConsoleHelper.WriteLineColored("!", ConsoleColor.White);
                    return userModel.Username;
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteLineColored($"Error 0000x2: {ex.Message}", ConsoleColor.Red);
            }
        }

        ConsoleHelper.WriteLineColored("Write your name:", ConsoleColor.Cyan);
        string username = Console.ReadLine()?.Trim() ?? "User";

        while (string.IsNullOrEmpty(username))
        {
            ConsoleHelper.WriteLineColored("Error 0000x1:", ConsoleColor.Red);
            username = Console.ReadLine()?.Trim() ?? "User";
        }

        var user = new UserModel { Username = username };
        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(user, jsonOptions);
        await File.WriteAllTextAsync(USER_FILE, json);

        ConsoleHelper.WriteColored($"Welcome ", ConsoleColor.White);
        ConsoleHelper.WriteColored(username, ConsoleColor.Magenta);
        ConsoleHelper.WriteLineColored("!", ConsoleColor.White);
        return username;
    }
} 