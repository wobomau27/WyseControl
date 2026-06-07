using System.Text.Json.Serialization;

namespace WyseControl.Models.VnStat;

public class VnStatValue
{
    [JsonPropertyName("rx")]
    public long Rx { get; set; }

    [JsonPropertyName("tx")]
    public long Tx { get; set; }
}