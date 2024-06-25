using CommandLine;

namespace DependencyFinder.Tool.Modules;

public class Options
{
    /// <summary>
    /// Input file path
    /// </summary>
    [Option('i', "input", Required = false, HelpText = "Input file path.")]
    public required string InputPath { get; set; } = string.Empty;

    /// <summary>
    /// Output directory path
    /// </summary>
    [Option('o', "output", Required = false, HelpText = "Output directory path.")]
    public string OutputPath { get; set; } = "./Output/";
}