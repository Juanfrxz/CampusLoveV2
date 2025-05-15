using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using MySql.Data.MySqlClient;
using Dapper;

namespace CampusLove.Infrastructure.Repositories
{
    public class ProfileRepository : IGenericRepository<Profile>, IProfileRepository
    {
        private readonly MySqlConnection _connection;

        public ProfileRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Profile>> GetAllAsync()
        {
            var profiles = new List<Profile>();
            using var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM profile";

            try
            {
                if (_connection.State != System.Data.ConnectionState.Open)
                {
                    await _connection.OpenAsync();
                }
                
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var profile = new Profile
                    {
                        Id = reader["id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id"]),
                        Name = reader["name"]?.ToString() ?? string.Empty,
                        LastName = reader["lastname"]?.ToString() ?? string.Empty,
                        Identification = reader["identification"]?.ToString() ?? string.Empty,
                        Slogan = reader["slogan"]?.ToString() ?? string.Empty,
                        GenderId = reader["gender_id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["gender_id"]),
                        ProfessionId = reader["profession_id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["profession_id"]),
                        StatusId = reader["status_id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["status_id"]),
                        TotalLikes = reader["total_likes"] == DBNull.Value ? 0 : Convert.ToInt32(reader["total_likes"]),
                        createDate = reader["createDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["createDate"])
                    };
                    profiles.Add(profile);
                }
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    await _connection.CloseAsync();
                }
            }

            return profiles;
        }

        public async Task<Profile?> GetByIdAsync(object id)
        {
            Profile? profile = null;

            using var command = new MySqlCommand(@"
                SELECT p.* 
                FROM profile p 
                WHERE p.id = @Id", _connection);

            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                profile = new Profile
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Name = reader["name"].ToString() ?? string.Empty,
                    LastName = reader["lastname"].ToString() ?? string.Empty,
                    Identification = reader["identification"].ToString() ?? string.Empty,
                    GenderId = Convert.ToInt32(reader["gender_id"]),
                    Slogan = reader["slogan"].ToString() ?? string.Empty,
                    StatusId = Convert.ToInt32(reader["status_id"]),
                    createDate = Convert.ToDateTime(reader["createDate"]),
                    ProfessionId = Convert.ToInt32(reader["profession_id"]),
                    TotalLikes = Convert.ToInt32(reader["total_likes"])
                };
            }

            return profile;
        }

        public async Task<bool> InsertAsync(Profile profile)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            const string query = "INSERT INTO profile (name, lastname, identification, gender_id, slogan, status_id, createDate, profession_id, total_likes) VALUES (@Name, @LastName, @Identification, @GenderId, @Slogan, @StatusId, @createDate, @ProfessionId, @TotalLikes)";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Name", profile.Name);
                command.Parameters.AddWithValue("@LastName", profile.LastName);
                command.Parameters.AddWithValue("@Identification", profile.Identification);
                command.Parameters.AddWithValue("@GenderId", profile.GenderId);
                command.Parameters.AddWithValue("@Slogan", profile.Slogan);
                command.Parameters.AddWithValue("@StatusId",
                profile.StatusId);
                command.Parameters.AddWithValue("@createDate", profile.createDate);
                command.Parameters.AddWithValue("@ProfessionId", profile.ProfessionId);
                command.Parameters.AddWithValue("@TotalLikes", profile.TotalLikes);

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

        public async Task<bool> UpdateAsync(Profile profile)
        {
            const string query = "UPDATE profile SET name = @Name, lastname = @LastName, gender_id = @GenderId, slogan = @Slogan, status_id = @StatusId, profession_id = @ProfessionId WHERE id = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Name", profile.Name);
                command.Parameters.AddWithValue("@LastName", profile.LastName);
                command.Parameters.AddWithValue("@GenderId", profile.GenderId);
                command.Parameters.AddWithValue("@Slogan", profile.Slogan);
                command.Parameters.AddWithValue("@StatusId", profile.StatusId);
                command.Parameters.AddWithValue("@ProfessionId", profile.ProfessionId);
                command.Parameters.AddWithValue("@Id", profile.Id);

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
            const string query = "DELETE FROM profile WHERE id = @Id";
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

        public async Task<Profile?> GetLastProfileAsync()
        {
            const string query = "SELECT id, name, lastname, identification, gender_id, slogan, status_id, createDate, profession_id, total_likes FROM profile ORDER BY id DESC LIMIT 1";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                return new Profile
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Name = reader["name"].ToString() ?? string.Empty,
                    LastName = reader["lastname"].ToString() ?? string.Empty,
                    Identification = reader["identification"].ToString() ?? string.Empty,
                    GenderId = Convert.ToInt32(reader["gender_id"]),
                    Slogan = reader["slogan"].ToString() ?? string.Empty,
                    StatusId = Convert.ToInt32(reader["status_id"]),
                    createDate = Convert.ToDateTime(reader["createDate"]),
                    ProfessionId = Convert.ToInt32(reader["profession_id"]),
                    TotalLikes = Convert.ToInt32(reader["total_likes"])
                };
            }

            return null;
        }

        public async Task<Profile?> GetByUserIdAsync(int userId)
        {
            const string sql = @"
                SELECT p.* FROM profile p
                INNER JOIN `user` u ON u.profile_id = p.id
                WHERE u.id = @UserId";

            return await _connection.QueryFirstOrDefaultAsync<Profile>(sql, new { UserId = userId });
        }
    }
}