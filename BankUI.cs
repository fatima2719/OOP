using System;
using System.Collections.Generic;
using BankManagementSystem.Model;
using BankManagementSystem.Service;

namespace BankManagementSystem.UI
{
    public class BankUI
    {
        private readonly AccountService service;

        public BankUI(AccountService service)
        {
            this.service = service;
        }

        public void Run()
        {
            bool running = true;

            while (running)
            {
                Console.Clear();
                ShowMenu();

                Console.Write("Enter your option: ");
                int choice;

                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    ShowMessage("Invalid input. Please enter a menu number.");
                    continue;
                }

                if (choice == 1)
                {
                    OpenAccount();
                }
                else if (choice == 2)
                {
                    ShowAccountDetails();
                }
                else if (choice == 3)
                {
                    Deposit();
                }
                else if (choice == 4)
                {
                    Withdraw();
                }
                else if (choice == 5)
                {
                    EditAccount();
                }
                else if (choice == 6)
                {
                    DeleteAccount();
                }
                else if (choice == 7)
                {
                    SearchAccount();
                }
                else if (choice == 8)
                {
                    TransferMoney();
                }
                else if (choice == 9)
                {
                    ShowAllAccounts();
                }
                else if (choice == 10)
                {
                    ChangePin();
                }
                else if (choice == 11)
                {
                    ShowTotalBankBalance();
                }
                else if (choice == 12)
                {
                    running = false;
                }
                else
                {
                    ShowMessage("Wrong option. Please select 1 to 12.");
                }
            }

            Console.WriteLine();
            Console.WriteLine("Thanks for using Bank Management System.");
            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
        }

        private void ShowMenu()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("======== BANK MANAGEMENT SYSTEM ========");
            Console.WriteLine("========================================");
            Console.WriteLine("1.  Open an account");
            Console.WriteLine("2.  Show account details");
            Console.WriteLine("3.  Deposit");
            Console.WriteLine("4.  Withdraw");
            Console.WriteLine("5.  Edit account details");
            Console.WriteLine("6.  Delete account");
            Console.WriteLine("7.  Search account");
            Console.WriteLine("8.  Transfer money");
            Console.WriteLine("9.  Show all accounts");
            Console.WriteLine("10. Change PIN");
            Console.WriteLine("11. Total bank balance");
            Console.WriteLine("12. Exit");
            Console.WriteLine("========================================");
        }

        private void OpenAccount()
        {
            Console.Clear();
            Console.WriteLine("===== OPEN NEW ACCOUNT =====");

            Console.Write("Enter name: ");
            string name = Console.ReadLine();

            Console.Write("Enter mobile number (11 digits): ");
            string mobile = Console.ReadLine();

            Console.Write("Create 4-digit PIN: ");
            string pin = Console.ReadLine();

            decimal initialBalance;
            Console.Write("Enter initial balance: ");

            if (!decimal.TryParse(Console.ReadLine(), out initialBalance))
            {
                ShowMessage("Invalid balance.");
                return;
            }

            string message;
            service.OpenAccount(name, mobile, pin, initialBalance, out message);
            ShowMessage(message);
        }

        private void ShowAccountDetails()
        {
            Console.Clear();
            Console.WriteLine("===== ACCOUNT DETAILS =====");

            int accountNumber = ReadAccountNumber();
            if (accountNumber <= 0) return;

            AccountModel account = service.GetAccount(accountNumber);

            if (account == null)
            {
                ShowMessage("Account not found.");
                return;
            }

            PrintAccount(account, false);
            Pause();
        }

        private void Deposit()
        {
            Console.Clear();
            Console.WriteLine("===== DEPOSIT MONEY =====");

            int accountNumber = ReadAccountNumber();
            if (accountNumber <= 0) return;

            decimal amount = ReadAmount("Enter amount to deposit: ");
            if (amount < 0) return;

            string message;
            service.Deposit(accountNumber, amount, out message);
            ShowMessage(message);
        }

        private void Withdraw()
        {
            Console.Clear();
            Console.WriteLine("===== WITHDRAW MONEY =====");

            int accountNumber = ReadAccountNumber();
            if (accountNumber <= 0) return;

            decimal amount = ReadAmount("Enter amount to withdraw: ");
            if (amount < 0) return;

            string message;
            service.Withdraw(accountNumber, amount, out message);
            ShowMessage(message);
        }

        private void EditAccount()
        {
            Console.Clear();
            Console.WriteLine("===== EDIT ACCOUNT DETAILS =====");

            int accountNumber = ReadAccountNumber();
            if (accountNumber <= 0) return;

            Console.Write("Enter new name: ");
            string name = Console.ReadLine();

            Console.Write("Enter new mobile number (11 digits): ");
            string mobile = Console.ReadLine();

            string message;
            service.EditAccount(accountNumber, name, mobile, out message);
            ShowMessage(message);
        }

        private void DeleteAccount()
        {
            Console.Clear();
            Console.WriteLine("===== DELETE ACCOUNT =====");

            int accountNumber = ReadAccountNumber();
            if (accountNumber <= 0) return;

            Console.Write("Enter account PIN to confirm deletion: ");
            string pin = Console.ReadLine();

            Console.Write("Are you sure you want to delete this account? (Y/N): ");
            string confirmation = Console.ReadLine();

            if (!string.Equals(confirmation, "Y", StringComparison.OrdinalIgnoreCase))
            {
                ShowMessage("Account deletion cancelled.");
                return;
            }

            string message;
            service.DeleteAccount(accountNumber, pin, out message);
            ShowMessage(message);
        }

        private void SearchAccount()
        {
            Console.Clear();
            Console.WriteLine("===== SEARCH ACCOUNT =====");

            int accountNumber = ReadAccountNumber();
            if (accountNumber <= 0) return;

            AccountModel account = service.GetAccount(accountNumber);

            if (account == null)
            {
                ShowMessage("Account not found.");
                return;
            }

            PrintAccount(account, false);
            Pause();
        }

        private void TransferMoney()
        {
            Console.Clear();
            Console.WriteLine("===== TRANSFER MONEY =====");

            Console.Write("Transfer from account number: ");
            int fromAccount;

            if (!int.TryParse(Console.ReadLine(), out fromAccount) || fromAccount <= 0)
            {
                ShowMessage("Invalid source account number.");
                return;
            }

            Console.Write("Transfer to account number: ");
            int toAccount;

            if (!int.TryParse(Console.ReadLine(), out toAccount) || toAccount <= 0)
            {
                ShowMessage("Invalid destination account number.");
                return;
            }

            decimal amount = ReadAmount("Enter transfer amount: ");
            if (amount < 0) return;

            string message;
            service.Transfer(fromAccount, toAccount, amount, out message);
            ShowMessage(message);
        }

        private void ShowAllAccounts()
        {
            Console.Clear();
            Console.WriteLine("===== ALL BANK ACCOUNTS =====");
            Console.WriteLine("{0,-15} {1,-22} {2,-15} {3,15}",
                "ACCOUNT NO.", "NAME", "MOBILE", "BALANCE");
            Console.WriteLine(new string('-', 72));

            List<AccountModel> accounts = service.GetAllAccounts();

            if (accounts.Count == 0)
            {
                Console.WriteLine("No accounts found.");
            }
            else
            {
                foreach (AccountModel account in accounts)
                {
                    Console.WriteLine("{0,-15} {1,-22} {2,-15} {3,15:N2}",
                        account.AccountNumber, account.Name, account.Mobile, account.Balance);
                }
            }

            Console.WriteLine();
            Console.WriteLine("Total accounts: " + accounts.Count);
            Pause();
        }

        private void ChangePin()
        {
            Console.Clear();
            Console.WriteLine("===== CHANGE ACCOUNT PIN =====");

            int accountNumber = ReadAccountNumber();
            if (accountNumber <= 0) return;

            Console.Write("Enter old PIN: ");
            string oldPin = Console.ReadLine();

            Console.Write("Enter new 4-digit PIN: ");
            string newPin = Console.ReadLine();

            string message;
            service.ChangePin(accountNumber, oldPin, newPin, out message);
            ShowMessage(message);
        }

        private void ShowTotalBankBalance()
        {
            Console.Clear();
            Console.WriteLine("===== TOTAL BANK BALANCE =====");
            Console.WriteLine("Total balance across all accounts: PKR {0:N2}",
                service.GetTotalBankBalance());
            Pause();
        }

        private int ReadAccountNumber()
        {
            Console.Write("Enter account number: ");
            int accountNumber;

            if (!int.TryParse(Console.ReadLine(), out accountNumber) || accountNumber <= 0)
            {
                ShowMessage("Invalid account number.");
                return -1;
            }

            return accountNumber;
        }

        private decimal ReadAmount(string prompt)
        {
            Console.Write(prompt);
            decimal amount;

            if (!decimal.TryParse(Console.ReadLine(), out amount) || amount < 0)
            {
                ShowMessage("Please enter a valid non-negative amount.");
                return -1;
            }

            return amount;
        }

        private void PrintAccount(AccountModel account, bool includePin)
        {
            Console.WriteLine();
            Console.WriteLine("Account Number: " + account.AccountNumber);
            Console.WriteLine("Name: " + account.Name);
            Console.WriteLine("Mobile: " + account.Mobile);
            Console.WriteLine("Balance: PKR " + account.Balance.ToString("N2"));

            if (includePin)
            {
                Console.WriteLine("PIN: " + account.Pin);
            }
        }

        private void ShowMessage(string message)
        {
            Console.WriteLine();
            Console.WriteLine(message);
            Pause();
        }

        private void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}