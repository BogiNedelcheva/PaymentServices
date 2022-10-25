using System;
using PaymentServices.Types;

namespace PaymentServices.Data
{
    public interface IDataStore
    {
        Account GetAccount(string accountNumber);

        void UpdateAccount(Account account);

        List<Account> Accounts { get; set; }

        
    }
}


