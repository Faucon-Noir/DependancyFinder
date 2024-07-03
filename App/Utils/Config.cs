using CommandLine;
using static App.Utils.EnumUtils;

namespace App.Utils;

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

    /// <summary>
    /// Define if you want a written report by a GPT model (set your .ENV before, better works)
    /// </summary>
    [Option("gpt", Required = false, HelpText = "Define if you want a written report by a GPT model (set your .ENV before, better works)")]
    public bool GPTReport { get; set; } = false;

    /// <summary>
    /// Set output verbosity
    /// </summary>
    [Option("verbose", Required = false, HelpText = "Set output verbosity")]
    public UsageEnum Verbose { get; set; } = UsageEnum.Processing;
}