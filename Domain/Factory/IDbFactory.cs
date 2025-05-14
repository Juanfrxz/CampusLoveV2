using System;
using CampusLove.Domain.Ports;

namespace CampusLove.Domain.Factory;

public interface IDbFactory
{
    IUserRepository CreateUserRepository();
    IProfileRepository CreateProfileRepository();
    IReactionRepository CreateReactionRepository();
    IDailyLikesRepository CreateDailyLikesRepository();
    IUserMatchRepository CreateUserMatchRepository();
}