using System.Text.Json.Serialization;
using static DependencyFinder.App.Utils.EnumUtils;

namespace DependencyFinder.App.Entities;

/// <summary>
/// Entity class for stored procedures
/// </summary>
public class SPEntity
{
    public string Name { get; set; } = string.Empty;

    public List<SPEntity> Dependencies { get; set; } = [];

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SPType Type { get; set; } = SPType.Unkwon;

    public string FilePath { get; set; } = string.Empty;

    // public List<string> HeavyQueries { get; set; } = new List<string>();

    public string GPTReport { get; set; } = "No report requested";
    public DateTime LastUpdated { get; set; } = DateTime.Now;
}
