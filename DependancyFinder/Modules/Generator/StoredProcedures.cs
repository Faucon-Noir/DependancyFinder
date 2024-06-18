using TSQL;
using TSQL.Statements;
using TSQL.Tokens;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
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
        public static async Task<(string, string)> GenerateStoredProceduresAsync(string inputPath)
        {
            CustomWriteLine(UsageEnum.Processing, "Generating Stored Procedures");
            CustomWriteLine(UsageEnum.Log, $"{SplitFilePath(inputPath)}");
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

            Dependency root = new Dependency { Name = fileName };

            foreach (string dep in dependencies)
            {
                root.Dependencies[dep] = new Dependency { Name = dep };
            }

            Dictionary<string, Dependency> output = new Dictionary<string, Dependency> { { fileName, root } };

            string json = JsonConvert.SerializeObject(output, Formatting.Indented);

            string outputPath = "./" + fileName + ".json";
            CustomWriteLine(UsageEnum.Log, $"Output Path: {outputPath}");

            await File.WriteAllTextAsync(outputPath, json);
            CustomWriteLine(UsageEnum.Processing, "Stored Procedures Generated");

            return (json, outputPath);
        }
    }
}