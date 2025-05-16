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
        private readonly InteractWithProfilesMenu _interactMenu;
        private readonly ViewProfilesMenu _viewprofilesMenu;

        public LogInMenu(MySqlConnection connection)
        {
            _userRepository = new UserRepository(connection);
            _profileRepository = new ProfileRepository(connection);
            _reactionRepository = new ReactionRepository(connection);
            _usermatchRepository = new UserMatchRepository(connection);
            _settingsMenu = new SettingsMenu(connection);
            _interactMenu = new InteractWithProfilesMenu(connection);
            _viewprofilesMenu = new ViewProfilesMenu(connection);
        }

        public async Task ValidateUser()
        {
            bool loginSuccessful = false;

            while (!loginSuccessful)
            {
                Console.Clear();
                Console.WriteLine("👥 LOG IN");
                Console.WriteLine("Welcome to CampusLove");
                Console.WriteLine("------------------");
                Console.WriteLine("\nPress TAB to toggle password visibility");

                try
                {
                    string username = MainMenu.ReadText("\nUsername: ").Trim();
                    string password = MainMenu.ReadSecurePassword("Password: ").Trim();
                    
                    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    {
                        MainMenu.ShowMessage("❌ Fields cannot be empty.", ConsoleColor.Red);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("\nPress any key to continue... (ESC to return to menu)");
                        Console.ResetColor();

                        var key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Escape)
                        {
                            return;
                        }
                        continue;
                    }

                    var user = await _userRepository.GetByUsernameAsync(username);
                    if (user == null)
                    {
                        MainMenu.ShowMessage("❌ User not found.", ConsoleColor.Red);
                        Console.ForegroundColor = ConsoleColor.Yellow;

                        Console.Write("\nPress any key to continue... (ESC to return to menu)");
                            Console.ResetColor();

                        var key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Escape)
                        {
                            return;
                        }

                        continue;
                    }

                    if (user.Password != password)
                    {
                        MainMenu.ShowMessage("❌ Incorrect password.", ConsoleColor.Red);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("\nPress any key to continue... (ESC to return to menu)");
                            Console.ResetColor();

                        var key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Escape)
                        {
                            return;
                        }
                        continue;
                    }

                    MainMenu.ShowMessage($"\n✅ Welcome {user.Username}!", ConsoleColor.Green);
                    loginSuccessful = true;
                    ShowMenu(user);
                }
                catch (Exception ex)
                {
                    MainMenu.ShowMessage($"\n❌ Error during login: {ex.Message}", ConsoleColor.Red);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("\nPress any key to continue... (ESC to return to menu)");
                    Console.ResetColor();

                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Escape)
                    {
                        return;
                    }
                }
            }
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
                Console.WriteLine("  ║     3️⃣  View Matches              💞        ║");
                Console.WriteLine("  ║     4️⃣  Settings                   ⚙️        ║");
                Console.WriteLine("  ║     0️⃣  Logout                    ❌        ║");
                Console.WriteLine("  ╚════════════════════════════════════════════╝");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Gray;
                string option = MainMenu.ReadText("\n✨ Select an option: ");

                try
                {
                    switch (option)
                    {
                        case "1":
                            _viewprofilesMenu.ShowMenu(currentUser).Wait();
                            break;
                        case "2":
                            _interactMenu.ShowMenu(currentUser).Wait();
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