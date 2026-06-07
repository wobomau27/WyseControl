using System.Text.Json.Serialization;

namespace WyseControl.Models.VnStat;

public class VnStatTraffic
{
    [JsonPropertyName("total")]
    public VnStatValue Total { get; set; } = new();

    [JsonPropertyName("day")]
    public List<VnStatValue> Day { get; set; } = [];

    [JsonPropertyName("month")]
    public List<VnStatValue> Month { get; set; } = [];
}