using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using CampusLove.Infrastructure.Repositories;
using CampusLove.Infrastructure.Configuration;
using MySql.Data.MySqlClient;

namespace CampusLove.Application.UI
{
    public class LogInMenu
    {
        private readonly UserRepository _userRepository;
        private readonly SettingsMenu _settingsMenu;
        private readonly ProfileRepository _profileRepository;
        private readonly ReactionRepository _reactionRepository;
        private readonly UserMatchRepository _usermatchRepository;

        public LogInMenu(MySqlConnection connection)
        {
            _userRepository = new UserRepository(connection);
            _profileRepository = new ProfileRepository(connection);
            _reactionRepository = new ReactionRepository(connection);
            _usermatchRepository = new UserMatchRepository(connection);
            _settingsMenu = new SettingsMenu(connection);
        }

        public async Task ValidateUser()
        {
            Console.Clear();
            Console.WriteLine("👥 LOG IN");
            Console.WriteLine("Welcome to CampusLove");
            Console.WriteLine("------------------");

            try
            {
                string username = MainMenu.ReadText("\nUsername: ").Trim();
                if (string.IsNullOrWhiteSpace(username))
                {
                    MainMenu.ShowMessage("❌ Username cannot be empty.", ConsoleColor.Red);
                    return;
                }

                string password = MainMenu.ReadText("Password: ").Trim();
                if (string.IsNullOrWhiteSpace(password))
                {
                    MainMenu.ShowMessage("❌ Password cannot be empty.", ConsoleColor.Red);
                }

                var user = await _userRepository.GetByUsernameAsync(username);
                if (user == null)
                {
                    MainMenu.ShowMessage("❌ User not found.", ConsoleColor.Red);
                }

                if (user.Password != password)
                {
                    MainMenu.ShowMessage("❌ Incorrect password.", ConsoleColor.Red);
                }

                MainMenu.ShowMessage($"\n✅ Welcome {user.Username}!", ConsoleColor.Green);
                
                ShowMenu(user);
                
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error during login: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        public void ShowMenu(User currentUser)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool returnToMain = false;

            while (!returnToMain)
            {
                Console.Clear();
                Console.WriteLine($" 🪪 USER MENU - {currentUser.Username}");

                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("  ╔════════════════════════════════════════════╗");
                Console.WriteLine("  ║                👤 USER MENU                ║");
                Console.WriteLine("  ╠════════════════════════════════════════════╣");
                Console.WriteLine("  ║     1️⃣  View Profiles             👥        ║");
                Console.WriteLine("  ║     2️⃣  Interact with Profiles    😍        ║");
                Console.WriteLine("  ║     3️⃣  View Matches             💞        ║");
                Console.WriteLine("  ║     4️⃣  Settings                 ⚙️        ║");
                Console.WriteLine("  ║     0️⃣  Logout                   ❌        ║");
                Console.WriteLine("  ╚════════════════════════════════════════════╝");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Gray;
                string option = MainMenu.ReadText("\n✨ Select an option: ");

                try
                {
                    switch (option)
                    {
                        case "1":
                            //-
                            break;
                        case "2":
                            //--
                            break;
                        case "3":
                            //
                            break;
                        case "4":
                            _settingsMenu.ShowMenu(currentUser);
                            break;    
                        case "0":
                            returnToMain = true;
                            MainMenu.ShowMessage("\n👋 Logging out...", ConsoleColor.Blue);
                            break;
                        default:
                            MainMenu.ShowMessage("⚠️ Invalid option. Please try again.", ConsoleColor.Red);
                            Console.ReadKey();
                            break;  
                    }
                }
                catch (Exception ex)
                {
                    MainMenu.ShowMessage($"\n❌ Error: {ex.Message}", ConsoleColor.Red);
                    Console.ReadKey();
                }
            }
        }
    }
}