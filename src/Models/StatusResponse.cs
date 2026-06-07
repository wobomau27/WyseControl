namespace WyseControl.Models;

public class StatusResponse
{
    public bool VpnConnected { get; set; }

    public string Hostname { get; set; } = string.Empty;

    public string PublicIp { get; set; } = string.Empty;

    public double CpuUsage { get; set; }

    public double MemoryUsage { get; set; }

    public string Uptime { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public TrafficSummary Traffic { get; set; } = new();

    public string RoutingMode { get; set; } = "";
}