namespace ContractService.Messaging.RPC;

public class CustomerRpcClientHostedService(CustomerRpcClient rpcClient) : IHostedService
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