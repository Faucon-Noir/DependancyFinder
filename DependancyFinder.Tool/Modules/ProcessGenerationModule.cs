using DependencyFinder.Tool.Modules.Generator;
using System.Text.Json;
using static DependencyFinder.Tool.Modules.EnumModule;
using DependencyFinder.Tool.Modules.Entities;

namespace DependencyFinder.Tool.Modules
{
    public class ProcessGenerationModule
    {
        public static async Task<Task> ProcessGeneration(Options options)
        {
            string inputPath = options.InputPath;
            string outputPath = options.OutputPath;

            try
            {
                CustomWriteLine(UsageEnum.Processing, $"Output {outputPath}");
                if (Directory.Exists(inputPath))
                {
                    string[] files = Directory.GetFiles(inputPath, "*.sql");

                    Dictionary<string, object> allOutputs = new Dictionary<string, object>();

                    foreach (string file in files)
                    {
                        var output = await StoredProcedures.GenerateStoredProceduresAsync(file);
                        string outputString = JsonSerializer.Serialize(output);
                        var outputJson = JsonSerializer.Deserialize<Dictionary<string, object>>(outputString);
                        allOutputs.Add(SplitFilePath(file).Item2, outputJson!);
                    }

                    string json = JsonSerializer.Serialize(allOutputs, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });
                    IsValidDirectory(outputPath);
                    File.WriteAllText(Path.Combine(outputPath, "all.json"), json);
                }
                else
                {
                    string fileName = SplitFilePath(inputPath).Item2;
                    SPEntity objectToSerialize = await StoredProcedures.GenerateStoredProceduresAsync(inputPath);
                    string json = JsonSerializer.Serialize(objectToSerialize, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });
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