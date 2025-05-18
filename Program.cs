using System;
using System.Threading.Tasks;
using CampusLove.Application.UI;
using CampusLove.Infrastructure.Configuration;
using CampusLove.Infrastructure.Repositories;
using MySql.Data.MySqlClient;

namespace CampusLove
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            try
            {
                var connection = DatabaseConfig.GetConnection();

                var mainMenu = new MainMenu();
                await mainMenu.ShowMenu();
            }
            catch (Exception ex)
            {
               Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                Console.ResetColor(); 
            }
            finally
            {
                DatabaseConfig.CloseConnection();
            }
        }
    }
}