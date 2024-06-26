using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using static DependencyFinder.Tool.Modules.EnumModule;

namespace DependencyFinder.Tool.Modules.Entities;

/// <summary>
/// Entity class for stored procedures
/// </summary>
public class ChatDTO
{
    public string? chatId { get; set; } = string.Empty;
    public string? chat { get; set; } = string.Empty;
    public string? culture { get; set; } = string.Empty;
    public string token { get; set; } = string.Empty;
    public IEnumerable<object>? dataSources { get; set; } = new List<object>();
    public string? signalRConnectionId { get; set; } = string.Empty;
    public object? promptOptions { get; set; } = new object();
}
