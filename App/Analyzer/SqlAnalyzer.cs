using TSQL;
using TSQL.Tokens;
using static DependencyFinder.Tool.Utils.Enum;
using DependencyFinder.Tool.Entities;
using DependencyFinder.Tool.Controller;


namespace DependencyFinder.Tool.Generator;

public class SqlAnalyzer
{
    public static async Task<SPEntity> AnalyzeSqlAsync(string inputPath, bool gptReport)
    {
        string fileName = SplitFilePath(inputPath).Item2;
        fileName = FormatFileName(fileName);
        string filePath = SplitFilePath(inputPath).Item1;
        SPEntity root = new()
        {
            Name = fileName,
            FilePath = inputPath,
            Type = SPType.StoreProcedure
        };
        CustomWriteLine(UsageEnum.Log, $"Analyzing {fileName} at {inputPath}");
        try
        {
            if (gptReport)
            {
                SetEnv();
                var response = await ChatController.SendMessageAsync(File.ReadAllText(inputPath));
                root.GPTReport = response;
            }

            string sql = await File.ReadAllTextAsync(inputPath);
            TSQLTokenizer tokenizer = new(sql);

            HashSet<string> dependencies = [];
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
                if (file == "File not found" || file == "Directory not found")
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
                    var spEntity = await AnalyzeSqlAsync(file, gptReport);
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