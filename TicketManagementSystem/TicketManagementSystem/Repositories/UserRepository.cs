using System;
using System.Data.SqlClient;
using TicketManagementSystem.Interfaces.Repositories;
using TicketManagementSystem.Models;

namespace TicketManagementSystem.Repositories
{
    public class UserRepository : IUserRepository, IDisposable
    {
        private readonly SqlConnection _connection;
        public UserRepository()
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["database"].ConnectionString; 
            _connection = new SqlConnection(connectionString);
        }
        
        public User GetUser(string username)
        {
            // Assume this method does not need to change and is connected to a database with users populated.
            try
            {
                string sql = "SELECT TOP 1 FROM Users u WHERE u.Username == @p1";
                _connection.Open();

                var command = new SqlCommand(sql, _connection)
                {
                    CommandType = System.Data.CommandType.Text,
                };

                command.Parameters.Add("@p1", System.Data.SqlDbType.NVarChar).Value = username;

                return (User)command.ExecuteScalar();
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                _connection.Close();
            }
        }

        public User GetAccountManager()
        {
            // Assume this method does not need to change.
            return GetUser("Sarah");
        }

        public void Dispose()
        {
            // Assume this method does not need to change.
            _connection.Dispose();
        }
    }
}
