namespace WyseControl.Services;

public interface IVpnService
{
    bool IsConnected();

    Task EnableRoutingAsync();

    Task DisableRoutingAsync();

    Task RebootAsync();

    Task<string> GetRoutingModeAsync();

    Task StartVpnAsync();

    Task StopVpnAsync();
}