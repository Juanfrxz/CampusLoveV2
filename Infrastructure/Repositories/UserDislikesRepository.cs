using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MySql.Data.MySqlClient;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using Dapper;

namespace CampusLove.Infrastructure.Repositories;

public class UserDislikesRepository : IUserDislikesRepository
{
    private readonly MySqlConnection _connection;

    public UserDislikesRepository(MySqlConnection connection)
    {
        _connection = connection;
        // Configurar mapeo de columnas
        SqlMapper.SetTypeMap(typeof(UserDislikes), new CustomPropertyTypeMap(
            typeof(UserDislikes),
            (type, columnName) =>
                type.GetProperties().FirstOrDefault(prop =>
                    prop.GetCustomAttributes(false)
                        .OfType<System.ComponentModel.DataAnnotations.Schema.ColumnAttribute>()
                        .Any(attr => attr.Name == columnName) ||
                    prop.Name == columnName
                )!
        ));
    }

    private async Task EnsureConnectionOpen()
    {
        if (_connection.State != System.Data.ConnectionState.Open)
        {
            await _connection.OpenAsync();
        }
    }

    public async Task<UserDislikes?> GetByIdAsync(int id)
    {
        await EnsureConnectionOpen();
        const string sql = @"
            SELECT id as Id, 
                user_id as UserId, 
                disliked_profile_id as DislikedProfileId, 
                dislike_date as DislikeDate
            FROM userdislike 
            WHERE id = @Id";

        return await _connection.QueryFirstOrDefaultAsync<UserDislikes>(sql, new { Id = id });
    }

    public async Task<IEnumerable<UserDislikes>> GetAllAsync()
    {
        await EnsureConnectionOpen();
        const string sql = @"
            SELECT id as Id, 
                user_id as UserId, 
                disliked_profile_id as dislikedProfileId, 
                dislike_date as dislikeDate 
            FROM userdislike";
        return await _connection.QueryAsync<UserDislikes>(sql);
    }

    public async Task<IEnumerable<UserDislikes>> GetDislikesByUserIdAsync(int userId)
    {
        await EnsureConnectionOpen();
        const string sql = @"
            SELECT id as Id, 
                user_id as UserId, 
                disliked_profile_id as dislikedProfileId, 
                dislike_date as dislikeDate
            FROM userdislike 
            WHERE user_id = @UserId";

        return await _connection.QueryAsync<UserDislikes>(sql, new { UserId = userId });
    }

    public async Task<IEnumerable<UserDislikes>> GetDislikesByProfileIdAsync(int profileId)
    {
        await EnsureConnectionOpen();
        const string sql = @"
            SELECT id as Id, 
                user_id as UserId, 
                disliked_profile_id as dislikedProfileId, 
                dislike_date as dislikeDate
            FROM userdislike
            WHERE disliked_profile_id = @ProfileId";

        return await _connection.QueryAsync<UserDislikes>(sql, new { ProfileId = profileId });
    }

    public async Task<int> GetDislikeCountByUserIdAsync(int userId)
    {
        await EnsureConnectionOpen();
        const string sql = @"
            SELECT COUNT(*) FROM userdislike
            WHERE user_id = @UserId";

        return await _connection.ExecuteScalarAsync<int>(sql, new { UserId = userId });
    }

    public async Task<bool> HasUserDislikedProfileAsync(int userId, int profileId)
    {
        await EnsureConnectionOpen();
        const string sql = @"
            SELECT COUNT(*) FROM userdislike
            WHERE user_id = @UserId AND disliked_profile_id = @ProfileId";

        var count = await _connection.ExecuteScalarAsync<int>(sql, new { UserId = userId, ProfileId = profileId });
        return count > 0;
    }

    public async Task<UserDislikes> CreateDislikeAsync(UserDislikes dislike)
    {
        await EnsureConnectionOpen();
        const string sql = @"
            INSERT INTO userdislike (user_id, disliked_profile_id, dislike_date)
            VALUES (@UserId, @DislikedProfileId, @DislikeDate);
            SELECT LAST_INSERT_ID();";

        dislike.DislikeDate = DateTime.UtcNow;
        dislike.Id = await _connection.ExecuteScalarAsync<int>(sql, dislike);
        return dislike;
    }

    public async Task<bool> DeleteDislikeAsync(int id)
    {
        await EnsureConnectionOpen();
        const string sql = "DELETE FROM userdislike WHERE id = @Id";
        var rowsAffected = await _connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }
}