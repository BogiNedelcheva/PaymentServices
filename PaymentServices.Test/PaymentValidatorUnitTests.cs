using System;
using System.Configuration;
using System.Net.NetworkInformation;
using NUnit.Framework;
using PaymentServices.Data;
using PaymentServices.Services;
using PaymentServices.Types;

namespace PaymentServices.Test;

public class PaymentValidatorUnitTests
{
    private const decimal AMMOUNT = 6.5m;
    private const string ACCOUNT_NUMBER_FIRST = "123";
    private const string ACCOUNT_NUMBER_SECOND = "13451";
    private const string ACCOUNT_NUMBER_THIRD = "23452";

    private const string ACCOUNT_DATASTORE_TYPE = "DataStoreType";
    private const string BACKUP_ACCOUNT_DATASTORE_TYPE = "Backup";

    private IPaymentValidator? paymentValidator;
    private IPaymentService? paymentService;

    private IDataStore? dataStore;

    private Account? firstAccount;
    private Account? secondAccount;

    [SetUp]
    public void Setup()
    {
        var accountDataStoreFactory = new DataStoreFactory();

        this.dataStore = accountDataStoreFactory!.CreateDataStore(ACCOUNT_DATASTORE_TYPE);


        this.paymentValidator = new PaymentValidator(dataStore);

        this.paymentService = new PaymentService(paymentValidator, dataStore);

        firstAccount = new Account()
        {
            AccountNumber = ACCOUNT_NUMBER_FIRST,
            Balance = AMMOUNT,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
            Status = AccountStatus.Live
        };
        secondAccount = new Account()
        {
            AccountNumber = ACCOUNT_NUMBER_SECOND,
            Balance = AMMOUNT,
            AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
            Status = AccountStatus.Live
        };

        dataStore!.Accounts.AddRange(new Account[] { firstAccount });
    }


    [Test]
    public void ShouldIsValidAccountstateReturnsTrueWhenAccountExist()
    {
        MakePaymentRequest request = new MakePaymentRequest()
        {
            Amount = AMMOUNT,
            CreditorAccountNumber = firstAccount!.AccountNumber!,
            DebtorAccountNumber = firstAccount!.AccountNumber!,
            PaymentDate = DateTime.Now,
            PaymentScheme = PaymentScheme.Chaps
        };

        var result = paymentValidator!.IsValidAccountState(firstAccount!, request);

        Assert.AreEqual(true, result);
    }

    [Test]
    public void ShouldIsValidAccountStateReturnsFalseWhenAccountDoesNotExist()
    {

        MakePaymentRequest request = new MakePaymentRequest()
        {
            Amount = AMMOUNT,
            CreditorAccountNumber = ACCOUNT_NUMBER_FIRST,
            DebtorAccountNumber = ACCOUNT_NUMBER_FIRST,
            PaymentDate = DateTime.Now,
            PaymentScheme = PaymentScheme.Chaps
        };

        var result = paymentValidator!.IsValidAccountState(secondAccount!, request);

        Assert.IsFalse(result);
    }

    [Test]
    // fails next  rows
    //[TestCase(AccountStatus.Live, AllowedPaymentSchemes.Chaps, PaymentScheme.Chaps)]
    //[TestCase(AccountStatus.Live, AllowedPaymentSchemes.Bacs, PaymentScheme.Bacs)]
    //[TestCase(AccountStatus.Live, AllowedPaymentSchemes.FasterPayments, PaymentScheme.FasterPayments)]

    //[TestCase(AccountStatus.Disabled, AllowedPaymentSchemes.Bacs, PaymentScheme.Bacs)]
    //[TestCase(AccountStatus.Disabled, AllowedPaymentSchemes.FasterPayments, PaymentScheme.FasterPayments)]

    //[TestCase(AccountStatus.InboundPaymentsOnly, AllowedPaymentSchemes.Bacs, PaymentScheme.Bacs)]
    //[TestCase(AccountStatus.InboundPaymentsOnly, AllowedPaymentSchemes.FasterPayments, PaymentScheme.FasterPayments)]

    [TestCase(AccountStatus.InboundPaymentsOnly, AllowedPaymentSchemes.Chaps, PaymentScheme.Chaps)]
    [TestCase(AccountStatus.Disabled, AllowedPaymentSchemes.Chaps, PaymentScheme.Chaps)]
    public void ShouldIsValidAccountStateReturnsFalseWhenAccountStatusIsNotValid(AccountStatus accountStatus, AllowedPaymentSchemes allowedPaymentSchemes, PaymentScheme scheme)
    {
        firstAccount!.Status = accountStatus;
        firstAccount!.AllowedPaymentSchemes = allowedPaymentSchemes;

        MakePaymentRequest request = new MakePaymentRequest()
        {
            Amount = AMMOUNT,
            CreditorAccountNumber = firstAccount.AccountNumber!,
            DebtorAccountNumber = firstAccount.AccountNumber!,
            PaymentDate = DateTime.Now,
            PaymentScheme = scheme,
        };

        var result = paymentValidator!.IsValidAccountState(firstAccount!, request);

        Assert.IsFalse(result);

    }

    //todo: acountStatus.Live when Payment.scheme == chaps?/ stepenuvane?
    [Test]
    [TestCase(AccountStatus.Live, AllowedPaymentSchemes.Chaps, PaymentScheme.Chaps)]
    [TestCase(AccountStatus.Live, AllowedPaymentSchemes.Bacs, PaymentScheme.Bacs)]
    [TestCase(AccountStatus.Live, AllowedPaymentSchemes.FasterPayments, PaymentScheme.FasterPayments)]

    // fails next 2 rows
    //[TestCase(AccountStatus.InboundPaymentsOnly, AllowedPaymentSchemes.Chaps, PaymentScheme.Chaps)]
    //[TestCase(AccountStatus.Disabled, AllowedPaymentSchemes.Chaps, PaymentScheme.Chaps)]

    [TestCase(AccountStatus.Disabled, AllowedPaymentSchemes.Bacs, PaymentScheme.Bacs)]
    [TestCase(AccountStatus.Disabled, AllowedPaymentSchemes.FasterPayments, PaymentScheme.FasterPayments)]

    [TestCase(AccountStatus.InboundPaymentsOnly, AllowedPaymentSchemes.Bacs, PaymentScheme.Bacs)]
    [TestCase(AccountStatus.InboundPaymentsOnly, AllowedPaymentSchemes.FasterPayments, PaymentScheme.FasterPayments)]

    public void ShouldIsValidAccountStateReturnsTrueWhenAccountStatusIsValid(AccountStatus accountStatus, AllowedPaymentSchemes allowedPaymentSchemes, PaymentScheme scheme)
    {
        firstAccount!.Status = accountStatus;
        firstAccount!.AllowedPaymentSchemes = allowedPaymentSchemes;

        MakePaymentRequest request = new MakePaymentRequest()
        {
            Amount = AMMOUNT,
            CreditorAccountNumber = firstAccount.AccountNumber!,
            DebtorAccountNumber = firstAccount.AccountNumber!,
            PaymentDate = DateTime.Now,
            PaymentScheme = scheme,
        };

        var result = paymentValidator!.IsValidAccountState(firstAccount!, request);

        Assert.IsTrue(result);

    }

    [Test]
    [TestCase(AllowedPaymentSchemes.Bacs, PaymentScheme.Chaps)]
    [TestCase(AllowedPaymentSchemes.FasterPayments, PaymentScheme.Bacs)]
    [TestCase(AllowedPaymentSchemes.Chaps, PaymentScheme.FasterPayments)]
    public void ShouldIsValidAccountStateReturnsFalseWhenPaymentSchemesIsNotValid(AllowedPaymentSchemes allowedPaymentSchemes, PaymentScheme scheme)
    {
        firstAccount!.AllowedPaymentSchemes = allowedPaymentSchemes;

        MakePaymentRequest request = new MakePaymentRequest()
        {
            Amount = AMMOUNT,
            CreditorAccountNumber = firstAccount.AccountNumber!,
            DebtorAccountNumber = firstAccount.AccountNumber!,
            PaymentDate = DateTime.Now,
            PaymentScheme = scheme,
        };

        var result = paymentValidator!.IsValidAccountState(firstAccount!, request);

        Assert.IsFalse(result);
    }

    [Test]
    [TestCase(AllowedPaymentSchemes.Bacs, PaymentScheme.Bacs)]
    [TestCase(AllowedPaymentSchemes.FasterPayments, PaymentScheme.FasterPayments)]
    [TestCase(AllowedPaymentSchemes.Chaps, PaymentScheme.Chaps)]

    public void ShouldIsValidAccountStateReturnsTrueWhenPaymentSchemesIsValid(AllowedPaymentSchemes allowedPaymentSchemes, PaymentScheme scheme)
    {
        firstAccount!.AllowedPaymentSchemes = allowedPaymentSchemes;

        MakePaymentRequest request = new MakePaymentRequest()
        {
            Amount = AMMOUNT,
            CreditorAccountNumber = firstAccount.AccountNumber!,
            DebtorAccountNumber = firstAccount.AccountNumber!,
            PaymentDate = DateTime.Now,
            PaymentScheme = scheme,
        };

        var result = paymentValidator!.IsValidAccountState(firstAccount!, request);

        Assert.IsTrue(result);
    }

    [Test]
    [TestCase(-1, AccountStatus.Live, AllowedPaymentSchemes.Chaps, PaymentScheme.Chaps)]
    [TestCase(0, AccountStatus.Live, AllowedPaymentSchemes.Chaps, PaymentScheme.Chaps)]
    [TestCase(6.5, AccountStatus.Live, AllowedPaymentSchemes.Chaps, PaymentScheme.Chaps)]

    [TestCase(-1, AccountStatus.Live, AllowedPaymentSchemes.Bacs, PaymentScheme.Bacs)]
    [TestCase(0, AccountStatus.Live, AllowedPaymentSchemes.Bacs, PaymentScheme.Bacs)]
    [TestCase(6.5, AccountStatus.Live, AllowedPaymentSchemes.Bacs, PaymentScheme.Bacs)]

    [TestCase(-1, AccountStatus.Disabled, AllowedPaymentSchemes.Bacs, PaymentScheme.Bacs)]
    [TestCase(0, AccountStatus.Disabled, AllowedPaymentSchemes.Bacs, PaymentScheme.Bacs)]
    [TestCase(6.5, AccountStatus.Disabled, AllowedPaymentSchemes.Bacs, PaymentScheme.Bacs)]

    [TestCase(-1, AccountStatus.InboundPaymentsOnly, AllowedPaymentSchemes.Bacs, PaymentScheme.Bacs)]
    [TestCase(0, AccountStatus.InboundPaymentsOnly, AllowedPaymentSchemes.Bacs, PaymentScheme.Bacs)]
    [TestCase(6.5, AccountStatus.InboundPaymentsOnly, AllowedPaymentSchemes.Bacs, PaymentScheme.Bacs)]
    // next rows fails
    //[TestCase(-1,AccountStatus.Live, AllowedPaymentSchemes.FasterPayments, PaymentScheme.FasterPayments)]
    //[TestCase(0, AccountStatus.Live, AllowedPaymentSchemes.FasterPayments, PaymentScheme.FasterPayments)]

    //[TestCase(-1,AccountStatus.Disabled, AllowedPaymentSchemes.FasterPayments, PaymentScheme.FasterPayments)]
    //[TestCase(0, AccountStatus.Disabled, AllowedPaymentSchemes.FasterPayments, PaymentScheme.FasterPayments)]

    //[TestCase(-1,AccountStatus.InboundPaymentsOnly, AllowedPaymentSchemes.FasterPayments, PaymentScheme.FasterPayments)]
    //[TestCase(0, AccountStatus.InboundPaymentsOnly, AllowedPaymentSchemes.FasterPayments, PaymentScheme.FasterPayments)]


    [TestCase(6.5, AccountStatus.Live, AllowedPaymentSchemes.FasterPayments, PaymentScheme.FasterPayments)]
    [TestCase(6.5, AccountStatus.Disabled, AllowedPaymentSchemes.FasterPayments, PaymentScheme.FasterPayments)]
    [TestCase(6.5, AccountStatus.InboundPaymentsOnly, AllowedPaymentSchemes.FasterPayments, PaymentScheme.FasterPayments)]

    public void ShouldIsValidAccountStateReturnsTrueWhenAccountBalanceIsSufficient(decimal balance, AccountStatus accountStatus, AllowedPaymentSchemes allowedPaymentSchemes, PaymentScheme scheme)
    {
        firstAccount!.Balance = balance;
        firstAccount!.Status = accountStatus;
        firstAccount!.AllowedPaymentSchemes = allowedPaymentSchemes;

        MakePaymentRequest request = new MakePaymentRequest()
        {
            Amount = 6.5m,
            CreditorAccountNumber = ACCOUNT_NUMBER_FIRST,
            DebtorAccountNumber = ACCOUNT_NUMBER_FIRST,
            PaymentDate = DateTime.Now,
            PaymentScheme = scheme

        };

        var result = paymentValidator!.IsValidAccountState(firstAccount!, request);

        Assert.IsTrue(result);
    }
   
    [Test]
    [TestCase(-1, AllowedPaymentSchemes.FasterPayments, PaymentScheme.FasterPayments)]
    [TestCase(0, AllowedPaymentSchemes.FasterPayments, PaymentScheme.FasterPayments)]
    [TestCase(4.5, AllowedPaymentSchemes.FasterPayments, PaymentScheme.FasterPayments)]
    public void ShouldIsValidAccountStateReturnsFalseWhenAccountBalanceIsLow(decimal balance, AllowedPaymentSchemes allowedPaymentSchemes, PaymentScheme scheme)
    {
        firstAccount!.Balance = balance;
        firstAccount!.AllowedPaymentSchemes = allowedPaymentSchemes;

        MakePaymentRequest request = new MakePaymentRequest()
        {
            Amount = 6.5m,
            CreditorAccountNumber = ACCOUNT_NUMBER_FIRST,
            DebtorAccountNumber = ACCOUNT_NUMBER_FIRST,
            PaymentDate = DateTime.Now,
            PaymentScheme = scheme

        };

        var result = paymentValidator!.IsValidAccountState(firstAccount!, request);

        Assert.IsFalse(result);
    }
}

