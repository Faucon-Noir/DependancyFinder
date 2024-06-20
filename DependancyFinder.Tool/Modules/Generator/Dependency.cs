using DependencyFinder.Tool.Modules.Entities;
using TSQL;
using TSQL.Tokens;
using static DependencyFinder.Tool.Modules.EnumModule;


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

                HashSet<string> dependencies = new HashSet<string>();
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
                    SPEntity spEntity = await GenerateDependencyAsync(dep, filePath);
                    spDependency.Dependencies?.Add(spEntity);
                }


            }
            catch (FileNotFoundException)
            {
                CustomWriteLine(UsageEnum.Log, $"Skipping {name} - file not found");
            }

            return spDependency;
        }
    }
}