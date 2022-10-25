using System.Security.Principal;
using PaymentServices.Types;

namespace PaymentServices.Data
{
    public class BackupAccountDataStore:IDataStore
    {
        public Account GetAccount(string accountNumber)
        {
            Account account = Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber)!;

            return account;
        }

        public void UpdateAccount(Account account)
        {
        }

        public List<Account> Accounts { get; set; } = new List<Account>();

    }
}
