using DependencyFinder.Tool.Modules.Generator;
using System.Text.Json;
using static DependencyFinder.Tool.Modules.EnumModule;
using DependencyFinder.Tool.Modules.Entities;
using DependancyFinder.Tool.Modules.ChatController;

namespace DependencyFinder.Tool.Modules
{
    public class ProcessGenerationModule
    {
        public static async Task<Task> ProcessGeneration(Options options)
        {
            string inputPath = options.InputPath;
            string outputPath = options.OutputPath;
            bool gptReport = options.GPTReport;
            Environment.SetEnvironmentVariable("Verbose", options.Verbose.ToString());

            try
            {
                // Folder processing
                if (Directory.Exists(inputPath))
                {
                    string[] files = Directory.GetFiles(inputPath, "*.sql");

                    Dictionary<string, object> allOutputs = [];

                    foreach (string file in files)
                    {
                        var output = await SqlAnalyzer.AnalyzeSqlAsync(file);
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
                // Single file processing
                else
                {
                    string fileName = SplitFilePath(inputPath).Item2;
                    SPEntity sqlAnalysisData = await SqlAnalyzer.AnalyzeSqlAsync(inputPath);

                    // On laisse uniquement pour l'objet racine dans un premier temps, on verra pour les enfants plus tard
                    if (gptReport)
                    {
                        SetEnv();
                        string chatId = Environment.GetEnvironmentVariable("CHAT_ID")!;
                        string token = Environment.GetEnvironmentVariable("TOKEN")!;
                        string chatUrl = Environment.GetEnvironmentVariable("CHAT_URL")!;
                        sqlAnalysisData.GPTReport = await ChatController.SendMessageAsync(chatId, token, File.ReadAllText(inputPath), chatUrl);
                    }

                    string json = JsonSerializer.Serialize(sqlAnalysisData, new JsonSerializerOptions
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