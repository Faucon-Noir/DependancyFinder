using App.Validation;
using static App.Utils.EnumUtils;

namespace App.Utils;

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
    /// Method to load the environment variables from the .env file
    /// </summary>
    public static void SetEnv()
    {
        try
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
        catch (Exception e)
        {
            CustomWriteLine(UsageEnum.Error, $"SetEnv Error: {e.Message}");
        }
    }

}