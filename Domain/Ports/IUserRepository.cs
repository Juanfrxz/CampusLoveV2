using System;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;

namespace CampusLove.Domain.Ports;

public interface IUserRepository : IGenericRepository<User> 
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByProfileIdAsync(int profileId);
}