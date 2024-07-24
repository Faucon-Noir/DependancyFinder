using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using static App.Utils.EnumUtils;

namespace App.Utils;

public class FileUtils
{
    /// <summary>
    /// HashSet to store the valid file extensions
    /// </summary>
    private static readonly HashSet<string> validExtensions = [".sql"];

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
    /// Method to verify if the file exists and if the extension is supported. Support directory as well
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static bool IsValidPath(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                // Check file
                string extension = Path.GetExtension(filePath);
                bool isValidExtension = validExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
                bool FileExist = File.Exists(filePath);
                bool NotEmptyFile = File.ReadAllText(filePath).Length > 0;
                bool isValideContent = FileExist && NotEmptyFile;

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
                return isValideContent;
            }
            else if (Directory.Exists(filePath))
            {
                // Check folder
                string directory = Path.GetDirectoryName(filePath)!;
                bool isValidDirectory = Directory.Exists(directory);
                var hasSqlFiles = Directory.GetFiles(directory, $"*.sql").Length > 0;
                if (!isValidDirectory)
                {
                    CustomWriteLine(UsageEnum.Error, $"Directory {filePath} does not exist.");
                    return false;
                }
                if (!hasSqlFiles)
                {
                    CustomWriteLine(UsageEnum.Error, $"Directory {filePath} does not contain any valid SQL files.");
                    return false;
                }
                CustomWriteLine(UsageEnum.Success, $"Directory path {filePath} is valid.");
                return hasSqlFiles;
            }
            else
            {
                CustomWriteLine(UsageEnum.Error, $"Path {filePath} does not exist.");
                return false;
            }
        }
        catch (Exception e)
        {
            CustomWriteLine(UsageEnum.Error, $"IsValidPath Error: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// Method to format a json file to be visualized in the web visualizer
    /// </summary>
    /// <param name="filepath"></param>
    public static void JsonToVisualizer(string filePath)
    {
        try
        {
            File.Copy(filePath, @".\App.Server\output.json", true);
            Process.Start(new ProcessStartInfo
            {
                FileName = @".\App\index.html",
                UseShellExecute = true
            });
        }
        catch (Exception e)
        {
            CustomWriteLine(UsageEnum.Error, $"Error Opening File: {e.Message}");
        }
    }
}
