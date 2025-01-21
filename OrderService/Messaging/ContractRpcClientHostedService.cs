namespace OrderService.Messaging;

public class ContractRpcClientHostedService(ContractRpcClient rpcClient) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await rpcClient.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await rpcClient.DisposeAsync();
    }
}