using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using MySql.Data.MySqlClient;

namespace CampusLove.Infrastructure.Repositories
{
    public class DailyLikesRepository : IGenericRepository<DailyLikes>, IDailyLikesRepository
    {
        private readonly MySqlConnection _connection;

        public DailyLikesRepository(MySqlConnection connection)
        {
            _connection = connection;
        }
        public async Task<IEnumerable<DailyLikes>> GetAllAsync()
        {
            var dailyLikes = new List<DailyLikes>();
            const string query = "SELECT id, date, profile_id, number_likes, status FROM daily_likes";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                dailyLikes.Add(new DailyLikes
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Date = Convert.ToDateTime(reader["date"]),
                    ProfileId = Convert.ToInt32(reader["profile_id"]),
                    Number_Likes = Convert.ToInt32(reader["number_likes"]),
                    Status = Convert.ToBoolean(reader["status"])
                });
            }

            return dailyLikes;
        }

        public async Task<DailyLikes?> GetByIdAsync(object id)
        {
            const string query = "SELECT id, date, profile_id, number_likes, status FROM dailty_likes WHERE id = @Id";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new DailyLikes
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Date = Convert.ToDateTime(reader["date"]),
                    ProfileId = Convert.ToInt32(reader["profile_id"]),
                    Number_Likes = Convert.ToInt32(reader["number_likes"]),
                    Status = Convert.ToBoolean(reader["status"])
                };
            }

            return null;
        }

        public async Task<bool> InsertAsync(DailyLikes dailyLikes)
        {
            if (dailyLikes == null)
                throw new ArgumentNullException(nameof(dailyLikes));

            const string query = "INSERT INTO daily_likes (date, profile_id, number_likes, status) VALUES (@Date, @ProfileId, @Number_Likes, @Status)";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Date", dailyLikes.Date);
                command.Parameters.AddWithValue("@ProfileId", dailyLikes.ProfileId);
                command.Parameters.AddWithValue("@Number_Likes", dailyLikes.Number_Likes);
                command.Parameters.AddWithValue("@Status", dailyLikes.Status);

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

        public async Task<bool> UpdateAsync(DailyLikes dailyLikes)
        {
            if (dailyLikes == null)
                throw new ArgumentNullException(nameof(dailyLikes));

            const string query = "UPDATE daily_likes SET date = @Date, profile_id = @ProfileId, number_likes = @Number_Likes, status = @Status WHERE id = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

             try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Date", dailyLikes.Date);
                command.Parameters.AddWithValue("@ProfileId", dailyLikes.ProfileId);
                command.Parameters.AddWithValue("@Number_Likes", dailyLikes.Number_Likes);
                command.Parameters.AddWithValue("@Status", dailyLikes.Status);

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
            const string query = "DELETE FROM daily_likes WHERE id = @Id";
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