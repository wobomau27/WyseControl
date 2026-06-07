using System.Text.Json.Serialization;

namespace WyseControl.Models.VnStat;

public class VnStatInterface
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("traffic")]
    public VnStatTraffic Traffic { get; set; } = new();
}