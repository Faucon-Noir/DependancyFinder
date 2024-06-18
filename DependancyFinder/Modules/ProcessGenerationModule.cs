using static DependancyFinder.Modules.EnumModule;

namespace DependancyFinder.Modules.Generator
{
    public class ProcessGenerationModule
    {
        public static async Task<Task> ProcessGeneration(Options options)
        {
            string inputPath = options.InputPath;
            string outputPath = options.OutputPath ?? "./";
            try
            {
                if (Directory.Exists(inputPath))
                {
                    string[] files = Directory.GetFiles(inputPath, "*.sql");
                    foreach (string file in files)
                    {
                        await StoredProcedures.GenerateStoredProceduresAsync(file, outputPath);
                    }
                }
                else
                {
                    await StoredProcedures.GenerateStoredProceduresAsync(inputPath, outputPath);
                }

                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                CustomWriteLine(UsageEnum.Error, $"Error Processing: {e.Message}");
                throw;
            }
        }
    }
}