using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using MySql.Data.MySqlClient;

namespace CampusLove.Infrastructure.Repositories
{
    public class ProfessionRepository : IGenericRepository<Profession>
    {
        private readonly MySqlConnection _connection;

        public ProfessionRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Profession>> GetAllAsync()
        {
            var professionList = new List<Profession>();
            const string query = "SELECT id, description FROM profession";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                professionList.Add(new Profession
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Description = reader["description"].ToString() ?? string.Empty
                });
            }

            return professionList;
        }

        public async Task<Profession?> GetByIdAsync(object id)
        {
            const string query = "SELECT id, description FROM profession WHERE id = @Id";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Profession
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Description = reader["description"].ToString() ?? string.Empty
                };
            }

            return null;
        }

        public async Task<bool> InsertAsync(Profession profession)
        {
            if (profession == null)
                throw new ArgumentNullException(nameof(profession));

            const string query = "INSERT INTO profession (description) VALUES (@Description)";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Description", profession.Description);

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

        public async Task<bool> UpdateAsync(Profession profession)
        {
            if (profession == null)
                throw new ArgumentNullException(nameof(profession));

            const string query = "UPDATE profession SET description = @Description WHERE id = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Description", profession.Description);
                command.Parameters.AddWithValue("@Id", profession.Id);

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
            const string query = "DELETE FROM profession WHERE id = @Id";
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