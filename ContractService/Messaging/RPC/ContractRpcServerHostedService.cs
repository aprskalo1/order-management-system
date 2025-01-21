namespace ContractService.Messaging.RPC;

public class ContractRpcServerHostedService(ContractRpcServer rpcServer) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await rpcServer.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await rpcServer.DisposeAsync();
    }
}