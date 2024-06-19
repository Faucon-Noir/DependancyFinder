using DependencyFinder.Tool.Modules.Entities;
using Newtonsoft.Json;
using TSQL;
using TSQL.Tokens;
using static DependencyFinder.Tool.Modules.EnumModule;
using static DependencyFinder.Tool.Modules.Generator.StoredProcedures;


namespace DependencyFinder.Tool.Modules.Generator
{

    public class DependencyRecursive
    {
        public static async Task<SPEntity> GenerateDependencyAsync(string name, string filePath)
        {
            name = FormatFileName(name);
            SPEntity spDependency = new SPEntity { Name = name };
            try
            {
                CustomWriteLine(UsageEnum.Processing, $"Processing Dependencies {name}");
                name = FormatFileName(name);
                string sql = await File.ReadAllTextAsync(FindFileInFilePath(filePath, name));

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
                CustomWriteLine(UsageEnum.Log, $"Adding {dependencies.Any()} to {name}");
                foreach (string dep in dependencies)
                {
                    SPEntity spEntity = await GenerateDependencyAsync(dep, filePath);
                    spDependency.Dependencies?.Add(spEntity);
                    // spDependency.Dependencies[dep] = await GenerateDependencyAsync(dep, filePath);
                }

            }
            catch (FileNotFoundException)
            {
                CustomWriteLine(UsageEnum.Error, $"Skipping {name} - file not found");
            }
            CustomWriteLine(UsageEnum.Complete, $"Completed {spDependency.Dependencies?.Count}");
            // string fileName = SplitFilePath(inputPath).Item2;
            // Dictionary<string, SPEntity> output = new Dictionary<string, SPEntity> { { FormatFileName(fileName), root } };
            // string json = JsonConvert.SerializeObject(output, Formatting.Indented);
            return spDependency;
        }
    }
}