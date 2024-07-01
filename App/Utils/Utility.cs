using System.Text.RegularExpressions;
using DependencyFinder.Tool.Validation;
using static DependencyFinder.Tool.Utils.Enum;

namespace DependencyFinder.Tool.Utils;

public partial class Utility
{
    /// <summary>
    /// Custom WriteLine method to write messages in different colors on a single line of code
    /// </summary>
    /// <param name="color"></param>
    /// <param name="message"></param>
    public static void CustomWriteLine(UsageEnum usage, string message)
    {
        // Get the verbosity level from the environment variable
        Enum.TryParse(Environment.GetEnvironmentVariable("Verbose"), out UsageEnum usageEnum);

        // Get the console color based on the verbosity level
        static ConsoleColor GetConsoleColor(UsageEnum usage)
        {
            return usage switch
            {
                UsageEnum.Processing => ConsoleColor.Yellow,
                UsageEnum.Success => ConsoleColor.Green,
                UsageEnum.Error => ConsoleColor.Red,
                UsageEnum.Complete => ConsoleColor.Blue,
                UsageEnum.Log => ConsoleColor.Magenta,
                _ => ConsoleColor.White,
            };
        }
        if (usage <= usageEnum)
        {
            Console.ForegroundColor = GetConsoleColor(usage);
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

    }

    /// <summary>
    /// HashSet to store the valid file extensions
    /// </summary>
    private static readonly HashSet<string> validExtensions = [".sql"];

    /// <summary>
    /// Method to verify if the file exists and if the extension is supported. Support directory as well
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static bool IsValidPath(string filePath)
    {
        try
        {
            // Check file
            string extension = Path.GetExtension(filePath);
            bool isValidExtension = validExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
            bool FileExist = File.Exists(filePath);
            var NotEmptyFile = File.ReadAllText(filePath).Length > 0;
            bool isValideContent = FileExist && NotEmptyFile;

            // Check folder
            string directory = Path.GetDirectoryName(filePath)!;
            bool isValidDirectory = Directory.Exists(directory);
            bool hasSqlFiles = Directory.GetFiles(directory, "*.sql").Length > 0;

            bool isValidPath = isValidExtension && isValideContent || isValidDirectory && hasSqlFiles;
            if (!isValidExtension)
            {
                CustomWriteLine(UsageEnum.Error, $"File extension {extension} is not supported, please use a valid sql file.");
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
            CustomWriteLine(UsageEnum.Success, $"Path {filePath} is valid.");
            return isValidPath;
        }
        catch (Exception e)
        {
            CustomWriteLine(UsageEnum.Error, $"IsValidPath Error: {e.Message}");
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
        try
        {
            if (Directory.Exists(directoryPath))
            {
                CustomWriteLine(UsageEnum.Success, $"Directory {directoryPath} exists.");
                return true;
            }
            else
            {
                CustomWriteLine(UsageEnum.Processing, "Creating directory...");
                Directory.CreateDirectory(directoryPath);
                CustomWriteLine(UsageEnum.Complete, $"Directory {directoryPath} created.");
                IsValidDirectory(directoryPath);
                return false;
            }
        }
        catch (Exception e)
        {
            CustomWriteLine(UsageEnum.Error, $"IsValidDirectory Error: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// Method to validate the entries. Take the options object and apply pre defined rules to validate the entries
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    public static List<string> EntryValidation(Options o)
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
        else
        {
            CustomWriteLine(UsageEnum.Success, "Options are valid.");
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
        // start
        while (fileName.Length > 0 && !char.IsLetterOrDigit(fileName[0]))
        {
            fileName = fileName[1..];
        }
        // end
        while (fileName.Length > 0 && !char.IsLetterOrDigit(fileName[^1]))
        {
            fileName = fileName[..^1];
        }

        // If the string is still empty after removing non-letter/digit characters, throw an exception
        if (fileName.Length == 0)
        {
            throw new ArgumentException($"File name does not contain any valid characters.");
        }

        CustomWriteLine(UsageEnum.Complete, $"File name formatted to {fileName}.");
        return fileName;
    }

    /// <summary>
    /// Method to find a file from a partial file name
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="name"></param>
    public static string FindFileInFilePath(string filePath, string name)
    {
        try
        {
            if (!Directory.Exists(filePath))
            {
                CustomWriteLine(UsageEnum.Error, $"Directory {filePath} does not exist.");
                return "Directory not found";
            }

            Regex regex = new(@$"\b{name}\b", RegexOptions.IgnoreCase);

            var files = Directory.GetFiles(filePath);
            foreach (var file in files)
            {
                string fileName = Path.GetFileName(file);
                Match match = regex.Match(fileName);
                if (match.Success)
                {
                    CustomWriteLine(UsageEnum.Success, $"File {file} found.");
                    return file;
                }
            }

            return "File not found";
        }
        catch (Exception e)
        {
            CustomWriteLine(UsageEnum.Error, $"FindFileInFilePath Error: {e.Message}");
            return e.Message;
        }
    }

    /// <summary>
    /// Method to load the environment variables from the .env file
    /// </summary>
    public static void SetEnv()
    {
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        string path = Path.Combine(basePath, ".env");

        if (!File.Exists(path))
        {
            CustomWriteLine(UsageEnum.Error, $"Env file not found at {path}");
            return;
        }

        var lines = File.ReadAllLines(path);
        foreach (var line in lines)
        {
            if (line.Contains('='))
            {
                var parts = line.Split('=', 2);
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();
                    Environment.SetEnvironmentVariable(key, value);
                    CustomWriteLine(UsageEnum.Log, $"Loaded {key}: {value}");
                }
            }
        }
    }

}