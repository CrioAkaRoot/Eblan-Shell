using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

public class TextEditor
{
    private List<string> _lines;
    private int _currentLine;
    private int _currentColumn;
    private bool _isInsertMode;
    private string _filename;
    private bool _isModified;

    public async Task RunAsync(string filename)
    {
        if (string.IsNullOrEmpty(filename))
        {
            ConsoleHelper.WriteLineColored("Error: Filename not specified", ConsoleColor.Red);
            return;
        }

        _filename = filename;
        _lines = new List<string>();
        _currentLine = 0;
        _currentColumn = 0;
        _isInsertMode = false;
        _isModified = false;

        if (File.Exists(filename))
        {
            _lines.AddRange(await File.ReadAllLinesAsync(filename));
        }
        if (_lines.Count == 0) _lines.Add("");

        ConsoleHelper.Clear();
        ShowHelp();
        
        while (true)
        {
            RefreshScreen();
            var key = Console.ReadKey(true);

            if (!_isInsertMode)
            {

                switch (key.Key)
                {
                    case ConsoleKey.I:
                        _isInsertMode = true;
                        break;
                    case ConsoleKey.Q:
                        if (!_isModified || ConfirmExit())
                        {
                            ConsoleHelper.Clear(); 
                            return;
                        }
                        break;
                    case ConsoleKey.S when key.Modifiers == ConsoleModifiers.Control:
                        await SaveFileAsync();
                        break;
                    case ConsoleKey.UpArrow:
                        MoveCursor(-1, 0);
                        break;
                    case ConsoleKey.DownArrow:
                        MoveCursor(1, 0);
                        break;
                    case ConsoleKey.LeftArrow:
                        MoveCursor(0, -1);
                        break;
                    case ConsoleKey.RightArrow:
                        MoveCursor(0, 1);
                        break;
                    case ConsoleKey.D when key.Modifiers == ConsoleModifiers.Control:
                        if (_lines.Count > 1)
                        {
                            _lines.RemoveAt(_currentLine);
                            _isModified = true;
                        }
                        break;
                }
            }
            else
            {
                
                switch (key.Key)
                {
                    case ConsoleKey.Escape:
                        _isInsertMode = false;
                        break;
                    case ConsoleKey.Enter:
                        string currentLine = _lines[_currentLine];
                        string newLine = currentLine.Substring(_currentColumn);
                        _lines[_currentLine] = currentLine.Substring(0, _currentColumn);
                        _lines.Insert(_currentLine + 1, newLine);
                        _currentLine++;
                        _currentColumn = 0;
                        _isModified = true;
                        break;
                    case ConsoleKey.Backspace:
                        if (_currentColumn > 0)
                        {
                            string line = _lines[_currentLine];
                            _lines[_currentLine] = line.Remove(_currentColumn - 1, 1);
                            _currentColumn--;
                            _isModified = true;
                        }
                        else if (_currentLine > 0)
                        {
                            string currentLineContent = _lines[_currentLine];
                            _lines.RemoveAt(_currentLine);
                            _currentLine--;
                            _currentColumn = _lines[_currentLine].Length;
                            _lines[_currentLine] += currentLineContent;
                            _isModified = true;
                        }
                        break;
                    default:
                        if (key.KeyChar >= 32 && key.KeyChar <= 126)
                        {
                            string line = _lines[_currentLine];
                            _lines[_currentLine] = line.Insert(_currentColumn, key.KeyChar.ToString());
                            _currentColumn++;
                            _isModified = true;
                        }
                        break;
                }
            }
        }
    }

    private void ShowHelp()
    {
        ConsoleHelper.WriteLineColored("Eblan Editor - Commands:", ConsoleColor.Cyan);
        ConsoleHelper.WriteLineColored("ESC - command mode", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("i - insert mode", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("Ctrl+S - save file", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("q - quit", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("Ctrl+D - delete line", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("Arrows - move cursor", ConsoleColor.White);
        Console.WriteLine();
    }

    private void RefreshScreen()
    {
        Console.SetCursorPosition(0, 7);
        ConsoleHelper.WriteLineColored($"File: {_filename} {(_isModified ? "[Modified]" : "")} | Mode: {(_isInsertMode ? "Insert" : "Command")}", ConsoleColor.Yellow);
        Console.WriteLine(new string('-', Console.WindowWidth));

        for (int i = 0; i < _lines.Count; i++)
        {
            Console.WriteLine(_lines[i].PadRight(Console.WindowWidth));
        }

        for (int i = 0; i < Console.WindowHeight - _lines.Count - 10; i++)
        {
            Console.WriteLine(new string(' ', Console.WindowWidth));
        }

        Console.SetCursorPosition(_currentColumn, _currentLine + 9);
    }

    private void MoveCursor(int deltaLine, int deltaColumn)
    {
        _currentLine = Math.Max(0, Math.Min(_lines.Count - 1, _currentLine + deltaLine));
        _currentColumn = Math.Max(0, Math.Min(_lines[_currentLine].Length, _currentColumn + deltaColumn));
    }

    private async Task SaveFileAsync()
    {
        try
        {
            await File.WriteAllLinesAsync(_filename, _lines);
            _isModified = false;
            ConsoleHelper.WriteLineColored($"\nFile saved: {_filename}", ConsoleColor.Green);
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteLineColored($"\nError saving file: {ex.Message}", ConsoleColor.Red);
        }
    }

    private bool ConfirmExit()
    {
        Console.SetCursorPosition(0, Console.WindowHeight - 1);
        ConsoleHelper.WriteLineColored("There are unsaved changes. Quit without saving? (y/n)", ConsoleColor.Red);
        var result = Console.ReadKey(true).Key == ConsoleKey.Y;
        if (result)
        {
            ConsoleHelper.Clear(); 
        }
        return result;
    }
} 