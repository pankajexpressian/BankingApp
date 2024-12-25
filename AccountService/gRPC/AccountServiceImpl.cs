using Grpc.Core;
using Myaccountservice;  // Import the namespace from the .proto file
using System.Collections.Generic;
using System.Threading.Tasks;
using AccountService;

public class AccountServiceImpl : Myaccountservice.AccountService.AccountServiceBase
{
    public override Task<AccountStatementResponse> GetAccountStatement(AccountStatementRequest request, ServerCallContext context)
    {
        // For now, just returning a dummy response with dummy transaction data
        var response = new AccountStatementResponse
        {
            AccountNumber = request.AccountNumber,
            Transactions = {
                new Transaction {
                    FromAccount = "123",
                    ToAccount = "456",
                    TransactionType = "Debit",
                    Amount = 100.00,
                    DateTime = "2024-12-25T12:00:00"
                }
            }
        };
        return Task.FromResult(response);
    }
}
