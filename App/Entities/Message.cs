namespace DependencyFinder.App.Entities;

public class Message
{
    /// <summary>
    /// Message Id
    /// </summary>
    public int MessageId { get; set; }

    /// <summary>
    /// Message we want in GPTReport
    /// </summary>
    public string Message1 { get; set; } = string.Empty;

    /// <summary>
    /// Chat Id
    /// </summary>
    public int ChatId { get; set; }
    /// <summary>
    /// Chat Role
    /// </summary>
    public int ChatRole { get; set; }
    /// <summary>
    /// Chat Options
    /// </summary>
    public string Options { get; set; } = string.Empty;
    /// <summary>
    /// Data Source
    /// </summary>
    public object DataSource { get; set; } = string.Empty;
    /// <summary>
    /// 1 if chat is Liked, default 0
    /// </summary>
    public string IsLiked { get; set; } = string.Empty;
    /// <summary>
    /// 1 if chat is Disliked, default 0
    /// </summary>
    public string IsDisliked { get; set; } = string.Empty;
}