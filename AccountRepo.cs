using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using BankManagementSystem.Model;

namespace BankManagementSystem.Repository
{
    public class AccountRepo
    {
        private readonly List<AccountModel> accounts;
        private readonly string dataDirectory;
        private readonly string filePath;

        public AccountRepo()
        {
            accounts = new List<AccountModel>();
            dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            filePath = Path.Combine(dataDirectory, "accounts.txt");

            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }

            if (File.Exists(filePath))
            {
                LoadAccounts();
            }
            else
            {
                CreateSampleAccounts();
                SaveAccounts();
            }
        }

        public List<AccountModel> GetAll()
        {
            return accounts;
        }

        public AccountModel FindByAccountNumber(int accountNumber)
        {
            foreach (AccountModel account in accounts)
            {
                if (account.AccountNumber == accountNumber)
                {
                    return account;
                }
            }

            return null;
        }

        public bool Add(AccountModel account)
        {
            if (account == null || FindByAccountNumber(account.AccountNumber) != null)
            {
                return false;
            }

            accounts.Add(account);
            SaveAccounts();
            return true;
        }

        public bool Delete(int accountNumber)
        {
            AccountModel account = FindByAccountNumber(accountNumber);

            if (account == null)
            {
                return false;
            }

            accounts.Remove(account);
            SaveAccounts();
            return true;
        }

        public void SaveAccounts()
        {
            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }

            string temporaryPath = filePath + ".tmp";

            using (StreamWriter writer = new StreamWriter(temporaryPath, false))
            {
                foreach (AccountModel account in accounts)
                {
                    writer.WriteLine(
                        account.AccountNumber + "|" +
                        CleanField(account.Name) + "|" +
                        CleanField(account.Mobile) + "|" +
                        CleanField(account.Pin) + "|" +
                        account.Balance.ToString(CultureInfo.InvariantCulture)
                    );
                }
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            File.Move(temporaryPath, filePath);
        }

        private void LoadAccounts()
        {
            List<AccountModel> loadedAccounts = new List<AccountModel>();

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    string[] fields = line.Split('|');

                    if (fields.Length != 5)
                    {
                        continue;
                    }

                    int accountNumber;
                    decimal balance;

                    if (!int.TryParse(fields[0], out accountNumber) ||
                        !decimal.TryParse(fields[4], NumberStyles.Number,
                            CultureInfo.InvariantCulture, out balance) ||
                        accountNumber <= 0 || balance < 0 ||
                        string.IsNullOrWhiteSpace(fields[1]) ||
                        string.IsNullOrWhiteSpace(fields[2]) ||
                        string.IsNullOrWhiteSpace(fields[3]))
                    {
                        continue;
                    }

                    if (ContainsAccountNumber(loadedAccounts, accountNumber))
                    {
                        continue;
                    }

                    loadedAccounts.Add(new AccountModel
                    {
                        AccountNumber = accountNumber,
                        Name = fields[1],
                        Mobile = fields[2],
                        Pin = fields[3],
                        Balance = balance
                    });
                }
            }

            accounts.Clear();
            accounts.AddRange(loadedAccounts);
        }

        private bool ContainsAccountNumber(List<AccountModel> list, int accountNumber)
        {
            foreach (AccountModel account in list)
            {
                if (account.AccountNumber == accountNumber)
                {
                    return true;
                }
            }

            return false;
        }

        private string CleanField(string value)
        {
            return (value ?? "").Replace("|", " ").Replace("\r", " ").Replace("\n", " ");
        }

        private void CreateSampleAccounts()
        {
            accounts.Add(new AccountModel
            {
                AccountNumber = 1001,
                Name = "Fatima",
                Mobile = "03001234567",
                Pin = "1234",
                Balance = 5000.50m
            });
            accounts.Add(new AccountModel
            {
                AccountNumber = 1002,
                Name = "Ali",
                Mobile = "03111234567",
                Pin = "2345",
                Balance = 10000.00m
            });
            accounts.Add(new AccountModel
            {
                AccountNumber = 1003,
                Name = "Sara",
                Mobile = "03221234567",
                Pin = "3456",
                Balance = 7500.75m
            });
            accounts.Add(new AccountModel
            {
                AccountNumber = 1004,
                Name = "Ayesha",
                Mobile = "03331234567",
                Pin = "4567",
                Balance = 3000.00m
            });
            accounts.Add(new AccountModel
            {
                AccountNumber = 1005,
                Name = "Usman",
                Mobile = "03441234567",
                Pin = "5678",
                Balance = 15000.25m
            });
        }
    }
}
