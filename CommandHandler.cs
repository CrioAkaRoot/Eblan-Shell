using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class CommandHandler
{
    private readonly Dictionary<string, Func<string[], string, Task>> _commands;
    private readonly SystemCommands _systemCommands;
    private readonly FileCommands _fileCommands;
    private readonly TextEditor _textEditor;
    private readonly List<string> _commandHistory = new List<string>();

    public CommandHandler()
    {
        _systemCommands = new SystemCommands();
        _fileCommands = new FileCommands();
        _textEditor = new TextEditor();
        
        _commands = new Dictionary<string, Func<string[], string, Task>>
        {
            { "help", (_, __) => Task.Run(() => ShowHelp()) },
            { "clear", (_, __) => Task.Run(() => ConsoleHelper.Clear()) },
            { "whoami", (_, user) => Task.Run(() => ShowWhoami(user)) },
            { "date", (_, __) => Task.Run(() => _systemCommands.ShowDateTime()) },
            { "sysinfo", (_, __) => Task.Run(() => _systemCommands.ShowSystemInfo()) },
            { "calc", (_, __) => Task.Run(() => _systemCommands.Calculator()) },
            { "neofetch", (_, user) => Task.Run(() => _systemCommands.Neofetch(user)) },
            { "ls", (args, __) => Task.Run(() => _fileCommands.ListDirectory(args.Length > 1 ? args[1] : ".")) },
            { "cd", (args, __) => Task.Run(() => _fileCommands.ChangeDirectory(args.Length > 1 ? args[1] : "~")) },
            { "pwd", (_, __) => Task.Run(() => _fileCommands.PrintWorkingDirectory()) },
            { "cat", (args, __) => Task.Run(() => _fileCommands.ReadFile(args.Length > 1 ? args[1] : string.Empty)) },
            { "mkdir", (args, __) => Task.Run(() => _fileCommands.CreateDirectory(args.Length > 1 ? args[1] : string.Empty)) },
            { "rm", (args, __) => Task.Run(() => _fileCommands.RemoveFile(args.Length > 1 ? args[1] : string.Empty)) },
            { "touch", (args, __) => Task.Run(() => _fileCommands.CreateFile(args.Length > 1 ? args[1] : string.Empty)) },
            { "cp", (args, __) => Task.Run(() => _fileCommands.CopyFile(args)) },
            { "mv", (args, __) => Task.Run(() => _fileCommands.MoveFile(args)) },
            { "echo", (args, __) => Task.Run(() => Console.WriteLine(string.Join(" ", args.Skip(1)))) },
            { "grep", (args, __) => Task.Run(() => _fileCommands.Grep(args)) },
            { "edit", (args, __) => _textEditor.RunAsync(args.Length > 1 ? args[1] : string.Empty) },
            { "htop", (_, __) => Task.Run(() => _systemCommands.ShowResourceUsage()) },
            { "zip", (args, __) => Task.Run(() => _fileCommands.CreateZipArchive(args)) },
            { "unzip", (args, __) => Task.Run(() => _fileCommands.ExtractZipArchive(args.Length > 1 ? args[1] : string.Empty)) },
            { "find", (args, __) => Task.Run(() => _fileCommands.FindFiles(args.Length > 1 ? args[1] : string.Empty)) },
            { "genpass", (args, __) => Task.Run(() => _systemCommands.GeneratePassword(args.Length > 1 ? args[1] : "12")) },
            { "history", (_, __) => Task.Run(() => ShowHistory()) },
            { "tree", (args, __) => Task.Run(() => _fileCommands.ShowDirectoryTree(args.Length > 1 ? args[1] : ".")) },
            { "ping", (args, _) => _systemCommands.PingHostAsync(args) }
        };
    }

    public async Task HandleCommandAsync(string input, string currentUser)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                _commandHistory.Add(input);
            }
            
            string[] parts = input.Split(' ');
            string command = parts[0].ToLower();

            if (_commands.TryGetValue(command, out var handler))
            {
                await handler(parts, currentUser);
            }
            else
            {
                ConsoleHelper.WriteLineColored($"Command '{command}' not found. Type 'help' to see available commands.", ConsoleColor.Red);
            }
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteLineColored($"Error executing command: {ex.Message}", ConsoleColor.Red);
        }
    }

    private void ShowHelp()
    {
        ConsoleHelper.WriteLineColored("Available commands:", ConsoleColor.Yellow);
        ConsoleHelper.WriteLineColored("help - show list of commands", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("exit - exit the system", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("clear - clear the screen", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("whoami - show current username", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("date - show current date and time", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("sysinfo - show system information", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("calc - simple calculator", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("ls - list files in current directory", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("cd <dir> - change directory", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("pwd - print working directory", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("cat <file> - read file contents", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("mkdir <name> - create directory", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("rm <file> - remove file", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("touch <file> - create empty file", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("cp <source> <dest> - copy file", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("mv <source> <dest> - move file", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("echo <text> - display text", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("grep <pattern> <file> - search text in file", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("edit <file> - text editor", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("htop - show system resource usage", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("zip <archive> <file/dir> - create zip archive", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("unzip <archive> - extract zip archive", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("find <pattern> - find files by pattern", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("genpass <length> - generate random password", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("neofetch - display system information in fancy way", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("history - show command history", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("tree <dir> - show directory tree", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("ping <host/ip> - ping specified host", ConsoleColor.White);
    }

    private void ShowWhoami(string user)
    {
        ConsoleHelper.WriteLineColored($"Current user: {user}", ConsoleColor.Cyan);
    }

    private void ShowHistory()
    {
        for (int i = 0; i < _commandHistory.Count; i++)
        {
            ConsoleHelper.WriteLineColored($"{i + 1}: {_commandHistory[i]}", ConsoleColor.White);
        }
    }
} 