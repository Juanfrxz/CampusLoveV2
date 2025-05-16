using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using MySql.Data.MySqlClient;

namespace CampusLove.Infrastructure.Repositories
{
    public class InterestProfileRepository : IGenericRepository<InterestProfile>
    {
        private readonly MySqlConnection _connection;

        public InterestProfileRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<InterestProfile>> GetAllAsync()
        {
            var interestProfileList = new List<InterestProfile>();
            const string query = "SELECT profile_id, interest_id FROM interestProfile";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                interestProfileList.Add(new InterestProfile
                {
                    ProfileId = Convert.ToInt32(reader["profile_id"]),
                    InterestId = Convert.ToInt32(reader["interest_id"])
                });
            }

            return interestProfileList;
        }

        public async Task<InterestProfile?> GetByIdAsync(object profile_id)
        {
            const string query = "SELECT profile_id, interest_id FROM interestProfile WHERE profile_id = @ProfileId";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@ProfileId", profile_id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new InterestProfile
                {
                    ProfileId = Convert.ToInt32(reader["profile_id"]),
                    InterestId = Convert.ToInt32(reader["interest_id"])
                };
            }

            return null;
        }

        public async Task<bool> InsertAsync(InterestProfile interestProfile)
        {
            if (interestProfile == null)
                throw new ArgumentNullException(nameof(interestProfile));

            const string query = "INSERT INTO interestProfile (profile_id, interest_id) VALUES (@ProfileId, @InterestId)";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@ProfileId", interestProfile.ProfileId);
                command.Parameters.AddWithValue("@InterestId", interestProfile.InterestId);

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

        public async Task<bool> UpdateAsync(InterestProfile interestProfile)
        {
            if (interestProfile == null)
                throw new ArgumentNullException(nameof(interestProfile));

            const string query = "UPDATE interestProfile SET profile_id = @ProfileId, interest_id = InterestId WHERE profile_id = @ProfileId";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@ProfileId", interestProfile.ProfileId);
                command.Parameters.AddWithValue("@InterestId", interestProfile.InterestId);

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

        public async Task<bool> DeleteAsync(object profile_id)
        {
            const string query = "DELETE FROM interestProfile WHERE profile_id = @ProfileId";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@ProfileId", profile_id);

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