global using static App.Utils.Utility;
global using static App.Utils.FileUtils;
using App.Utils;
using App;
using CommandLine;
using System.Diagnostics;
using static App.Utils.EnumUtils;

namespace DependencyFinder.Tool;

public class Program
{
    public static async Task Main(string[] args)
    {
        CustomWriteLine(UsageEnum.Info, "--------------------------------------------- Program Start ---------------------------------------------");
        Stopwatch stopwatch = new();
        try
        {
            CustomWriteLine(UsageEnum.Log, "Parsing Arguments");
            stopwatch.Start();
            SetEnv();
            await Parser.Default.ParseArguments<Options>(args).WithParsedAsync(async o =>
           {
               Utility utilityModule = new();
               var errorMessages = EntryValidation(o);
               foreach (var errorMessage in errorMessages)
               {
                   Console.WriteLine(errorMessage);
               }
               var process = new ProcessAnalyze();
               await process.ProcessAnalyzeAsync(o);
           });
            stopwatch.Stop();
            CustomWriteLine(UsageEnum.Info, "\n--------------------------------------------- Program Complete ---------------------------------------------");
            CustomWriteLine(UsageEnum.Info, $"Execution Time: {stopwatch.ElapsedMilliseconds} ms\n");
        }
        catch (Exception e)
        {
            CustomWriteLine(UsageEnum.Error, $"Error Program: {e.Message}");
        }
    }
}
