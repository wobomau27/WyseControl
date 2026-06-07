using System.Text.Json.Serialization;

namespace WyseControl.Models.VnStat;

public class VnStatRoot
{
    [JsonPropertyName("interfaces")]
    public List<VnStatInterface> Interfaces { get; set; } = [];
}