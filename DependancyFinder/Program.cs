global using static DependancyFinder.Modules.UtilityModule;
using CommandLine;
using DependancyFinder.Modules.Generator;
using DependancyFinder.Modules;
using System.Diagnostics;
using static DependancyFinder.Modules.EnumModule;

namespace DependancyFinder
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                stopwatch.Start();
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
                CustomWriteLine(UsageEnum.Error, $"Error Program: {e.Message}\nCheck {e.TargetSite}");
            }
        }
    }
}
