using System.Transactions;

namespace Users.Infrastructure.General
{
    internal static class Helpers
    {
        public static string GenerateSalt()
        {
            return Guid.NewGuid().ToString();
        }
        public static TransactionScope CreateTransactionScope(int seconds = 60)
        {
            return new TransactionScope(
                TransactionScopeOption.Required,
                TimeSpan.FromSeconds(seconds),
                TransactionScopeAsyncFlowOption.Enabled);
        }
    }
}
