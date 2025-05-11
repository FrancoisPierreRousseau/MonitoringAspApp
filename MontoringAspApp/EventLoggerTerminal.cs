using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MontoringAspApp;

class EventLoggerTerminal
{
    private string logType;
    private string[] filters;

    public EventLoggerTerminal(string configFile)
    {
        LoadConfig(configFile);
    }

    private void LoadConfig(string configFile)
    {
        var configContent = File.ReadAllText(configFile);
        dynamic config = JsonConvert.DeserializeObject(configContent);
        logType = config.log_type ?? "Application";
        filters = config.filters.ToObject<string[]>();
    }

    private bool MatchFilters(string message)
    {
        foreach(var filter in filters)
        {
            if(Regex.IsMatch(message, filter))
            {
                return true;
            }
        }
        return false;
    }

    public void StartLogging()
    {
        Console.WriteLine($"Starting to log {logType} events...");

        var eventLog = new EventLog(logType);
        eventLog.EntryWritten += new EntryWrittenEventHandler((sender, e) =>
        {
            if(e.Entry != null && MatchFilters(e.Entry.Source))
            {
                Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {e.Entry.Message}");
            }
        });
        eventLog.EnableRaisingEvents = true;

        while(true)
        {
            Thread.Sleep(1000);
        }
    }
}

