using System;
using System.Configuration;
using Microsoft.VisualBasic;
using PaymentServices.Data;
using PaymentServices.Types;
namespace PaymentServices
{
    public interface  IDataStoreFactory
    {
      IDataStore CreateDataStore(string dataStoreType);
        
    }
}