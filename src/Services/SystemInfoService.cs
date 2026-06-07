using System.Globalization;
using WyseControl.Models;

namespace WyseControl.Services;

public class SystemInfoService
{
    private readonly IVnStatService _vnStatService;
    private readonly IVpnService _vpnService;
    private readonly IHttpClientFactory _httpClientFactory;

    public SystemInfoService(
        IHttpClientFactory httpClientFactory,
        IVnStatService vnStatService, IVpnService vpnService)
    {
        _httpClientFactory = httpClientFactory;
        _vnStatService = vnStatService;
        _vpnService = vpnService;
    }

    public async Task<StatusResponse> GetStatus(
        bool vpnConnected)
    {
        return new StatusResponse
        {
            VpnConnected = vpnConnected,
            Hostname = Environment.MachineName,
            PublicIp = await GetPublicIpAsync(),
            MemoryUsage = GetMemoryUsage(),
            Uptime = GetUptime(),
            CpuUsage = GetCpuUsage(),
            Timestamp = DateTime.UtcNow,
            Traffic = await _vnStatService.GetTrafficAsync(),
            RoutingMode = await _vpnService.GetRoutingModeAsync()
        };
    }

    private async Task<string> GetPublicIpAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient();

            return await client.GetStringAsync(
                "https://api.ipify.org");
        }
        catch
        {
            return "Unavailable";
        }
    }

    private double GetMemoryUsage()
    {
        try
        {
            var lines = File.ReadAllLines(
                "/proc/meminfo");

            var total = ParseKb(
                lines,
                "MemTotal");

            var available = ParseKb(
                lines,
                "MemAvailable");

            return Math.Round(
                ((double)(total - available) / total) * 100,
                1);
        }
        catch
        {
            return 0;
        }
    }

    private static long ParseKb(
        string[] lines,
        string key)
    {
        var line = lines.First(
            x => x.StartsWith(key));

        var value = line
            .Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries)[1];

        return long.Parse(value);
    }

    private string GetUptime()
    {
        try
        {
            var uptimeSeconds =
                double.Parse(
                    File.ReadAllText("/proc/uptime")
                        .Split(' ')[0],
                    CultureInfo.InvariantCulture);

            var uptime =
                TimeSpan.FromSeconds(
                    uptimeSeconds);

            return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m";
        }
        catch
        {
            return "-";
        }
    }

    private double GetCpuUsage()
    {
        try
        {
            var first = ReadCpuStat();

            Thread.Sleep(200);

            var second = ReadCpuStat();

            var idle =
                second.Idle - first.Idle;

            var total =
                second.Total - first.Total;

            if (total == 0)
            {
                return 0;
            }

            return Math.Round(
                100.0 * (1.0 - ((double)idle / total)),
                1);
        }
        catch
        {
            return 0;
        }
    }
        private static CpuStat ReadCpuStat()
    {
        var line =
            File.ReadLines("/proc/stat")
                .First();

        var values =
            line.Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries)
                .Skip(1)
                .Select(long.Parse)
                .ToArray();

        var idle =
            values[3];

        var total =
            values.Sum();

        return new CpuStat
        {
            Idle = idle,
            Total = total
        };
    }
        private class CpuStat
    {
        public long Idle { get; set; }

        public long Total { get; set; }
    }
}