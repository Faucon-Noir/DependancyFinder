global using static DependencyFinder.Tool.Modules.UtilityModule;
using CommandLine;
using DependencyFinder.Tool.Modules;
using System.Diagnostics;
using static DependencyFinder.Tool.Modules.EnumModule;

namespace DependencyFinder.Tool;

public class Program
{
    public static async Task Main(string[] args)
    {
        Stopwatch stopwatch = new Stopwatch();
        try
        {
            stopwatch.Start();
            CustomWriteLine(UsageEnum.Info, "--------------------------------------------- Program Start ---------------------------------------------");
            await Parser.Default.ParseArguments<Options>(args).WithParsedAsync(async o =>
           {
               var utilityModule = new UtilityModule();
               var errorMessages = utilityModule.EntryValidation(o);
               foreach (var errorMessage in errorMessages)
               {
                   Console.WriteLine(errorMessage);
               }
               await ProcessGenerationModule.ProcessGeneration(o);
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
