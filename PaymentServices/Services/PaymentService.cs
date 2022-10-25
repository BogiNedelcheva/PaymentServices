using System.Configuration;
using System.Security.Principal;
using Microsoft.VisualBasic;
using PaymentServices.Data;
using PaymentServices.Types;

namespace PaymentServices.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentValidator paymentValidator;
    private readonly IDataStore dataStore;

    public PaymentService(IPaymentValidator paymentValidator, IDataStore dataStore)
    {
        this.paymentValidator = paymentValidator;
        this.dataStore = dataStore;

    }

    public MakePaymentResult MakePayment(MakePaymentRequest request)
    {

        var account = dataStore.GetAccount(request.DebtorAccountNumber);

        bool isValidAccountState = this.paymentValidator.IsValidAccountState(account, request);

        var result = new MakePaymentResult();

        if (isValidAccountState)
        {
            account.Balance -= request.Amount;

            this.dataStore.UpdateAccount(account);

            result.Success = true;
        }

        return result;

    }
}
