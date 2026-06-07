using System.Diagnostics;

namespace WyseControl.Services;

public class WireGuardService : IVpnService
{
    public bool IsConnected()
    {
        return ExecuteShellWithExitCodeAsync(
            "systemctl is-active --quiet wg-quick@wg0")
            .GetAwaiter()
            .GetResult() == 0;
    }

    public async Task EnableRoutingAsync()
    {
        await StartVpnAsync();

        await Task.Delay(1000);

        await RemoveWanRulesAsync();

        await EnsureRuleAsync(
            "iptables -t nat -C POSTROUTING -o wg0 -j MASQUERADE",
            "iptables -t nat -A POSTROUTING -o wg0 -j MASQUERADE");

        await EnsureRuleAsync(
            "iptables -C FORWARD -i enp1s0.20 -o wg0 -j ACCEPT",
            "iptables -A FORWARD -i enp1s0.20 -o wg0 -j ACCEPT");

        await EnsureRuleAsync(
            "iptables -C FORWARD -i wg0 -o enp1s0.20 -m state --state RELATED,ESTABLISHED -j ACCEPT",
            "iptables -A FORWARD -i wg0 -o enp1s0.20 -m state --state RELATED,ESTABLISHED -j ACCEPT");
    }

    public async Task DisableRoutingAsync()
    {

        await ExecuteShellAsync(
            "iptables -t nat -D POSTROUTING -o wg0 -j MASQUERADE || true");

        await ExecuteShellAsync(
            "iptables -D FORWARD -i enp1s0.20 -o wg0 -j ACCEPT || true");

        await ExecuteShellAsync(
            "iptables -D FORWARD -i wg0 -o enp1s0.20 -m state --state RELATED,ESTABLISHED -j ACCEPT || true");

        await EnsureRuleAsync(
            "iptables -t nat -C POSTROUTING -o enp1s0 -j MASQUERADE",
            "iptables -t nat -A POSTROUTING -o enp1s0 -j MASQUERADE");

        await EnsureRuleAsync(
            "iptables -C FORWARD -i enp1s0.20 -o enp1s0 -j ACCEPT",
            "iptables -A FORWARD -i enp1s0.20 -o enp1s0 -j ACCEPT");

        await EnsureRuleAsync(
            "iptables -C FORWARD -i enp1s0 -o enp1s0.20 -m state --state RELATED,ESTABLISHED -j ACCEPT",
            "iptables -A FORWARD -i enp1s0 -o enp1s0.20 -m state --state RELATED,ESTABLISHED -j ACCEPT");

        await StopVpnAsync();
        
    }

    public async Task RebootAsync()
    {
        await ExecuteAsync(
            "/usr/sbin/reboot",
            "");
    }

    private static async Task RemoveWanRulesAsync()
    {
        await ExecuteShellAsync(
            "iptables -t nat -D POSTROUTING -o enp1s0 -j MASQUERADE || true");

        await ExecuteShellAsync(
            "iptables -D FORWARD -i enp1s0.20 -o enp1s0 -j ACCEPT || true");

        await ExecuteShellAsync(
            "iptables -D FORWARD -i enp1s0 -o enp1s0.20 -m state --state RELATED,ESTABLISHED -j ACCEPT || true");
    }

    private static async Task EnsureRuleAsync(
        string checkCommand,
        string addCommand)
    {
        var exitCode =
            await ExecuteShellWithExitCodeAsync(
                checkCommand);

        if (exitCode != 0)
        {
            await ExecuteShellAsync(
                addCommand);
        }
    }

    private static async Task ExecuteShellAsync(
        string command)
    {
        using var process = new Process();

        process.StartInfo.FileName = "/bin/bash";
        process.StartInfo.Arguments = $"-c \"{command}\"";
        process.StartInfo.UseShellExecute = false;

        process.Start();

        await process.WaitForExitAsync();
    }

    private static async Task<int> ExecuteShellWithExitCodeAsync(
        string command)
    {
        using var process = new Process();

        process.StartInfo.FileName = "/bin/bash";
        process.StartInfo.Arguments = $"-c \"{command}\"";
        process.StartInfo.UseShellExecute = false;

        process.Start();

        await process.WaitForExitAsync();

        return process.ExitCode;
    }

    private static async Task ExecuteAsync(
        string fileName,
        string arguments)
    {
        using var process = new Process();

        process.StartInfo.FileName = fileName;
        process.StartInfo.Arguments = arguments;
        process.StartInfo.UseShellExecute = false;

        process.Start();

        await process.WaitForExitAsync();
    }
    public async Task<string> GetRoutingModeAsync()
    {
        var exitCode =
            await ExecuteShellWithExitCodeAsync(
                "systemctl is-active --quiet wg-quick@wg0");

        return exitCode == 0
            ? "VPN"
            : "WAN";
    }
    public async Task StartVpnAsync()
    {
        await ExecuteAsync(
            "systemctl",
            "start wg-quick@wg0");
    }

    public async Task StopVpnAsync()
    {
        await ExecuteAsync(
            "systemctl",
            "stop wg-quick@wg0");
    }
}