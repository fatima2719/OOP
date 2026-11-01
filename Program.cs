using BankManagementSystem.Repository;
using BankManagementSystem.Service;
using BankManagementSystem.UI;

namespace BankManagementSystem
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            AccountRepo repository = new AccountRepo();
            AccountService service = new AccountService(repository);
            BankUI bankUI = new BankUI(service);

            bankUI.Run();
        }
    }
}
