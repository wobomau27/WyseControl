using System.Diagnostics;
using System.Text.Json;
using WyseControl.Models;
using WyseControl.Models.VnStat;

namespace WyseControl.Services;

public interface IVnStatService
{
    Task<TrafficSummary> GetTrafficAsync();
}

public class VnStatService : IVnStatService
{
    public async Task<TrafficSummary> GetTrafficAsync()
    {
        if (OperatingSystem.IsMacOS())
        {
            return new TrafficSummary
            {
                Today = "Development",
                Month = "Development",
                Total = "Development"
            };
        }

        var json =
            await ExecuteAsync(
                "vnstat",
                "--json");

        var vnstat =
            JsonSerializer.Deserialize<VnStatRoot>(
                json);

        var wg =
            vnstat?.Interfaces
                .FirstOrDefault(
                    x => x.Name == "wg0");

        if (wg is null)
        {
            return new TrafficSummary();
        }

        var today =
            wg.Traffic.Day.LastOrDefault();

        var month =
            wg.Traffic.Month.LastOrDefault();

        return new TrafficSummary
        {
            Today = FormatBytes(
                (today?.Rx ?? 0) +
                (today?.Tx ?? 0)),

            Month = FormatBytes(
                (month?.Rx ?? 0) +
                (month?.Tx ?? 0)),

            Total = FormatBytes(
                wg.Traffic.Total.Rx +
                wg.Traffic.Total.Tx)
        };
    }

    private static string FormatBytes(
        long bytes)
    {
        const double gb =
            1024d * 1024d * 1024d;

        return $"{bytes / gb:F2} GB";
    }

    private static async Task<string> ExecuteAsync(
        string fileName,
        string arguments)
    {
        using var process =
            new Process();

        process.StartInfo.FileName =
            fileName;

        process.StartInfo.Arguments =
            arguments;

        process.StartInfo.RedirectStandardOutput =
            true;

        process.StartInfo.UseShellExecute =
            false;

        process.Start();

        return await process.StandardOutput
            .ReadToEndAsync();
    }
}