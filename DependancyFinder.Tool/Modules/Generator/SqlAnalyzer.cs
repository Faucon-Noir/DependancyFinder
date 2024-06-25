using TSQL;
using TSQL.Tokens;
using DependencyFinder.Tool.Modules.Entities;
using static DependencyFinder.Tool.Modules.EnumModule;


namespace DependencyFinder.Tool.Modules.Generator
{
    public class SqlAnalyzer
    {
        public static async Task<SPEntity> GenerateStoredProceduresAsync(string inputPath)
        {
            string fileName = SplitFilePath(inputPath).Item2;
            fileName = FormatFileName(fileName);
            string filePath = SplitFilePath(inputPath).Item1;
            SPEntity root = new SPEntity
            {
                Name = fileName,
                FilePath = inputPath,
                Type = SPType.StoreProcedure
            };

            try
            {
                string sql = await File.ReadAllTextAsync(inputPath);

                TSQLTokenizer tokenizer = new TSQLTokenizer(sql);

                HashSet<string> dependencies = new();
                bool isExecCommand = false;

                while (tokenizer.MoveNext())
                {
                    TSQLToken token = tokenizer.Current;

                    if (token.Type == TSQLTokenType.Keyword && string.Equals(token.Text, "EXEC", StringComparison.OrdinalIgnoreCase))
                    {
                        isExecCommand = true;
                    }
                    else if (isExecCommand && token.Type == TSQLTokenType.Identifier)
                    {
                        dependencies.Add(token.Text);
                        isExecCommand = false;
                    }
                }

                foreach (string dep in dependencies)
                {
                    var depToFind = FormatFileName(dep);
                    var file = FindFileInFilePath(filePath, depToFind);
                    if (file == "File not found" || file == "Not Found")
                    {
                        var spEntity = new SPEntity
                        {
                            Name = depToFind,
                            FilePath = file,
                            Type = SPType.Unkwon
                        };
                        root.Dependencies.Add(spEntity);
                    }
                    else
                    {
                        var spEntity = await GenerateStoredProceduresAsync(file);
                        spEntity.Type = SPType.StoreProcedure;
                        root.Dependencies.Add(spEntity);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                CustomWriteLine(UsageEnum.Log, $"Skipping {fileName} - file not found");
            }

            return root;
        }
    }
}