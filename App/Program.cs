global using static DependencyFinder.App.Utils.Utility;
using DependencyFinder.App.Utils;
using DependencyFinder.App;
using CommandLine;
using System.Diagnostics;
using static DependencyFinder.App.Utils.EnumUtils;

namespace DependencyFinder.Tool;

public class Program
{
    public static async Task Main(string[] args)
    {
        Stopwatch stopwatch = new();
        try
        {
            stopwatch.Start();
            await Parser.Default.ParseArguments<Options>(args).WithParsedAsync(async o =>
           {
               var utilityModule = new Utility();
               var errorMessages = EntryValidation(o);
               foreach (var errorMessage in errorMessages)
               {
                   Console.WriteLine(errorMessage);
               }
               await ProcessAnalyze.ProcessAnalyzeAsync(o);
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
