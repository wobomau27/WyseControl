using WyseControl.Services;

namespace WyseControl.Endpoints;

public static class StatusEndpoints
{
    public static void MapStatusEndpoints(
        this WebApplication app)
    {
        app.MapGet(
            "/api/status",
            async (
                IVpnService vpn,
                SystemInfoService systemInfo) =>
            {
                return await systemInfo.GetStatus(
                vpn.IsConnected());
            });
    }
}