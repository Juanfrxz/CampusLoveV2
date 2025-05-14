using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using MySql.Data.MySqlClient;

namespace CampusLove.Infrastructure.Repositories
{
    public class UserMatchRepository : IGenericRepository<UserMatch>, IUserMatchRepository
    {
        private readonly MySqlConnection _connection;

        public UserMatchRepository(MySqlConnection connection)
        {
            _connection = connection;
        }
        public async Task<IEnumerable<UserMatch>> GetAllAsync()
        {
            var userMatches = new List<UserMatch>();
            const string query = "SELECT id, user1_id, user2_id, matchDate FROM user_match";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                userMatches.Add(new UserMatch
                {
                    Id = Convert.ToInt32(reader["id"]),
                    User1_id = Convert.ToInt32(reader["user1_id"]),
                    User2_id = Convert.ToInt32(reader["user2_id"]),
                    MatchDate = Convert.ToDateTime(reader["matchDate"])
                });
            }

            return userMatches;
        }

        public async Task<UserMatch?> GetByIdAsync(object id)
        {
            const string query = "SELECT id, user1_id, user2_id, matchDate FROM user_match WHERE id = @Id";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new UserMatch
                {
                    Id = Convert.ToInt32(reader["id"]),
                    User1_id = Convert.ToInt32(reader["user1_id"]),
                    User2_id = Convert.ToInt32(reader["user2_id"]),
                    MatchDate = Convert.ToDateTime(reader["matchDate"])
                };
            }

            return null;
        }

        public async Task<bool> InsertAsync(UserMatch userMatch)
        {
            if (userMatch == null)
                throw new ArgumentNullException(nameof(userMatch));

            const string query = "INSERT INTO user_match (user1_id, user2_id, matchDate) VALUES (@User1_id, @User2_id, @MatchDate)";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@User1_id", userMatch.User1_id);
                command.Parameters.AddWithValue("@User2_id", userMatch.User2_id);
                command.Parameters.AddWithValue("@MatchDate", userMatch.MatchDate);

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

        public async Task<bool> UpdateAsync(UserMatch userMatch)
        {
            if (userMatch == null)
                throw new ArgumentNullException(nameof(userMatch));

            const string query = "UPDATE user_match SET user1_id = @User1_id, user2_id = @User2_id, matchDate = @MatchDate WHERE id = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

             try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@User1_id", userMatch.User1_id);
                command.Parameters.AddWithValue("@User2_id", userMatch.User2_id);
                command.Parameters.AddWithValue("@MatchDate", userMatch.MatchDate);

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
            const string query = "DELETE FROM user_match WHERE id = @Id";
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