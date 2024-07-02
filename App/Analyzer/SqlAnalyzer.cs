using DependencyFinder.App.Controller;
using DependencyFinder.App.Entities;
using System.Text;
using System.Text.RegularExpressions;
using TSQL;
using TSQL.Tokens;
using static DependencyFinder.App.Utils.EnumUtils;

namespace DependencyFinder.App.Analyzer;

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
            Type = SPType.StoreProcedure,
        };
        CustomWriteLine(UsageEnum.Processing, $"Analyzing {fileName} at {inputPath}");
        try
        {
            if (gptReport)
            {
                SetEnv();
                var response = await ChatService.SendMessageAsync(File.ReadAllText(inputPath));
                response = UniDecode(response);
                root.GPTReport = response;
            }

            string sql = await File.ReadAllTextAsync(inputPath);
            TSQLTokenizer tokenizer = new(sql);

            HashSet<string> dependencies = [];
            bool isExecCommand = false;

            while (tokenizer.MoveNext())
            {
                TSQLToken token = tokenizer.Current;

                // Analyze basic commands
                if (token.Type == TSQLTokenType.Keyword)
                {
                    // Finding basic commands
                    string tokenTextUpper = token.Text.ToUpperInvariant();
                    if (root.HeavyQueries.TryGetValue(tokenTextUpper, out int value))
                    {
                        root.HeavyQueries[tokenTextUpper] = ++value;
                    }
                    // Finding dependencies
                    else if (tokenTextUpper == "EXEC")
                    {
                        isExecCommand = true;
                    }
                    else if (token.Text.ToUpperInvariant() == "DROP" && tokenizer.MoveNext())
                    {
                        TSQLToken nextToken = tokenizer.Current;
                        if (nextToken.Type == TSQLTokenType.Keyword && nextToken.Text.ToUpperInvariant() == "TABLE")
                        {
                            root.HeavyQueries["DROP TABLE"]++;
                        }
                        else if (nextToken.Text.ToUpperInvariant() == "DATABASE")
                        {
                            root.HeavyQueries["DROP DATABASE"]++;
                        }
                    }
                    else if (token.Text.ToUpperInvariant() == "ALTER" && tokenizer.MoveNext())
                    {
                        TSQLToken nextToken = tokenizer.Current;
                        if (nextToken.Type == TSQLTokenType.Keyword && nextToken.Text.ToUpperInvariant() == "TABLE")
                        {
                            root.HeavyQueries["ALTER TABLE"]++;
                        }
                        else if (nextToken.Text.ToUpperInvariant() == "DATABASE")
                        {
                            root.HeavyQueries["ALTER DATABASE"]++;
                        }
                    }
                }
                // Adding dependencies
                else if (isExecCommand && token.Type == TSQLTokenType.Identifier)
                {
                    dependencies.Add(token.Text);
                    isExecCommand = false;
                }
            }

            // Logging heavy query
            foreach (var query in root.HeavyQueries)
            {
                CustomWriteLine(UsageEnum.Log, $"{query.Key}: {query.Value}");
            }

            // Recursive for dependencies
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