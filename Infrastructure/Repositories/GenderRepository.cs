using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using MySql.Data.MySqlClient;

namespace CampusLove.Infrastructure.Repositories
{
    public class GenderRepository : IGenericRepository<Gender>
    {
        private readonly MySqlConnection _connection;

        public GenderRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Gender>> GetAllAsync()
        {
            var genderList = new List<Gender>();
            const string query = "SELECT id, description FROM gender";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                genderList.Add(new Gender
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Description = reader["description"].ToString() ?? string.Empty
                });
            }

            return genderList;
        }

        public async Task<Gender?> GetByIdAsync(object id)
        {
            const string query = "SELECT id, description FROM gender WHERE id = @Id";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Gender
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Description = reader["description"].ToString() ?? string.Empty
                };
            }

            return null;
        }

        public async Task<bool> InsertAsync(Gender gender)
        {
            if (gender == null)
                throw new ArgumentNullException(nameof(gender));

            const string query = "INSERT INTO gender (description) VALUES (@Description)";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Description", gender.Description);

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

        public async Task<bool> UpdateAsync(Gender gender)
        {
            if (gender == null)
                throw new ArgumentNullException(nameof(gender));

            const string query = "UPDATE gender SET description = @Description WHERE id = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Description", gender.Description);
                command.Parameters.AddWithValue("@Id", gender.Id);

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
            const string query = "DELETE FROM gender WHERE id = @Id";
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