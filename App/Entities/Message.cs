namespace DependencyFinder.App.Entities;

public class Message
{
    public int MessageId { get; set; }
    public string Message1 { get; set; } = string.Empty;
    public int ChatId { get; set; }
    public int ChatRole { get; set; }
    public string Options { get; set; } = string.Empty;
    public object DataSource { get; set; } = string.Empty;
    public string IsLiked { get; set; } = string.Empty;
    public string IsDisliked { get; set; } = string.Empty;
}