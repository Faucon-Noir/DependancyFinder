using App.Analyzer;
using App.Entities;
using App.Utils;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using static App.Utils.EnumUtils;

namespace App;

public class ProcessAnalyze
{

    private readonly SqlAnalyzer _sqlAnalyzer;

    public ProcessAnalyze()
    {
        _sqlAnalyzer = new();
    }
    public async Task<Task> ProcessAnalyzeAsync(Options options)
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
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
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
                    CustomWriteLine(UsageEnum.Info, $"Processing: {file}");
                    var output = await _sqlAnalyzer.AnalyzeSqlAsync(file, gptReport);
                    string outputString = JsonSerializer.Serialize(output);
                    var outputJson = JsonSerializer.Deserialize<Dictionary<string, SPEntity>>(outputString) ?? [];
                    allOutputs.Add(SplitFilePath(file).Item2, outputJson);
                }

                string json = JsonSerializer.Serialize(allOutputs, jsonSerializerOptions);
                IsValidDirectory(outputPath);
                File.WriteAllText(Path.Combine(outputPath, "all.json"), json);
            }
            // Single file processing
            else
            {
                string fileName = SplitFilePath(inputPath).Item2;
                SPEntity sqlAnalysisData = await _sqlAnalyzer.AnalyzeSqlAsync(inputPath, gptReport);

                string json = JsonSerializer.Serialize(sqlAnalysisData, jsonSerializerOptions);
                IsValidDirectory(outputPath);
                string filePath = Path.Combine(outputPath, $"{fileName}.json");
                File.WriteAllText(filePath, json, encoding: System.Text.Encoding.UTF8);
                JsonToVisualizer(filePath);
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