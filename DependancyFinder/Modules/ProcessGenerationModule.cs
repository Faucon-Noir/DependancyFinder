using Newtonsoft.Json;
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

                    Dictionary<string, object> allOutputs = new Dictionary<string, object>();

                    foreach (string file in files)
                    {
                        var output = await StoredProcedures.GenerateStoredProceduresAsync(file, outputPath);
                        var outputJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(output);
                        allOutputs.Add(SplitFilePath(file).Item2, outputJson!);
                    }

                    string json = JsonConvert.SerializeObject(allOutputs, Formatting.Indented);
                    IsValidDirectory(outputPath);
                    File.WriteAllText(Path.Combine(outputPath, "all.json"), json);
                }
                else
                {
                    string fileName = SplitFilePath(inputPath).Item2;
                    string json = await StoredProcedures.GenerateStoredProceduresAsync(inputPath, outputPath);
                    IsValidDirectory(outputPath);
                    File.WriteAllText(Path.Combine(outputPath, $"{fileName}.json"), json);
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