using System;

namespace TicketManagementSystem.Interfaces.Services
{
    internal interface ITicketService
    {
        int CreateTicket(string title, Priority priority, string assignedTo, string description, DateTime ticketDate,
            bool isPayingCustomer);

        void AssignTicket(int id, string username);
    }
}
