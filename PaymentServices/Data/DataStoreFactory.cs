using System;
using System.Configuration;
using PaymentServices.Data;

namespace PaymentServices
{
    public class DataStoreFactory:IDataStoreFactory
    {
        public IDataStore CreateDataStore(string dataStoreType)
        {
            if (string.IsNullOrEmpty(dataStoreType))
            {
                dataStoreType = ConfigurationManager.AppSettings["DataStoreType"];
            }

            if (dataStoreType != "Backup")
            {
                return new AccountDataStore();

            }

            return new BackupAccountDataStore();
        }
    }
}

