using EmailService;
using System;
using System.Collections.Generic;
using TicketManagementSystem.Exceptions;
using TicketManagementSystem.Interfaces.Services;
using TicketManagementSystem.Models;
using TicketManagementSystem.Repositories;

namespace TicketManagementSystem
{
    public class TicketService : ITicketService
    {
        private readonly List<string> _priorityFlags = new() {"Crash", "Important", "Failure"};
        public int CreateTicket(string title, Priority priority, string assignedTo, string description, DateTime ticketDate, bool isPayingCustomer)
        {
            if (string.IsNullOrEmpty(description) 
                || string.IsNullOrEmpty(title))
                throw new InvalidTicketException("Title or description were null");

            using var userRepository = new UserRepository(); // Will be disposed when scope ends
            var user = userRepository.GetUser(assignedTo);

            if (user is null) 
                throw new UnknownUserException($"User {assignedTo} not found");

            priority = RaisePriority(priority, ticketDate, title);

            if (priority is Priority.High) 
                new EmailServiceProxy().SendEmailToAdministrator(title, assignedTo);

            var price = 0;
            User accountManager = default;

            if (isPayingCustomer) // Only paid customers have an account manager.
            {
                accountManager = userRepository.GetAccountManager();
                price = priority == Priority.High 
                    ? 100 
                    : 50;
            }

            return TicketRepository.CreateTicket(new Ticket
            {
                Title = title,
                AssignedUser = user,
                Priority = priority,
                Description = description,
                Created = ticketDate,
                PriceDollars = price,
                AccountManager = accountManager
            });
        }

        public void AssignTicket(int id, string username)
        {
            using var userRepository = new UserRepository(); // Will be disposed when scope ends
            var user = userRepository.GetUser(username);

            if (user is null)
                throw new UnknownUserException("User not found");

            var ticket = TicketRepository.GetTicket(id);

            if (ticket is null)
                throw new ApplicationException($"No ticket found for id {id}");

            ticket.AssignedUser = user;

            TicketRepository.UpdateTicket(ticket);
        }
        
        private Priority RaisePriority(Priority priority, DateTime ticketDate, string title)
        {
            if (ticketDate >= DateTime.UtcNow - TimeSpan.FromHours(1) 
                && !_priorityFlags.Contains(title)) 
                return priority;

            return priority switch
            {
                Priority.Low => Priority.Medium,
                Priority.Medium => Priority.High,
                _ => priority
            };

            /* will always raise priority but is dependent of the priority order of the enum.
            priority--;
            if (priority < 0)
                priority = 0;

            return priority;
            */
        }
    }
}
