using CampusLove.Domain.Entities;
using CampusLove.Domain.Factory;
using CampusLove.Domain.Ports;
using CampusLove.Infrastructure.Repositories;
using MySql.Data.MySqlClient;

namespace CampusLove.Infrastructure.Mysql;

public class MySqlDbFactory : IDbFactory
{
    private readonly string _connectionString;

    public MySqlDbFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IUserRepository CreateUserRepository()
    {
        var connection = ConexionSingleton.Instancia(_connectionString).ObtenerConexion();
        return new UserRepository(connection);
    }

    public IProfileRepository CreateProfileRepository()
    {
        var connection = ConexionSingleton.Instancia(_connectionString).ObtenerConexion();
        return new ProfileRepository(connection);
    }

    public IReactionRepository CreateReactionRepository()
    {
        var connection = ConexionSingleton.Instancia(_connectionString).ObtenerConexion();
        return new ReactionRepository(connection);
    }

    public IDailyLikesRepository CreateDailyLikesRepository()
    {
        var connection = ConexionSingleton.Instancia(_connectionString).ObtenerConexion();
        return new DailyLikesRepository(connection);
    }

    public IUserMatchRepository CreateUserMatchRepository()
    {
        var connection = ConexionSingleton.Instancia(_connectionString).ObtenerConexion();
        return new UserMatchRepository(connection);
    }

    
}
