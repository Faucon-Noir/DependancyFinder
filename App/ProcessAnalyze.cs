using DependencyFinder.App.Analyzer;
using DependencyFinder.App.Entities;
using DependencyFinder.App.Utils;
using System.Globalization;
using System.Text.Json;
using static DependencyFinder.App.Utils.EnumUtils;


namespace DependencyFinder.App;

public class ProcessAnalyze
{
    public static async Task<Task> ProcessAnalyzeAsync(Options options)
    {
        string inputPath = options.InputPath;
        string outputPath = options.OutputPath;
        bool gptReport = options.GPTReport;

        // setup verbose
        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
        Environment.SetEnvironmentVariable("Verbose", textInfo.ToTitleCase(options.Verbose.ToString()));

        // setup Json Serializer
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