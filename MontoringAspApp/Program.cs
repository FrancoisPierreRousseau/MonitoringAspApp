

using MontoringAspApp;

class Program
{
    static void Main(string[] args)
    {
        // Debugger.Launch();

        if(args.Length != 1)
        {
            Console.WriteLine("Usage: EventLoggerTerminal.exe <config_file.json>");
            return;
        }

        var configFile = args[0];
        var logger = new EventLoggerTerminal(configFile);
        logger.StartLogging();
    }
}

