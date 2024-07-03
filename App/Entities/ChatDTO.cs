namespace App.Entities;

/// <summary>
/// Entity class for stored procedures
/// </summary>
public class ChatDTO
{
    /// <summary>
    /// Target chat id [required]
    /// </summary>
    public string? ChatId { get; set; } = string.Empty;
    /// <summary>
    /// Message to send to chat [required]
    /// </summary>
    public string? Chat { get; set; } = string.Empty;
    /// <summary>
    /// Language culture [required]
    /// </summary>
    public string? Culture { get; set; } = string.Empty;
    /// <summary>
    /// Data sources [optional]
    /// </summary>
    public IEnumerable<string>? DataSources { get; set; } = [];
    /// <summary>
    /// SignalR connection id [required]
    /// </summary>
    public string? SignalRConnectionId { get; set; } = string.Empty;
    /// <summary>
    /// PromptOptions for GPT model [required]
    /// </summary>
    public PromptOption? PromptOptions { get; set; } = new PromptOption();
}
