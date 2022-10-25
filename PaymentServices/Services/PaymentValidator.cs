using System;
using PaymentServices.Data;
using PaymentServices.Types;
using System.Configuration;

namespace PaymentServices.Services
{

    public class PaymentValidator : IPaymentValidator
    {
        private IDataStore dataStore;

        public PaymentValidator(IDataStore dataStore)
        {
            this.dataStore = dataStore;
        }

        public bool IsValidAccountState(Account account, MakePaymentRequest request)
        {
            bool result = true;

            if (account == null || request.PaymentScheme.ToString() != account.AllowedPaymentSchemes.ToString())
            {
                return result = false;
            }

            switch (request.PaymentScheme)
            {
                case PaymentScheme.FasterPayments:
                    if (account.Balance < request.Amount)
                    {
                        result = false;
                    }

                    break;

                case PaymentScheme.Chaps:
                    if (account.Status != AccountStatus.Live)
                    {
                        result = false;
                    }

                    break;
            }

            // original code
            //switch (request.PaymentScheme)
            //{
            //    case PaymentScheme.Bacs:
            //        if (account == null)
            //        {
            //            result = false;
            //        }
            //        else if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs))
            //        {
            //            result = false;
            //        }
            //        break;

            //    case PaymentScheme.FasterPayments:
            //        if (account == null)
            //        {
            //            result = false;
            //        }
            //        else if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments))
            //        {
            //            result = false;
            //        }
            //        else if (account.Balance < request.Amount)
            //        {
            //            result = false;
            //        }
            //        break;

            //    case PaymentScheme.Chaps:
            //        if (account == null)
            //        {
            //            result = false;
            //        }
            //        else if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps))
            //        {
            //            result = false;
            //        }
            //        else if (account.Status != AccountStatus.Live)
            //        {
            //            result = false;
            //        }
            //        break;
            //}

            return result;
        }

    }
}

