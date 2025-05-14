using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using MySql.Data.MySqlClient;

namespace CampusLove.Infrastructure.Repositories
{
    public class ReactionRepository : IGenericRepository<Reaction>, IReactionRepository
    {
       private readonly MySqlConnection _connection;

        public ReactionRepository(MySqlConnection connection)
        {
            _connection = connection;
        } 

        public async Task<IEnumerable<Reaction>> GetAllAsync()
        {
            var reactions = new List<Reaction>();
            const string query = "SELECT id, user_id, profile_id, reaction_type, reactionDate FROM reaction";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                reactions.Add(new Reaction
                {
                    Id = Convert.ToInt32(reader["id"]),
                    UserId = Convert.ToInt32(reader["user_id"]),
                    ProfileId = Convert.ToInt32(reader["profile_id"]),
                    ReactionType = (ReactionType)Convert.ToInt32(reader["reaction_type"]),
                    reactionDate = Convert.ToDateTime(reader["birthdate"])
                });
            }

            return reactions;
        }

        public async Task<Reaction?> GetByIdAsync(object id)
        {
            const string query = "SELECT id, user_id, profile_id, reactionDate, reaction_type FROM reaction WHERE id = @Id";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Reaction
                {
                    Id = Convert.ToInt32(reader["id"]),
                    UserId = Convert.ToInt32(reader["user_id"]),
                    ProfileId = Convert.ToInt32(reader["profile_id"]),
                    ReactionType = (ReactionType)Convert.ToInt32(reader["reaction_type"]),
                    reactionDate = Convert.ToDateTime(reader["birthdate"])
                };
            }

            return null;
        }

        public async Task<bool> InsertAsync(Reaction reaction)
        {
            if (reaction == null)
                throw new ArgumentNullException(nameof(reaction));

            const string query = "INSERT INTO reaction (user_id, reactionDate, profile_id, reaction_type) VALUES (@UserId, @reactionDate, @ProfileId, @ReactionType)";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@UserId", reaction.UserId);
                command.Parameters.AddWithValue("@reactionDate", reaction.reactionDate);
                command.Parameters.AddWithValue("@ProfileId", reaction.ProfileId);
                command.Parameters.AddWithValue("@ReactionType", reaction.ReactionType);

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

        public async Task<bool> UpdateAsync(Reaction reaction)
        {
            if (reaction == null)
                throw new ArgumentNullException(nameof(reaction));

            const string query = "UPDATE reaction SET user_id = @UserId, reactionDate = @reactionDate, profile_id = @ProfileId, reaction_type = @ReactionType WHERE id = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

             try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@UserId", reaction.UserId);
                command.Parameters.AddWithValue("@reactionDate", reaction.reactionDate);
                command.Parameters.AddWithValue("@ProfileId", reaction.ProfileId);
                command.Parameters.AddWithValue("@ReactionType", reaction.ReactionType);

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
            const string query = "DELETE FROM reaction WHERE id = @Id";
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