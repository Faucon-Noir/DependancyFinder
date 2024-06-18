using CommandLine;
public class Options
{
    /// <summary>
    /// Input file path
    /// </summary>
    [Option('i', "inputPath", Required = false, HelpText = "Input file path.")]
    public required string InputPath { get; set; } = string.Empty;

    /// <summary>
    /// Output directory path
    /// </summary>
    [Option('o', "outputPath", Required = false, HelpText = "Output directory path.")]
    public string? OutputPath { get; set; } = string.Empty;
}