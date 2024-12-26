using Grpc.Net.Client;
using Myaccountservice;
using System.Threading.Tasks;

public class GrpcClient
{
    private readonly AccountService.AccountServiceClient _client;

    public GrpcClient()
    {
        var channel = GrpcChannel.ForAddress("https://localhost:5001");
        _client = new AccountService.AccountServiceClient(channel);
    }

    public async Task<AccountStatementResponse> GetAccountStatement(string accountNumber)
    {
        var request = new AccountStatementRequest { AccountNumber = accountNumber };
        var response = await _client.GetAccountStatementAsync(request); // Call the gRPC method
        return response;
    }
}
