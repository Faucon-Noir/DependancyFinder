using App.Controller;
using App.Entities;
using TSQL;
using TSQL.Tokens;
using static App.Utils.EnumUtils;

namespace App.Analyzer;

public class SqlAnalyzer
{
    private readonly ChatService _chatService;
    public SqlAnalyzer()
    {
        _chatService = new();
    }

    /// <summary>
    /// Method to recursively analyze SQL file.
    /// </summary>
    /// <param name="gptReport"></param> 
    /// <param name="filePath"></param>
    /// <param name="root"></param>
    /// <param name="dependencies"></param>
    /// <returns></returns>
    public async Task SqlRecursive(bool gptReport, string filePath, SPEntity root, HashSet<string> dependencies)
    {
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

    /// <summary>
    /// Method to handle patterns that acts on Table or Database
    /// </summary>
    /// <param name="root"></param>
    /// <param name="tokenizer"></param>
    /// <param name="pattern"></param>
    public static void HandlePattern(SPEntity root, TSQLTokenizer tokenizer, string pattern)
    {
        TSQLToken nextToken = tokenizer.Current;
        if (nextToken.Type != TSQLTokenType.Keyword || !nextToken.Text.Equals(pattern, StringComparison.InvariantCultureIgnoreCase))
        {
            if (nextToken.Text.Equals("DATABASE", StringComparison.InvariantCultureIgnoreCase))
            {
                root.HeavyQueries[$"{pattern} DATABASE"]++;
            }
        }
        else
        {
            root.HeavyQueries[$"{pattern} TABLE"]++;
        }
    }

    /// <summary>
    /// Method to analyze SQL file.
    /// Also support folder that contains SQL files.
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="gptReport"></param>
    /// <returns></returns>
    public async Task<SPEntity> AnalyzeSqlAsync(string inputPath, bool gptReport)
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

        try
        {
            CustomWriteLine(UsageEnum.Processing, $"Analyzing {fileName} at {inputPath}");

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

                    switch (tokenTextUpper)
                    {
                        case "EXEC":
                            isExecCommand = true;
                            break;
                        case "DROP":
                            HandlePattern(root, tokenizer, "DROP");
                            break;
                        case "ALTER":
                            HandlePattern(root, tokenizer, "ALTER");
                            break;
                        case "SELECT":
                        case "UPDATE":
                        case "INSERT":
                        case "MERGE":
                            ++root.HeavyQueries[tokenTextUpper];
                            break;
                    }
                }
                // Adding dependencies
                else if (isExecCommand && token.Type == TSQLTokenType.Identifier)
                {
                    dependencies.Add(token.Text);
                    isExecCommand = false;
                }
            }

            // Recursive for dependencies
            await SqlRecursive(gptReport, filePath, root, dependencies);

            // Logging heavy query
            foreach (var query in root.HeavyQueries)
            {
                CustomWriteLine(UsageEnum.Log, $"{query.Key}: {query.Value} in file {fileName}");
            }

            // GPT Report
            if (gptReport)
            {
                var response = await _chatService.SendMessageAsync(File.ReadAllText(inputPath));
                root.GPTReport = response;
            }
        }
        catch (FileNotFoundException)
        {
            CustomWriteLine(UsageEnum.Log, $"Skipping {fileName} - file not found");
        }

        return root;
    }


}