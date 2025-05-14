using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using MySql.Data.MySqlClient;

namespace CampusLove.Infrastructure.Repositories
{
    public class UserRepository : IGenericRepository<User>, IUserRepository
    {
        private readonly MySqlConnection _connection;

        public UserRepository(MySqlConnection connection)
        {
            _connection = connection;
        }
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var userList = new List<User>();
            const string query = "SELECT id, username, password, profile_id, birthdate FROM user";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                userList.Add(new User
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Username = reader["username"].ToString() ?? string.Empty,
                    Password = reader["password"].ToString() ?? string.Empty,
                    ProfileId = Convert.ToInt32(reader["profile_id"]),
                    Birthdate = Convert.ToDateTime(reader["birthdate"])
                });
            }

            return userList;
        }

        public async Task<User?> GetByIdAsync(object id)
        {
            const string query = "SELECT id, username, password, profile_id, birthdate FROM user WHERE id = @Id";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Username = reader["username"].ToString() ?? string.Empty,
                    Password = reader["password"].ToString() ?? string.Empty,
                    ProfileId = Convert.ToInt32(reader["profile_id"]),
                    Birthdate = Convert.ToDateTime(reader["birthdate"])
                };
            }

            return null;
        }

        public async Task<bool> InsertAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            const string query = "INSERT INTO user (username, password, profile_id, birthdate) VALUES (@Username, @Password, @ProfileId, @Birthdate)";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@Password", user.Password);
                command.Parameters.AddWithValue("@ProfileId", user.ProfileId);
                command.Parameters.AddWithValue("@Birthdate", user.Birthdate);

                var result = await command.ExecuteNonQueryAsync() > 0;
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            const string query = "UPDATE user SET username = @Username, password = @Password, profile_id = @ProfileId, birthdate = @Birthdate WHERE id = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

             try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@Password", user.Password);
                command.Parameters.AddWithValue("@ProfileId", user.ProfileId);
                command.Parameters.AddWithValue("@Birthdate", user.Birthdate);

                var result = await command.ExecuteNonQueryAsync() > 0;
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(object id)
        {
            const string query = "DELETE FROM user WHERE id = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Id", id);

                var result = await command.ExecuteNonQueryAsync() > 0;
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty", nameof(username));

            const string query = "SELECT id, username, password, profile_id, birthdate FROM user WHERE username = @Username";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Username", username);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Username = reader["username"].ToString() ?? string.Empty,
                    Password = reader["password"].ToString() ?? string.Empty,
                    ProfileId = Convert.ToInt32(reader["profile_id"]),
                    Birthdate = Convert.ToDateTime(reader["birthdate"])
                };
            }

            return null;
        }
    }

}