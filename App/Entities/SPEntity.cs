using System.Text.Json.Serialization;
using static App.Utils.EnumUtils;

namespace App.Entities;

/// <summary>
/// Entity class for stored procedures
/// </summary>
public class SPEntity
{
    /// <summary>
    /// Procedure name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Procedure dependencies
    /// </summary>
    public List<SPEntity> Dependencies { get; set; } = [];

    /// <summary>
    /// Count of dependencies
    /// </summary>
    public int Size => Dependencies.Count;

    /// <summary>
    /// Procedure type
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SPType Type { get; set; } = SPType.Unkwon;

    /// <summary>
    /// Absolute path to the file
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Heavy queries found in the procedure
    /// </summary>
    public Dictionary<string, int> HeavyQueries { get; set; } = new Dictionary<string, int>
    {
        ["SELECT"] = 0,
        ["UPDATE"] = 0,
        ["DELETE"] = 0,
        ["INSERT"] = 0,
        ["MERGE"] = 0,
        ["DROP TABLE"] = 0,
        ["DROP DATBASE"] = 0,
        ["ALTER TABLE"] = 0,
        ["ALTER DATABASE"] = 0
    };

    /// <summary>
    /// Quality report from C2S GPT model
    /// </summary>
    public string GPTReport { get; set; } = "No report requested";

    /// <summary>
    /// Last time the entity was updated
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.Now;
}