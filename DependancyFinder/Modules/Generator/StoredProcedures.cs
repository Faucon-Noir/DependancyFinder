using TSQL;
using TSQL.Tokens;
using Newtonsoft.Json;
using static DependancyFinder.Modules.EnumModule;


namespace DependancyFinder.Modules.Generator
{
    public class Dependency
    {
        public string Name { get; set; } = string.Empty;
        public Dictionary<string, Dependency> Dependencies { get; set; } = new Dictionary<string, Dependency>();
    }
    public class StoredProcedures
    {
        public static async Task<(string, string)> GenerateStoredProceduresAsync(string inputPath, string outputPath)
        {
            CustomWriteLine(UsageEnum.Processing, "Generating Stored Procedures");
            string sql = await File.ReadAllTextAsync(inputPath);

            TSQLTokenizer tokenizer = new TSQLTokenizer(sql);

            List<string> dependencies = new List<string>();
            bool isExecCommand = false;

            while (tokenizer.MoveNext())
            {
                TSQLToken token = tokenizer.Current;

                if (token.Type == TSQLTokenType.Keyword && token.Text.ToUpper() == "EXEC")
                {
                    isExecCommand = true;
                }
                else if (isExecCommand && token.Type == TSQLTokenType.Identifier)
                {
                    dependencies.Add(token.Text);
                    isExecCommand = false;
                }
            }
            string fileName = SplitFilePath(inputPath).Item2;
            string filePath = SplitFilePath(inputPath).Item1;

            Dependency root = new Dependency { Name = fileName };

            foreach (string dep in dependencies)
            {
                root.Dependencies[dep] = await DependencyRecursive.GenerateDependencyAsync(dep, filePath);
            }

            Dictionary<string, Dependency> output = new Dictionary<string, Dependency> { { FormatFileName(fileName), root } };

            string json = JsonConvert.SerializeObject(output, Formatting.Indented);

            IsValidDirectory(outputPath);
            string outputFile = Path.Combine(outputPath, fileName + ".json");
            await File.WriteAllTextAsync(outputFile, json);
            CustomWriteLine(UsageEnum.Complete, $"Process complete. See {outputFile}");

            return (json, outputFile);
        }
    }
}