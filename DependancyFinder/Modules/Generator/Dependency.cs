using TSQL;
using TSQL.Statements;
using TSQL.Tokens;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using static DependancyFinder.Modules.EnumModule;


namespace DependancyFinder.Modules.Generator
{

    public class DependencyRecursive
    {
        public static async Task<Dependency> GenerateDependencyAsync(string name)
        {
            try
            {
                CustomWriteLine(UsageEnum.Processing, $"Generating Stored Procedures for {name}");
                string sql = await File.ReadAllTextAsync(FormatFileName(name) + ".sql");

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

                Dependency dependency = new Dependency { Name = name };

                foreach (string dep in dependencies)
                {
                    dependency.Dependencies[dep] = await GenerateDependencyAsync(dep);
                }

                return dependency;
            }
            catch (Exception e)
            {
                CustomWriteLine(UsageEnum.Error, $"Error GenerateDependencyAsync: {e.Message}");
                throw;
            }
        }
    }
}