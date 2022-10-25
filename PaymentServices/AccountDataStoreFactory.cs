using System;
using System.Configuration;
using PaymentServices.Data;

namespace PaymentServices
{
    public class AccountDataStoreFactory:IDataStoreFactory
    {
        public IDataStore CreateDataStore(string dataStoreType)
        {
            if (string.IsNullOrEmpty(dataStoreType))
            {
                dataStoreType = ConfigurationManager.AppSettings["DataStoreType"];
            }

            return new AccountDataStore();
        }
    }
}

