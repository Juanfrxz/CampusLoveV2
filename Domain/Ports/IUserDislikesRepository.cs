using System.Collections.Generic;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;

namespace CampusLove.Domain.Ports;

public interface IUserDislikesRepository
{
    Task<UserDislikes?> GetByIdAsync(int id);
    Task<IEnumerable<UserDislikes>> GetAllAsync();
    Task<IEnumerable<UserDislikes>> GetDislikesByUserIdAsync(int userId);
    Task<IEnumerable<UserDislikes>> GetDislikesByProfileIdAsync(int profileId);
    Task<int> GetDislikeCountByUserIdAsync(int userId);
    Task<bool> HasUserDislikedProfileAsync(int userId, int profileId);
    Task<UserDislikes> CreateDislikeAsync(UserDislikes dislike);
    Task<bool> DeleteDislikeAsync(int id);
}