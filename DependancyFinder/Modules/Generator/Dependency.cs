using TSQL;
using TSQL.Statements;
using TSQL.Tokens;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using static DependancyFinder.Modules.EnumModule;
using System.Text.RegularExpressions;


namespace DependancyFinder.Modules.Generator
{

    public class DependencyRecursive
    {
        public static async Task<Dependency> GenerateDependencyAsync(string name, string filePath)
        {
            name = FormatFileName(name);
            Dependency dependency = new Dependency { Name = name };
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

                foreach (string dep in dependencies)
                {
                    dependency.Dependencies[dep] = await GenerateDependencyAsync(dep, filePath);
                }

            }
            catch (FileNotFoundException)
            {
                CustomWriteLine(UsageEnum.Error, $"Skipping {name} - file not found");
            }

            return dependency;
        }
    }
}