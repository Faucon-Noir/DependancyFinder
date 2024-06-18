using static DependancyFinder.Modules.EnumModule;

namespace DependancyFinder.Modules.Generator
{
    public class ProcessGenerationModule
    {
        public static async Task<Task> ProcessGeneration(Options options)
        {
            string inputPath = options.InputPath;
            // string outputPath = options.OutputPath ?? inputPath;
            try
            {
                // J'ai besoin de construire un json qui contient le nom du fichier d'entrée, ses dépendances, et les dépendances des dépendances
                await StoredProcedures.GenerateStoredProceduresAsync(inputPath);
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                CustomWriteLine(UsageEnum.Error, $"Error Program: {e.Message}");
                throw;
            }
        }
    }
}