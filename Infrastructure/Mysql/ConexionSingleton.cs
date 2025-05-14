using System;
using MySql.Data.MySqlClient;

namespace CampusLove.Infrastructure.Mysql;

public class ConexionSingleton
{
    private static ConexionSingleton? _instancia;
    private readonly string _connectionString;
    private MySqlConnection? _conexion;

    private ConexionSingleton(string connectionString)
    {
        _connectionString = connectionString;
    }

    public static ConexionSingleton Instancia(string connectionString)
    {
        _instancia ??= new ConexionSingleton(connectionString);
        return _instancia;
    }

    public MySqlConnection ObtenerConexion()
    {
        _conexion ??= new MySqlConnection(_connectionString);

        if (_conexion.State != System.Data.ConnectionState.Open)
            _conexion.Open();

        return _conexion;
    }
}
