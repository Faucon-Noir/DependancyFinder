using System.Text.Json.Serialization;
using static DependencyFinder.Tool.Modules.EnumModule;

namespace DependencyFinder.Tool.Modules.Entities;

/// <summary>
/// Entity class for stored procedures
/// </summary>
public class SPEntity
{
    public string Name { get; set; } = string.Empty;
    public List<SPEntity> Dependencies { get; set; } = new List<SPEntity>();
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SPType Type { get; set; } = SPType.Unkwon;
    public string FilePath { get; set; } = string.Empty;
    // public List<string> HeavyQueries { get; set; } = new List<string>();
}
