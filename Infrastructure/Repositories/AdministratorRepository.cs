using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using MySql.Data.MySqlClient;

namespace CampusLove.Infrastructure.Repositories
{
    public class AdministratorRepository : IAdministratorRepository
    {
        private readonly MySqlConnection _connection;

        public AdministratorRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Administrator>> GetAllAsync()
        {
            var administratores = new List<Administrator>();
            const string query = "SELECT id, name, lastname, identification, username, password FROM administrator";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                administratores.Add(new Administrator
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Name = reader["name"].ToString() ?? string.Empty,
                    LastName = reader["lastname"].ToString() ?? string.Empty,
                    Identification = reader["identification"].ToString() ?? string.Empty,
                    Username = reader["username"].ToString() ?? string.Empty,
                    Password = reader["password"].ToString() ?? string.Empty
                });
            }

            return administratores;
        }

        public async Task<Administrator?> GetByIdAsync(object id)
        {
            const string query = "SELECT id, name, lastname, identification, username, password FROM administrator WHERE id = @Id";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Administrator
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Name = reader["name"].ToString() ?? string.Empty,
                    LastName = reader["lastname"].ToString() ?? string.Empty,
                    Identification = reader["identification"].ToString() ?? string.Empty,
                    Username = reader["username"].ToString() ?? string.Empty,
                    Password = reader["password"].ToString() ?? string.Empty
                };
            }

            return null;
        }

        public async Task<bool> InsertAsync(Administrator administrator)
        {
            if (administrator == null)
                throw new ArgumentNullException(nameof(administrator));

            const string query = "INSERT INTO administrator (name, lastname, identification, username, password) VALUES (@Name, @LastName, @Identification, @Username, @Password)";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Name", administrator.Name);
                command.Parameters.AddWithValue("@LastName", administrator.LastName);
                command.Parameters.AddWithValue("@Identification", administrator.Identification);
                command.Parameters.AddWithValue("@Username", administrator.Username);
                command.Parameters.AddWithValue("@Password", administrator.Password);

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

        public async Task<bool> UpdateAsync(Administrator administrator)
        {
            if (administrator == null)
                throw new ArgumentNullException(nameof(administrator));

            const string query = "UPDATE administrator SET name = @Name, lastname = @LastName, identification = @Identification, username = @Username, password = @Password WHERE id = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Name", administrator.Name);
                command.Parameters.AddWithValue("@LastName", administrator.LastName);
                command.Parameters.AddWithValue("@Identification", administrator.Identification);
                command.Parameters.AddWithValue("@Username", administrator.Username);
                command.Parameters.AddWithValue("@Password", administrator.Password);
                command.Parameters.AddWithValue("@Id", administrator.Id);

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
            const string query = "DELETE FROM administrator WHERE id = @Id";
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

        public async Task<Administrator?> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty", nameof(username));

            const string query = "SELECT id, username, password, name, lastname FROM administrator WHERE username = @Username";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Username", username);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Administrator
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Username = reader["username"].ToString() ?? string.Empty,
                    Password = reader["password"].ToString() ?? string.Empty,
                    Name = reader["name"].ToString() ?? string.Empty,
                    LastName = reader["lastname"].ToString() ?? string.Empty
                };
            }

            return null;
        }
    }
} 