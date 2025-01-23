using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Net.NetworkInformation;

public class SystemCommands
{
    public void ShowDateTime()
    {
        ConsoleHelper.WriteLineColored($"Current date and time: {DateTime.Now}", ConsoleColor.Yellow);
    }

    public void ShowSystemInfo()
    {
        ConsoleHelper.WriteLineColored("System Information:", ConsoleColor.Yellow);
        ConsoleHelper.WriteLineColored($"OS: {RuntimeInformation.OSDescription}", ConsoleColor.White);
        ConsoleHelper.WriteLineColored($"Architecture: {RuntimeInformation.ProcessArchitecture}", ConsoleColor.White);
        ConsoleHelper.WriteLineColored($"Framework: {RuntimeInformation.FrameworkDescription}", ConsoleColor.White);
    }

    public void Calculator()
    {
        ConsoleHelper.WriteLineColored("Simple Calculator Mode (Type 'exit' to quit)", ConsoleColor.Yellow);
        ConsoleHelper.WriteLineColored("Format: number operator number (e.g., 2 + 2)", ConsoleColor.Cyan);

        while (true)
        {
            Console.Write("Enter calculation: ");
            string input = Console.ReadLine()?.Trim().ToLower() ?? "";

            if (input == "exit")
                break;

            try
            {
                string[] parts = input.Split(' ');
                if (parts.Length != 3)
                    throw new FormatException();

                double num1 = double.Parse(parts[0]);
                double num2 = double.Parse(parts[2]);
                string op = parts[1];

                double result = op switch
                {
                    "+" => num1 + num2,
                    "-" => num1 - num2,
                    "*" => num1 * num2,
                    "/" => num2 != 0 ? num1 / num2 : throw new DivideByZeroException(),
                    _ => throw new ArgumentException("Invalid operator")
                };

                ConsoleHelper.WriteLineColored($"Result: {result}", ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteLineColored($"Error: {ex.Message}", ConsoleColor.Red);
            }
        }
    }

    public void ShowResourceUsage()
    {
        ConsoleHelper.WriteLineColored("System Resource Monitor (Press any key to exit)", ConsoleColor.Cyan);
        Console.WriteLine(new string('-', 50));

        var process = Process.GetCurrentProcess();
        var workingSet = process.WorkingSet64 / 1024.0 / 1024.0;
        var totalMemory = GC.GetTotalMemory(false) / 1024.0 / 1024.0;

        ConsoleHelper.WriteLineColored($"Process Memory Usage: {workingSet:F2} MB", ConsoleColor.White);
        ConsoleHelper.WriteLineColored($"Managed Memory: {totalMemory:F2} MB", ConsoleColor.White);
        
        Console.ReadKey();
    }

    public void GeneratePassword(string lengthStr)
    {
        try
        {
            if (!int.TryParse(lengthStr, out int length) || length < 4)
                throw new ArgumentException("Password length must be at least 4 characters");

            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()";
            var random = new Random();
            var password = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            ConsoleHelper.WriteLineColored($"Generated password: {password}", ConsoleColor.Green);
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteLineColored($"Error: {ex.Message}", ConsoleColor.Red);
        }
    }

    public void Neofetch(string user)
    {
        ConsoleHelper.Clear();
        
        var osInfo = RuntimeInformation.OSDescription;
        var hostname = Environment.MachineName;
        var framework = RuntimeInformation.FrameworkDescription;
        var uptime = GetUptime();
        var cpuInfo = GetCpuInfo();

        string asciiArt = GetOSAsciiArt(osInfo);
        ConsoleHelper.WriteRainbow(asciiArt);

        Console.SetCursorPosition(50, 1);
        ConsoleHelper.WriteColored($"{user}@{hostname}", ConsoleColor.Cyan);

        Console.SetCursorPosition(50, 3);
        ConsoleHelper.WriteColored("OS: ", ConsoleColor.White);
        ConsoleHelper.WriteLineColored(osInfo, ConsoleColor.Cyan);

        Console.SetCursorPosition(50, 4);
        ConsoleHelper.WriteColored("Host: ", ConsoleColor.White);
        ConsoleHelper.WriteLineColored(hostname, ConsoleColor.Cyan);

        Console.SetCursorPosition(50, 5);
        ConsoleHelper.WriteColored("Kernel: ", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("Unix " + Environment.OSVersion.Version.ToString(), ConsoleColor.Cyan);

        Console.SetCursorPosition(50, 6);
        ConsoleHelper.WriteColored("Uptime: ", ConsoleColor.White);
        ConsoleHelper.WriteLineColored(uptime, ConsoleColor.Cyan);

        Console.SetCursorPosition(50, 7);
        ConsoleHelper.WriteColored("Shell: ", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("Eblan Shell v1.0", ConsoleColor.Cyan);

        Console.SetCursorPosition(50, 8);
        ConsoleHelper.WriteColored("Terminal: ", ConsoleColor.White);
        ConsoleHelper.WriteLineColored("Eblan Terminal", ConsoleColor.Cyan);

        Console.SetCursorPosition(50, 9);
        ConsoleHelper.WriteColored("Framework: ", ConsoleColor.White);
        ConsoleHelper.WriteLineColored(framework, ConsoleColor.Cyan);

        Console.SetCursorPosition(50, 11);
        WriteColorBlocks();

        Console.WriteLine();
        Console.WriteLine();
    }

    private string GetUptime()
    {
        var uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);
        return $"{(int)uptime.TotalDays}d {uptime.Hours}h {uptime.Minutes}m";
    }

    private string GetMemoryInfo()
    {
        var process = Process.GetCurrentProcess();
        var totalPhysicalMemory = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / (1024 * 1024);
        var usedMemory = process.WorkingSet64 / (1024 * 1024);
        return $"{usedMemory}MiB / {totalPhysicalMemory}MiB";
    }

    private string GetCpuInfo()
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var cpuInfo = File.ReadAllLines("/proc/cpuinfo");
                var modelName = cpuInfo.FirstOrDefault(line => line.StartsWith("model name"));
                if (modelName != null)
                {
                    return modelName.Split(':')[1].Trim();
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "wmic",
                        Arguments = "cpu get name",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return output.Split('\n')[1].Trim();
            }
        }
        catch { }
        
        return "Unknown CPU";
    }

    private void WriteColorBlocks()
    {
        ConsoleColor[] colors = {
            ConsoleColor.DarkBlue, ConsoleColor.Red, ConsoleColor.Green, 
            ConsoleColor.Yellow, ConsoleColor.Blue, ConsoleColor.Magenta,
            ConsoleColor.Cyan, ConsoleColor.White
        };

        foreach (var color in colors)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write("███");
            Console.ForegroundColor = currentColor;
        }
    }

    private string GetOSAsciiArt(string osInfo)
    {
        return @"
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
    }

    public async Task PingHostAsync(string[] args)
    {
        if (args.Length != 2)
        {
            ConsoleHelper.WriteLineColored("Usage: ping <host/ip>", ConsoleColor.Red);
            return;
        }

        string host = args[1];
        ConsoleHelper.WriteLineColored($"Pinging {host}...\n", ConsoleColor.Cyan);

        int sent = 0;
        int received = 0;
        long totalTime = 0;
        long minTime = long.MaxValue;
        long maxTime = long.MinValue;

        try
        {
            var ping = new System.Net.NetworkInformation.Ping();
            var cancellation = new CancellationTokenSource();

            _ = Task.Run(() => 
            {
                Console.ReadKey(true);
                cancellation.Cancel();
            });

            while (!cancellation.Token.IsCancellationRequested)
            {
                try
                {
                    sent++;
                    var reply = await ping.SendPingAsync(host);
                    
                    if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                    {
                        received++;
                        totalTime += reply.RoundtripTime;
                        minTime = Math.Min(minTime, reply.RoundtripTime);
                        maxTime = Math.Max(maxTime, reply.RoundtripTime);

                        ConsoleHelper.WriteLineColored(
                            $"Reply from {reply.Address}: time={reply.RoundtripTime}ms TTL={reply.Options?.Ttl ?? 0}",
                            ConsoleColor.Green);
                    }
                    else
                    {
                        ConsoleHelper.WriteLineColored(
                            $"Failed: {reply.Status}",
                            ConsoleColor.Red);
                    }

                    await Task.Delay(1000, cancellation.Token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    ConsoleHelper.WriteLineColored($"Error: {ex.Message}", ConsoleColor.Red);
                }
            }

            Console.WriteLine();
            ConsoleHelper.WriteLineColored($"--- {host} ping statistics ---", ConsoleColor.Cyan);
            
            int lostPackets = sent - received;
            double lossPercentage = (double)lostPackets / sent * 100;
            double avgTime = received > 0 ? (double)totalTime / received : 0;

            ConsoleHelper.WriteLineColored(
                $"{sent} packets transmitted, {received} received, " +
                $"{lossPercentage:F1}% packet loss",
                ConsoleColor.White);

            if (received > 0)
            {
                ConsoleHelper.WriteLineColored(
                    $"rtt min/avg/max = {minTime:F1}/{avgTime:F1}/{maxTime:F1} ms",
                    ConsoleColor.White);
            }
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteLineColored($"Error: {ex.Message}", ConsoleColor.Red);
        }
    }
} 