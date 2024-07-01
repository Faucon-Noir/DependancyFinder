namespace DependencyFinder.App.Entities;

public class PromptOption
{
    /// <summary>
    /// Temperature for GPT Model
    /// </summary>
    public int Temperature { get; set; } = 0;

    /// <summary>
    /// Model type to use for GPT
    /// </summary>
    public string? Model { get; set; } = string.Empty;

    /// <summary>
    /// Capacities for GPT model
    /// </summary>
    public List<string>? Capacities { get; set; } = [];
}