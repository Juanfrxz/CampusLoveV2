using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using MySql.Data.MySqlClient;

namespace CampusLove.Infrastructure.Repositories
{
    public class InterestRepository : IInterestRepository
    {
        private readonly MySqlConnection _connection;

        public InterestRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Interest>> GetAllAsync()
        {
            var interestList = new List<Interest>();
            const string query = "SELECT id, description FROM interest";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                interestList.Add(new Interest
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Description = reader["description"].ToString() ?? string.Empty
                });
            }

            return interestList;
        }

        public async Task<Interest?> GetByIdAsync(object id)
        {
            const string query = "SELECT id, description FROM interest WHERE id = @Id";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Interest
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Description = reader["description"].ToString() ?? string.Empty
                };
            }

            return null;
        }

        public async Task<bool> InsertAsync(Interest interest)
        {
            if (interest == null)
                throw new ArgumentNullException(nameof(interest));

            const string query = "INSERT INTO interest (description) VALUES (@Description)";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Description", interest.Description);

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

        public async Task<bool> UpdateAsync(Interest interest)
        {
            if (interest == null)
                throw new ArgumentNullException(nameof(interest));

            const string query = "UPDATE interest SET description = @Description WHERE id = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Description", interest.Description);
                command.Parameters.AddWithValue("@Id", interest.Id);

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
            const string query = "DELETE FROM interest WHERE id = @Id";
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
    }
} 