public static class ConsoleHelper
{
    public static void Clear()
    {
        Console.Clear();
    }

    public static void WriteColored(string text, ConsoleColor color)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ForegroundColor = originalColor;
    }

    public static void WriteLineColored(string text, ConsoleColor color)
    {
        WriteColored(text + Environment.NewLine, color);
    }

    public static void WriteRainbow(string text)
    {
        ConsoleColor[] colors = {
            ConsoleColor.Red,
            ConsoleColor.Yellow,
            ConsoleColor.Green,
            ConsoleColor.Cyan,
            ConsoleColor.Blue,
            ConsoleColor.Magenta
        };

        int colorIndex = 0;
        foreach (char c in text)
        {
            if (c != ' ' && c != '\n')
            {
                WriteColored(c.ToString(), colors[colorIndex]);
                colorIndex = (colorIndex + 1) % colors.Length;
            }
            else
            {
                Console.Write(c);
            }
        }
    }
} 