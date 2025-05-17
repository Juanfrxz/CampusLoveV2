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
    public class LogInAdminMenu
    {
        private readonly AdministratorRepository _administratorRepository;
        private readonly InterestMenu _interestMenu;
        private readonly GenderMenu _genderMenu;
        private readonly StatusMenu _statusMenu;
        private readonly ProfessionMenu _professionMenu;
        private readonly ProfileRepository _profileRepository;

        public LogInAdminMenu(MySqlConnection connection)
        {
            _administratorRepository = new AdministratorRepository(connection);
            _interestMenu = new InterestMenu(connection);
            _genderMenu = new GenderMenu(connection);
            _statusMenu = new StatusMenu(connection);
            _professionMenu = new ProfessionMenu(connection);
            _profileRepository = new ProfileRepository(connection);
        }

        public async Task ValidateAdmin()
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

                    var administrator = await _administratorRepository.GetByUsernameAsync(username);
                    if (administrator == null)
                    {
                        MainMenu.ShowMessage("❌ Administrator not found.", ConsoleColor.Red);
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

                    if (administrator.Password != password)
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

                    MainMenu.ShowMessage($"\n✅ Welcome {administrator.Username}!", ConsoleColor.Green);
                    loginSuccessful = true;
                    ShowMenu();
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

        public void ShowMenu()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool returnToMain = false;

            while (!returnToMain)
            {
                Console.Clear();
                Console.WriteLine($" 🪪 ADMINISTRATOR MENU ");

                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("  ╔════════════════════════════════════════════╗");
                Console.WriteLine("  ║           👤 ADMINISTRATOR MENU            ║");
                Console.WriteLine("  ╠════════════════════════════════════════════╣");
                Console.WriteLine("  ║     1️⃣  Interestes                🚴        ║");
                Console.WriteLine("  ║     2️⃣  Genders                 ♀️ ♂️         ║");
                Console.WriteLine("  ║     3️⃣  Profession                🤓        ║");
                Console.WriteLine("  ║     4️⃣  Status                    👩‍❤️‍👩     ║");
                Console.WriteLine("  ║     5️⃣  Delete User               ❎        ║");
                Console.WriteLine("  ║     6️⃣  Administrator             📱        ║");
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
                            _interestMenu.ShowMenu();
                            break;
                        case "2":
                            _genderMenu.ShowMenu();
                            break;
                        case "3":
                            _professionMenu.ShowMenu();
                            break;
                        case "4":
                            _statusMenu.ShowMenu();
                            break; 
                        case "5":
                            DeleteProfile().Wait();
                            break; 
                        case "6":
                            //
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

        public async Task DeleteProfile()
        {
            Console.Clear();
            Console.WriteLine("DELETE PROFILE");

            try
            {
                int id = MainMenu.ReadInteger("\nEnter the profile ID to delete: ");
                var profile = await _profileRepository.GetByIdAsync(id);

                if (profile == null)
                {
                    MainMenu.ShowMessage("\n❌ The profile does not exist.", ConsoleColor.Red);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nProfile Information:");
                    Console.WriteLine($"ID: {profile.Id}");
                    Console.WriteLine($"Name: {profile.Name}");
                    Console.WriteLine($"Lastname: {profile.LastName}");
                    Console.WriteLine($"Slogan: {profile.Slogan}");
                    Console.ResetColor();

                    string confirm = MainMenu.ReadText("\n⚠️ Are you sure you want to delete this profile? (Y/N): ");
                    
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _profileRepository.DeleteAsync(id);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ Profile deleted successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to delete the profile.", ConsoleColor.Red);
                        }
                    }
                    else
                    {
                        MainMenu.ShowMessage("\n⚠️ Operation cancelled.", ConsoleColor.Yellow);
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error deleting the profile: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}