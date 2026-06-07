using WyseControl.Services;

namespace WyseControl.Endpoints;

public static class VpnEndpoints
{
    public static void MapVpnEndpoints(
        this WebApplication app)
    {
        app.MapPost(
            "/api/vpn/on",
            async (IVpnService vpn) =>
            {
                await vpn.EnableRoutingAsync();

                return Results.Ok();
            });

        app.MapPost(
            "/api/vpn/off",
            async (IVpnService vpn) =>
            {
                await vpn.DisableRoutingAsync();

                return Results.Ok();
            });

        app.MapPost(
            "/api/system/reboot",
            async (IVpnService vpn) =>
            {
                await vpn.RebootAsync();

                return Results.Ok();
            });
    }
}