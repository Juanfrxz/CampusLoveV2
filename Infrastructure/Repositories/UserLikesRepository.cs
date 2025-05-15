using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MySql.Data.MySqlClient;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using Dapper;

namespace CampusLove.Infrastructure.Repositories;

public class UserLikesRepository : IUserLikesRepository
{
    private readonly MySqlConnection _connection;

    public UserLikesRepository(MySqlConnection connection)
    {
        _connection = connection;
        // Configurar mapeo de columnas
        SqlMapper.SetTypeMap(typeof(UserLikes), new CustomPropertyTypeMap(
            typeof(UserLikes),
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

    public async Task<UserLikes?> GetByIdAsync(int id)
    {
        await EnsureConnectionOpen();
        const string sql = @"
            SELECT id as Id, 
                   user_id as UserId, 
                   liked_profile_id as LikedProfileId, 
                   like_date as LikeDate, 
                   is_match as IsMatch 
            FROM userlikes 
            WHERE id = @Id";

        return await _connection.QueryFirstOrDefaultAsync<UserLikes>(sql, new { Id = id });
    }

    public async Task<IEnumerable<UserLikes>> GetAllAsync()
    {
        await EnsureConnectionOpen();
        const string sql = @"
            SELECT id as Id, 
                   user_id as UserId, 
                   liked_profile_id as LikedProfileId, 
                   like_date as LikeDate, 
                   is_match as IsMatch 
            FROM userlikes";
        return await _connection.QueryAsync<UserLikes>(sql);
    }

    public async Task<IEnumerable<UserLikes>> GetLikesByUserIdAsync(int userId)
    {
        await EnsureConnectionOpen();
        const string sql = @"
            SELECT id as Id, 
                   user_id as UserId, 
                   liked_profile_id as LikedProfileId, 
                   like_date as LikeDate, 
                   is_match as IsMatch 
            FROM userlikes 
            WHERE user_id = @UserId";

        return await _connection.QueryAsync<UserLikes>(sql, new { UserId = userId });
    }

    public async Task<IEnumerable<UserLikes>> GetLikesByProfileIdAsync(int profileId)
    {
        await EnsureConnectionOpen();
        const string sql = @"
            SELECT id as Id, 
                   user_id as UserId, 
                   liked_profile_id as LikedProfileId, 
                   like_date as LikeDate, 
                   is_match as IsMatch 
            FROM userlikes 
            WHERE liked_profile_id = @ProfileId";

        return await _connection.QueryAsync<UserLikes>(sql, new { ProfileId = profileId });
    }

    public async Task<int> GetLikeCountByUserIdAsync(int userId)
    {
        await EnsureConnectionOpen();
        const string sql = @"
            SELECT COUNT(*) FROM userlikes 
            WHERE user_id = @UserId";

        return await _connection.ExecuteScalarAsync<int>(sql, new { UserId = userId });
    }

    public async Task<bool> HasUserLikedProfileAsync(int userId, int profileId)
    {
        await EnsureConnectionOpen();
        const string sql = @"
            SELECT COUNT(*) FROM userlikes 
            WHERE user_id = @UserId AND liked_profile_id = @ProfileId";

        var count = await _connection.ExecuteScalarAsync<int>(sql, new { UserId = userId, ProfileId = profileId });
        return count > 0;
    }

    public async Task<UserLikes> CreateLikeAsync(UserLikes like)
    {
        await EnsureConnectionOpen();
        const string sql = @"
            INSERT INTO userlikes (user_id, liked_profile_id, like_date, is_match)
            VALUES (@UserId, @LikedProfileId, @LikeDate, @IsMatch);
            SELECT LAST_INSERT_ID();";

        like.LikeDate = DateTime.UtcNow;
        like.Id = await _connection.ExecuteScalarAsync<int>(sql, like);
        return like;
    }

    public async Task<bool> DeleteLikeAsync(int id)
    {
        await EnsureConnectionOpen();
        const string sql = "DELETE FROM userlikes WHERE id = @Id";
        var rowsAffected = await _connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateMatchStatusAsync(int id, bool isMatch)
    {
        await EnsureConnectionOpen();
        const string sql = @"
            UPDATE userlikes 
            SET is_match = @IsMatch 
            WHERE id = @Id";

        var rowsAffected = await _connection.ExecuteAsync(sql, new { Id = id, IsMatch = isMatch });
        return rowsAffected > 0;
    }
} 