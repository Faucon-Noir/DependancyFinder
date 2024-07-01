namespace DependencyFinder.App.Entities;

public class PromptOption
{
    public int Temperature { get; set; } = 0;
    public string? Model { get; set; } = string.Empty;
    public List<string>? Capacities { get; set; } = [];
}