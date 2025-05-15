using System;
using System.Threading.Tasks;
using CampusLove.Infrastructure.Repositories;
using CampusLove.Infrastructure.Configuration;
using MySql.Data.MySqlClient;
using CampusLove.Application.UI;
using CampusLove.Domain.Entities;

namespace CampusLove.Application.UI
{
    public class MainMenu
    {
        private readonly SignUpMenu _signupMenu;
        private readonly LogInMenu _loginMenu;

        public MainMenu()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "  CampusLove ... Where is Love 💞   ";

            var connection = DatabaseConfig.GetConnection();
            _signupMenu = new SignUpMenu(connection);
            _loginMenu = new LogInMenu(connection);
        }

        public void ShowMenu()
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("CampusLove ... Where is Love 💞");

                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("  ╔════════════════════════════════════════════╗");
                Console.WriteLine("  ║                📋 MAIN MENU                ║");
                Console.WriteLine("  ╠════════════════════════════════════════════╣");
                Console.WriteLine("  ║     1️⃣  Sign Up                   📰        ║");
                Console.WriteLine("  ║     2️⃣  Log In                    ☑️         ║");
                Console.WriteLine("  ║     0️⃣  Exit                      ❌        ║");
                Console.WriteLine("  ╚════════════════════════════════════════════╝");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Gray;
                string option = ReadText("\n✨ Select an option: ");

                switch (option)
                {
                    case "1":
                        _signupMenu.RegisterUser().Wait();
                        break;
                    case "2":
                        _loginMenu.ValidateUser().Wait();
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        ShowMessage("⚠️ Invalid option. Please try again.", ConsoleColor.Red);
                        Console.ReadKey();
                        break;  
                }
            }

            ShowMessage("\n👋 Thank you for using the application! Have a great day! 🌟", ConsoleColor.Blue);
        }

        public static void ShowMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"\n{message}");
            Console.ResetColor();
        }

        public static string ReadText(string prompt)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(prompt);
            Console.ResetColor();
            return Console.ReadLine() ?? "";
        }

        public static int ReadInteger(string prompt)
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(prompt);
                Console.ResetColor();
                if (int.TryParse(Console.ReadLine(), out int value) && value >= 0)
                {
                    return value;
                }

                ShowMessage("❌ Error: Please enter a positive integer.", ConsoleColor.Red);
            }
        }

        public static DateTime ReadDate(string prompt)
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(prompt);
                Console.ResetColor();
                if (DateTime.TryParse(Console.ReadLine(), out DateTime date))
                {
                    return date;
                }

                ShowMessage("❌ Error: Please enter a valid date (DD/MM/YYYY).", ConsoleColor.Red);
            }
        }

    }
}