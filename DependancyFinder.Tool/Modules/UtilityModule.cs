using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using DependencyFinder.Tool.Modules.Validation;
using static DependencyFinder.Tool.Modules.EnumModule;

namespace DependencyFinder.Tool.Modules;

public class UtilityModule
{
    /// <summary>
    /// Custom WriteLine method to write messages in different colors on a single line of code
    /// </summary>
    /// <param name="color"></param>
    /// <param name="message"></param>
    public static void CustomWriteLine(UsageEnum usage, string message)
    {
        static ConsoleColor GetConsoleColor(UsageEnum usage)
        {
            switch (usage)
            {
                case UsageEnum.Processing:
                    return ConsoleColor.Yellow;
                case UsageEnum.Success:
                    return ConsoleColor.Green;
                case UsageEnum.Error:
                    return ConsoleColor.Red;
                case UsageEnum.Complete:
                    return ConsoleColor.Blue;
                case UsageEnum.Log:
                    return ConsoleColor.Magenta;
                default:
                case UsageEnum.Info:
                    return ConsoleColor.White;
            }
        }
        Console.ForegroundColor = GetConsoleColor(usage);
        Console.WriteLine(message);
        Console.ForegroundColor = ConsoleColor.White;
    }

    /// <summary>
    /// HashSet to store the valid file extensions
    /// </summary>
    private static readonly HashSet<string> validExtensions = new HashSet<string> { ".sql" };

    /// <summary>
    /// Method to verify if the file exists and if the extension is supported
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static bool IsValidFile(string filePath)
    {
        try
        {
            string extension = Path.GetExtension(filePath);
            bool isValidExtension = validExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
            bool FileExist = File.Exists(filePath);
            var NotEmptyFile = File.ReadAllText(filePath).Length > 0;
            bool isValideContent = FileExist && NotEmptyFile;
            bool isValidFile = isValidExtension && isValideContent;
            if (!isValidExtension)
            {
                CustomWriteLine(UsageEnum.Error, $"File extension {extension} is not supported, please use a valid json or yaml file.");
                return false;
            }
            if (!FileExist)
            {
                CustomWriteLine(UsageEnum.Error, $"File {filePath} does not exist.");
                return false;
            }
            if (!NotEmptyFile)
            {
                CustomWriteLine(UsageEnum.Error, $"File {filePath} is empty.");
                return false;
            }
            return isValidFile;
        }
        catch (Exception e)
        {
            CustomWriteLine(UsageEnum.Error, $"IsValidFile Error: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// Method to verify if the directory exists, if not, it creates the directory
    /// </summary>
    /// <param name="directoryPath"></param>
    /// <returns></returns>
    public static bool IsValidDirectory(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            return true;
        }
        else
        {
            CustomWriteLine(UsageEnum.Processing, "Creating directory...");
            Directory.CreateDirectory(directoryPath);
            IsValidDirectory(directoryPath);
            return false;
        }
    }

    /// <summary>
    /// Method to convert a string to PascalCase
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ToPascalCase(string text)
    {
        var match = Regex.Match(text, "^(?<word>^[a-z_.-/\\\\ -]+|[A-Z_.-/\\\\ -]+|[A-Z][a-z_.-/\\\\ -]+)+$");
        var groups = match.Groups["word"];
        var sb = new StringBuilder();
        foreach (var capture in groups.Captures.Cast<Capture>())
        {
            sb.Append(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(capture.Value.ToLower()));
        }
        var result = sb.ToString().Replace(" ", "").Replace("-", "").Replace("_", "").Replace("/", "").Replace("\\", "");
        return result;
    }

    /// <summary>
    /// Method to format the namespace inside the generated code
    /// </summary>
    /// <param name="path"></param>
    /// <param name="namespaceName"></param>
    /// <returns></returns>
    public static string FormatPath(string path, string namespaceName)
    {
        try
        {
            string pathNamespace;
            if (!path.EndsWith('/') && !path.EndsWith('\\'))
            {
                path += ".";
            }
            if (path == null)
            {
                pathNamespace = namespaceName;
            }
            else
            {
                pathNamespace = path.Replace('/', '.').Replace('\\', '.') + namespaceName;
            }
            pathNamespace = ToPascalCase(pathNamespace);
            pathNamespace = pathNamespace.TrimStart('.');
            return pathNamespace;
        }
        catch (Exception e)
        {
            CustomWriteLine(UsageEnum.Error, $"FormatPath Error: {e.Message}");
            return "";
        }
    }

    /// <summary>
    /// Method to validate the entries. Take the options object and apply pre defined rules to validate the entries
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    public List<string> EntryValidation(Options o)
    {
        var OptionValidation = new Options
        {
            InputPath = o.InputPath,
            // OutputPath = o.OutputPath,
        };
        var validator = new OptionsValidator();
        var validationResult = validator.Validate(OptionValidation);
        var errorMessages = new List<string>();
        if (!validationResult.IsValid)
        {
            foreach (var failure in validationResult.Errors)
            {
                errorMessages.Add($"Property {failure.PropertyName} failed validation. Error was: {failure.ErrorMessage}");
            }
        }
        return errorMessages;
    }

    /// <summary>
    /// Method to split the file path into directory and file name
    /// <param name="filePath"></param>
    /// <returns>A tuple where Item1 is the directory and Item2 the fileName wihtout extension</returns>
    public static (string, string) SplitFilePath(string filePath)
    {

        string directory = Path.GetDirectoryName(filePath)!;
        string fileName = Path.GetFileNameWithoutExtension(filePath);

        return (directory, fileName);
    }

    /// <summary>
    /// Method to format the file name
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string FormatFileName(string fileName)
    {
        // CustomWriteLine(UsageEnum.Log, $"Formatting file name: {fileName}");
        // start
        while (fileName.Length > 0 && !char.IsLetterOrDigit(fileName[0]))
        {
            fileName = fileName.Substring(1);
        }
        // end
        while (fileName.Length > 0 && !char.IsLetterOrDigit(fileName[^1]))
        {
            fileName = fileName.Substring(0, fileName.Length - 1);
        }

        // If the string is still empty after removing non-letter/digit characters, throw an exception
        if (fileName.Length == 0)
        {
            throw new ArgumentException($"File name does not contain any valid characters.");
        }
        return fileName;
    }

    /// <summary>
    /// Method to find a file from a partial file name
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="name"></param>
    public static string FindFileInFilePath(string filePath, string name)
    {
        if (!Directory.Exists(filePath))
        {
            CustomWriteLine(UsageEnum.Error, $"Directory {filePath} does not exist.");
            return "Not Found";
        }

        Regex regex = new Regex(@$"\b{name}\b", RegexOptions.IgnoreCase);

        var files = Directory.GetFiles(filePath);
        foreach (var file in files)
        {
            string fileName = Path.GetFileName(file);
            Match match = regex.Match(fileName);
            if (match.Success)
            {
                return file;
            }
        }

        CustomWriteLine(UsageEnum.Error, $"{name} in {filePath} not found.");
        return "Not Found";
    }
}