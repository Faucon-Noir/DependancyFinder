using App.Entities;
using TSQL.Tokens;
using TSQL;
using static App.Utils.EnumUtils;

namespace App.Analyzer
{
    public class SqlAnalyzerHelper
    {
        public readonly SqlAnalyzer _sqlAnalyzer;

        public SqlAnalyzerHelper()
        {
            _sqlAnalyzer = new SqlAnalyzer();
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
                    var spEntity = await _sqlAnalyzer.AnalyzeSqlAsync(file, gptReport);
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
        public void HandlePattern(SPEntity root, TSQLTokenizer tokenizer, string pattern)
        {
            TSQLToken nextToken = tokenizer.Current;
            if (nextToken.Type == TSQLTokenType.Keyword && nextToken.Text.ToUpperInvariant() == pattern)
            {
                root.HeavyQueries[$"{pattern} TABLE"]++;
            }
            else if (nextToken.Text.ToUpperInvariant() == "DATABASE")
            {
                root.HeavyQueries[$"{pattern} DATABASE"]++;
            }
        }
    }
}
