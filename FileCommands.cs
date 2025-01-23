using System;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

public class FileCommands
{
    public void ListDirectory(string path = ".")
    {
        try
        {
            var directory = new DirectoryInfo(path);
            foreach (var dir in directory.GetDirectories())
            {
                ConsoleHelper.WriteLineColored(dir.Name + "/", ConsoleColor.Blue);
            }
            foreach (var file in directory.GetFiles())
            {
                ConsoleHelper.WriteLineColored(file.Name, ConsoleColor.White);
            }
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteLineColored($"Error: {ex.Message}", ConsoleColor.Red);
        }
    }

    public void ChangeDirectory(string path)
    {
        try
        {
            if (path == "~")
                path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                
            Directory.SetCurrentDirectory(path);
            ConsoleHelper.WriteLineColored($"Changed directory to: {Directory.GetCurrentDirectory()}", ConsoleColor.Green);
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteLineColored($"Error: {ex.Message}", ConsoleColor.Red);
        }
    }

    public void PrintWorkingDirectory()
    {
        ConsoleHelper.WriteLineColored($"Current directory: {Directory.GetCurrentDirectory()}", ConsoleColor.Cyan);
    }

    public void ReadFile(string filename)
    {
        if (string.IsNullOrEmpty(filename))
        {
            ConsoleHelper.WriteLineColored("Usage: cat <filename>", ConsoleColor.Red);
            return;
        }

        try
        {
            string content = File.ReadAllText(filename);
            ConsoleHelper.WriteLineColored($"Contents of {filename}:", ConsoleColor.Cyan);
            Console.WriteLine(content);
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteLineColored($"Error: {ex.Message}", ConsoleColor.Red);
        }
    }

    public void CreateDirectory(string dirname)
    {
        if (string.IsNullOrEmpty(dirname))
        {
            ConsoleHelper.WriteLineColored("Usage: mkdir <dirname>", ConsoleColor.Red);
            return;
        }

        try
        {
            Directory.CreateDirectory(dirname);
            ConsoleHelper.WriteLineColored($"Created directory: {dirname}", ConsoleColor.Green);
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteLineColored($"Error: {ex.Message}", ConsoleColor.Red);
        }
    }

    public void RemoveFile(string filename)
    {
        if (string.IsNullOrEmpty(filename))
        {
            ConsoleHelper.WriteLineColored("Usage: rm <filename>", ConsoleColor.Red);
            return;
        }

        try
        {
            File.Delete(filename);
            ConsoleHelper.WriteLineColored($"Removed file: {filename}", ConsoleColor.Green);
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteLineColored($"Error: {ex.Message}", ConsoleColor.Red);
        }
    }

    public void CreateFile(string filename)
    {
        if (string.IsNullOrEmpty(filename))
        {
            ConsoleHelper.WriteLineColored("Usage: touch <filename>", ConsoleColor.Red);
            return;
        }

        try
        {
            File.Create(filename).Dispose();
            ConsoleHelper.WriteLineColored($"Created file: {filename}", ConsoleColor.Green);
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteLineColored($"Error: {ex.Message}", ConsoleColor.Red);
        }
    }

    public void CopyFile(string[] args)
    {
        if (args.Length != 3)
        {
            ConsoleHelper.WriteLineColored("Usage: cp <source> <destination>", ConsoleColor.Red);
            return;
        }

        try
        {
            File.Copy(args[1], args[2], true);
            ConsoleHelper.WriteLineColored($"Copied {args[1]} to {args[2]}", ConsoleColor.Green);
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteLineColored($"Error: {ex.Message}", ConsoleColor.Red);
        }
    }

    public void MoveFile(string[] args)
    {
        if (args.Length != 3)
        {
            ConsoleHelper.WriteLineColored("Usage: mv <source> <destination>", ConsoleColor.Red);
            return;
        }

        try
        {
            File.Move(args[1], args[2]);
            ConsoleHelper.WriteLineColored($"Moved {args[1]} to {args[2]}", ConsoleColor.Green);
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteLineColored($"Error: {ex.Message}", ConsoleColor.Red);
        }
    }

    public void Grep(string[] args)
    {
        if (args.Length != 3)
        {
            ConsoleHelper.WriteLineColored("Usage: grep <pattern> <file>", ConsoleColor.Red);
            return;
        }

        try
        {
            string pattern = args[1];
            string filename = args[2];
            var lines = File.ReadAllLines(filename);
            
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(pattern))
                {
                    ConsoleHelper.WriteLineColored($"{i + 1}: {lines[i]}", ConsoleColor.Green);
                }
            }
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteLineColored($"Error: {ex.Message}", ConsoleColor.Red);
        }
    }

    public void CreateZipArchive(string[] args)
    {
        if (args.Length != 3)
        {
            ConsoleHelper.WriteLineColored("Usage: zip <archive> <file/dir>", ConsoleColor.Red);
            return;
        }

        try
        {
            string archiveName = args[1];
            string target = args[2];

            if (!archiveName.EndsWith(".zip"))
                archiveName += ".zip";

            if (File.Exists(target))
            {
                using (var archive = ZipFile.Open(archiveName, ZipArchiveMode.Create))
                {
                    archive.CreateEntryFromFile(target, Path.GetFileName(target));
                }
            }
            else if (Directory.Exists(target))
            {
                ZipFile.CreateFromDirectory(target, archiveName);
            }

            ConsoleHelper.WriteLineColored($"Created archive: {archiveName}", ConsoleColor.Green);
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteLineColored($"Error: {ex.Message}", ConsoleColor.Red);
        }
    }

    public void ExtractZipArchive(string archiveName)
    {
        if (string.IsNullOrEmpty(archiveName))
        {
            ConsoleHelper.WriteLineColored("Usage: unzip <archive>", ConsoleColor.Red);
            return;
        }

        try
        {
            if (!archiveName.EndsWith(".zip"))
                archiveName += ".zip";

            ZipFile.ExtractToDirectory(archiveName, Directory.GetCurrentDirectory(), true);
            ConsoleHelper.WriteLineColored($"Extracted archive: {archiveName}", ConsoleColor.Green);
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteLineColored($"Error: {ex.Message}", ConsoleColor.Red);
        }
    }

    public void FindFiles(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            ConsoleHelper.WriteLineColored("Usage: find <pattern>", ConsoleColor.Red);
            return;
        }

        try
        {
            ConsoleHelper.WriteLineColored($"Searching for files matching '{pattern}':", ConsoleColor.Cyan);
            bool found = false;

            foreach (string file in Directory.GetFiles(".", "*", SearchOption.AllDirectories))
            {
                if (Path.GetFileName(file).Contains(pattern, StringComparison.OrdinalIgnoreCase))
                {
                    found = true;
                    if (Directory.Exists(file))
                        ConsoleHelper.WriteLineColored(file, ConsoleColor.Blue);
                    else
                        Console.WriteLine(file);
                }
            }

            if (!found)
                ConsoleHelper.WriteLineColored("No matching files found.", ConsoleColor.Yellow);
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteLineColored($"Error: {ex.Message}", ConsoleColor.Red);
        }
    }

    public void ShowDirectoryTree(string path = ".", string indent = "", bool isLast = true)
    {
        var dir = new DirectoryInfo(path);
        
        ConsoleHelper.WriteColored(indent, ConsoleColor.White);
        ConsoleHelper.WriteColored(isLast ? "└── " : "├── ", ConsoleColor.White);
        ConsoleHelper.WriteLineColored(dir.Name, ConsoleColor.Blue);

        var subDirs = dir.GetDirectories();
        var files = dir.GetFiles();
        
        for (int i = 0; i < subDirs.Length; i++)
        {
            ShowDirectoryTree(
                subDirs[i].FullName,
                indent + (isLast ? "    " : "│   "),
                i == subDirs.Length - 1 && files.Length == 0
            );
        }

        var nextIndent = indent + (isLast ? "    " : "│   ");
        for (int i = 0; i < files.Length; i++)
        {
            ConsoleHelper.WriteColored(nextIndent, ConsoleColor.White);
            ConsoleHelper.WriteColored(i == files.Length - 1 ? "└── " : "├── ", ConsoleColor.White);
            ConsoleHelper.WriteLineColored(files[i].Name, ConsoleColor.White);
        }
    }
} 