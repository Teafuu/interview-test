using TicketManagementSystem.Models;

namespace TicketManagementSystem.Interfaces.Repositories
{
    internal interface IUserRepository
    {
        User GetUser(string username);
        User GetAccountManager();
    }
}
