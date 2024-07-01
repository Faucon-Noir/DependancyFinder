using System.Text.Json;
using static DependencyFinder.Tool.Utils.Enum;
using DependencyFinder.Tool.Controller;
using DependencyFinder.Tool.Entities;
using DependencyFinder.Tool.Generator;
using DependencyFinder.Tool.Utils;
using System.Text.RegularExpressions;

namespace DependencyFinder.Tool;

public class Process
{
    public static async Task<Task> ProcessAnalyze(Options options)
    {
        string inputPath = options.InputPath;
        string outputPath = options.OutputPath;
        bool gptReport = options.GPTReport;
        Environment.SetEnvironmentVariable("Verbose", options.Verbose.ToString());
        JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true
        };

        try
        {
            // Folder processing
            if (Directory.Exists(inputPath))
            {
                string[] files = Directory.GetFiles(inputPath, "*.sql");

                Dictionary<string, object> allOutputs = [];

                foreach (string file in files)
                {
                    var output = await SqlAnalyzer.AnalyzeSqlAsync(file, gptReport);
                    string outputString = JsonSerializer.Serialize(output);
                    var outputJson = JsonSerializer.Deserialize<Dictionary<string, object>>(outputString);
                    allOutputs.Add(SplitFilePath(file).Item2, outputJson!);
                }

                string json = JsonSerializer.Serialize(allOutputs, jsonSerializerOptions);
                IsValidDirectory(outputPath);
                File.WriteAllText(Path.Combine(outputPath, "all.json"), json);
            }
            // Single file processing
            else
            {
                string fileName = SplitFilePath(inputPath).Item2;
                SPEntity sqlAnalysisData = await SqlAnalyzer.AnalyzeSqlAsync(inputPath, gptReport);

                // On laisse uniquement pour l'objet racine dans un premier temps, on verra pour les enfants plus tard


                string json = JsonSerializer.Serialize(sqlAnalysisData, jsonSerializerOptions);
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