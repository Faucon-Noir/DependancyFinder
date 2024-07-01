namespace DependencyFinder.Tool.Entities;

/// <summary>
/// Entity class for stored procedures
/// </summary>
public class ChatDTO
{
    public string? ChatId { get; set; } = string.Empty;
    public string? Chat { get; set; } = string.Empty;
    public string? Culture { get; set; } = string.Empty;
    // public string token { get; set; } = string.Empty;
    public IEnumerable<string>? DataSources { get; set; } = [];
    public string? SignalRConnectionId { get; set; } = string.Empty;
    public PromptOption? PromptOptions { get; set; } = new PromptOption();
}
