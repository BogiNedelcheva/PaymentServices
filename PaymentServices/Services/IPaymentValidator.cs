using System;
using PaymentServices.Types;

namespace PaymentServices.Services
{
    public interface IPaymentValidator
    {

        bool IsValidAccountState(Account account,MakePaymentRequest request);

    }
}

