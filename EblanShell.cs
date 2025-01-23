using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;

public class EblanShell
{
    private readonly CommandHandler _commandHandler;
    private readonly UserManager _userManager;
    private string _currentUser = string.Empty;

    public EblanShell()
    {
        _commandHandler = new CommandHandler();
        _userManager = new UserManager();
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        DisplayStartupBanner();
    }

    private void DisplayStartupBanner()
    {
        ConsoleHelper.Clear();
        string banner = @"
  ________  __        __                      
 /        |/  |      /  |                     
 $$$$$$$$/ $$ |____  $$ |  ______   _______   
 $$ |__    $$      \ $$ | /      \ /       \  
 $$    |   $$$$$$$  |$$ | $$$$$$  |$$$$$$$  | 
 $$$$$/    $$ |  $$ |$$ | /    $$ |$$ |  $$ | 
 $$ |_____ $$ |__$$ |$$ |/$$$$$$$ |$$ |  $$ | 
 $$       |$$    $$/ $$ |$$    $$ |$$ |  $$ | 
 $$$$$$$$/ $$$$$$$/  $$/  $$$$$$$/ $$/   $$/  
 
";
        ConsoleHelper.WriteRainbow(banner);
        ConsoleHelper.WriteLineColored("Eblan Shell V1.0 By Root aka Crio", ConsoleColor.Green);
        Console.WriteLine();
        ConsoleHelper.WriteLineColored("Welcome to Eblan Shell", ConsoleColor.Cyan);
        Console.WriteLine();
    }

    public async Task RunAsync()
    {
        _currentUser = await _userManager.GetOrCreateUserAsync();
        ConsoleHelper.WriteLineColored("\nType 'help' to see available commands", ConsoleColor.Cyan);

        while (true)
        {
            
            ConsoleHelper.WriteColored($"{_currentUser}", ConsoleColor.Green);
            ConsoleHelper.WriteColored("@", ConsoleColor.Green);
            ConsoleHelper.WriteColored($"{Environment.MachineName}", ConsoleColor.Green);
            ConsoleHelper.WriteColored(":", ConsoleColor.White);
            
            string currentPath = Directory.GetCurrentDirectory().Replace(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "~");
            ConsoleHelper.WriteColored(currentPath, ConsoleColor.Blue);
            
            ConsoleHelper.WriteColored("$ ", ConsoleColor.White);
            
            string command = Console.ReadLine()?.Trim().ToLower() ?? "";

            if (command == "exit")
            {
                ConsoleHelper.WriteLineColored("Goodbye!", ConsoleColor.Red);
                break;
            }

            await _commandHandler.HandleCommandAsync(command, _currentUser);
        }
    }
} 