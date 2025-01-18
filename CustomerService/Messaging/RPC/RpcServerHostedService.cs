namespace CustomerService.Messaging.RPC;

public class RpcServerHostedService(CustomerRpcServer rpcServer) : IHostedService
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