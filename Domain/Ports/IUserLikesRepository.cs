using System.Collections.Generic;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;

namespace CampusLove.Domain.Ports;

public interface IUserLikesRepository
{
    Task<UserLikes?> GetByIdAsync(int id);
    Task<IEnumerable<UserLikes>> GetAllAsync();
    Task<IEnumerable<UserLikes>> GetLikesByUserIdAsync(int userId);
    Task<IEnumerable<UserLikes>> GetLikesByProfileIdAsync(int profileId);
    Task<int> GetLikeCountByUserIdAsync(int userId);
    Task<bool> HasUserLikedProfileAsync(int userId, int profileId);
    Task<UserLikes> CreateLikeAsync(UserLikes like);
    Task<bool> DeleteLikeAsync(int id);
    Task<bool> UpdateMatchStatusAsync(int id, bool isMatch);
} 