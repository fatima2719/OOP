using System;
using System.Collections.Generic;
using BankManagementSystem.Model;
using BankManagementSystem.Repository;

namespace BankManagementSystem.Service
{
    public class AccountService
    {
        private readonly AccountRepo repository;

        public AccountService(AccountRepo repository)
        {
            this.repository = repository;
        }

        public List<AccountModel> GetAllAccounts()
        {
            return repository.GetAll();
        }

        public AccountModel GetAccount(int accountNumber)
        {
            return repository.FindByAccountNumber(accountNumber);
        }

        public int GenerateAccountNumber()
        {
            int highest = 1000;

            foreach (AccountModel account in repository.GetAll())
            {
                if (account.AccountNumber > highest)
                {
                    highest = account.AccountNumber;
                }
            }

            return highest + 1;
        }

        public bool OpenAccount(string name, string mobile, string pin,
            decimal initialBalance, out string message)
        {
            message = "";

            if (string.IsNullOrWhiteSpace(name))
            {
                message = "Name cannot be empty.";
                return false;
            }

            if (!IsValidMobile(mobile))
            {
                message = "Enter a valid 11-digit mobile number.";
                return false;
            }

            if (!IsValidPin(pin))
            {
                message = "PIN must contain exactly 4 digits.";
                return false;
            }

            if (initialBalance < 0)
            {
                message = "Initial balance cannot be negative.";
                return false;
            }

            int accountNumber = GenerateAccountNumber();

            AccountModel account = new AccountModel
            {
                AccountNumber = accountNumber,
                Name = name.Trim(),
                Mobile = mobile.Trim(),
                Pin = pin,
                Balance = initialBalance
            };

            if (!repository.Add(account))
            {
                message = "Unable to create the account.";
                return false;
            }

            message = "Account opened successfully. Your account number is " + accountNumber + ".";
            return true;
        }

        public bool Deposit(int accountNumber, decimal amount, out string message)
        {
            AccountModel account = repository.FindByAccountNumber(accountNumber);

            if (account == null)
            {
                message = "Account not found.";
                return false;
            }

            if (amount <= 0)
            {
                message = "Deposit amount must be greater than zero.";
                return false;
            }

            account.Balance += amount;
            repository.SaveAccounts();

            message = "Deposit completed successfully. New balance: " + account.Balance.ToString("N2");
            return true;
        }

        public bool Withdraw(int accountNumber, decimal amount, out string message)
        {
            AccountModel account = repository.FindByAccountNumber(accountNumber);

            if (account == null)
            {
                message = "Account not found.";
                return false;
            }

            if (amount <= 0)
            {
                message = "Withdrawal amount must be greater than zero.";
                return false;
            }

            if (amount > account.Balance)
            {
                message = "Insufficient balance.";
                return false;
            }

            account.Balance -= amount;
            repository.SaveAccounts();

            message = "Withdrawal completed successfully. Remaining balance: " + account.Balance.ToString("N2");
            return true;
        }

        public bool EditAccount(int accountNumber, string newName, string newMobile,
            out string message)
        {
            AccountModel account = repository.FindByAccountNumber(accountNumber);

            if (account == null)
            {
                message = "Account not found.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(newName))
            {
                message = "Name cannot be empty.";
                return false;
            }

            if (!IsValidMobile(newMobile))
            {
                message = "Enter a valid 11-digit mobile number.";
                return false;
            }

            account.Name = newName.Trim();
            account.Mobile = newMobile.Trim();
            repository.SaveAccounts();

            message = "Account details updated successfully.";
            return true;
        }

        public bool DeleteAccount(int accountNumber, string pin, out string message)
        {
            AccountModel account = repository.FindByAccountNumber(accountNumber);

            if (account == null)
            {
                message = "Account not found.";
                return false;
            }

            if (account.Pin != pin)
            {
                message = "Incorrect PIN. Account was not deleted.";
                return false;
            }

            if (account.Balance > 0)
            {
                message = "Account cannot be deleted while it has a balance. Withdraw or transfer the funds first.";
                return false;
            }

            repository.Delete(accountNumber);
            message = "Account deleted successfully.";
            return true;
        }

        public bool Transfer(int fromAccountNumber, int toAccountNumber,
            decimal amount, out string message)
        {
            AccountModel fromAccount = repository.FindByAccountNumber(fromAccountNumber);
            AccountModel toAccount = repository.FindByAccountNumber(toAccountNumber);

            if (fromAccount == null || toAccount == null)
            {
                message = "One or both account numbers are invalid.";
                return false;
            }

            if (fromAccountNumber == toAccountNumber)
            {
                message = "Source and destination accounts must be different.";
                return false;
            }

            if (amount <= 0)
            {
                message = "Transfer amount must be greater than zero.";
                return false;
            }

            if (amount > fromAccount.Balance)
            {
                message = "Insufficient balance.";
                return false;
            }

            fromAccount.Balance -= amount;
            toAccount.Balance += amount;
            repository.SaveAccounts();

            message = "Transfer completed successfully.";
            return true;
        }

        public bool ChangePin(int accountNumber, string oldPin, string newPin,
            out string message)
        {
            AccountModel account = repository.FindByAccountNumber(accountNumber);

            if (account == null)
            {
                message = "Account not found.";
                return false;
            }

            if (account.Pin != oldPin)
            {
                message = "Incorrect old PIN.";
                return false;
            }

            if (!IsValidPin(newPin))
            {
                message = "New PIN must contain exactly 4 digits.";
                return false;
            }

            if (newPin == oldPin)
            {
                message = "New PIN must be different from the old PIN.";
                return false;
            }

            account.Pin = newPin;
            repository.SaveAccounts();

            message = "PIN updated successfully.";
            return true;
        }

        public decimal GetTotalBankBalance()
        {
            decimal total = 0;

            foreach (AccountModel account in repository.GetAll())
            {
                total += account.Balance;
            }

            return total;
        }

        private bool IsValidPin(string pin)
        {
            if (string.IsNullOrWhiteSpace(pin) || pin.Length != 4)
            {
                return false;
            }

            foreach (char character in pin)
            {
                if (!char.IsDigit(character))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsValidMobile(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile) || mobile.Length != 11)
            {
                return false;
            }

            foreach (char character in mobile)
            {
                if (!char.IsDigit(character))
                {
                    return false;
                }
            }

            return true;
        }
    }
}