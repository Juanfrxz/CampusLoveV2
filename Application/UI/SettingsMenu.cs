using System;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using CampusLove.Infrastructure.Repositories;
using MySql.Data.MySqlClient;

namespace CampusLove.Application.UI
{
    public class SettingsMenu
    {
        private readonly UserRepository _userRepository;
        private readonly ProfileRepository _profileRepository;
        private readonly GenderRepository _genderRepository;
        private readonly ProfessionRepository _professionRepository;
        private readonly StatusRepository _statusRepository;

        public SettingsMenu(MySqlConnection connection)
        {
            _userRepository = new UserRepository(connection);
            _profileRepository = new ProfileRepository(connection);
            _genderRepository = new GenderRepository(connection);
            _professionRepository = new ProfessionRepository(connection);
            _statusRepository = new StatusRepository(connection);
        }

        public void ShowMenu(User currentUser)
        {
            bool returnToMain = false;

            while (!returnToMain)
            {
                Console.Clear();
                Console.WriteLine($"⚙️ SETTINGS MENU {currentUser.Username}");
                Console.WriteLine("------------------");

                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("  ╔════════════════════════════════════════════╗");
                Console.WriteLine("  ║              ⚙️ SETTINGS MENU              ║");
                Console.WriteLine("  ╠════════════════════════════════════════════╣");
                Console.WriteLine("  ║     1️⃣  View Profile              👤        ║");
                Console.WriteLine("  ║     2️⃣  Edit Profile              ✏️        ║");
                Console.WriteLine("  ║     3️⃣  Change Password          🔑        ║");
                Console.WriteLine("  ║     0️⃣  Return to Menu           ↩️        ║");
                Console.WriteLine("  ╚════════════════════════════════════════════╝");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Gray;
                string option = MainMenu.ReadText("\n✨ Select an option: ");

                try
                {
                    switch (option)
                    {
                        case "1":
                            ViewProfile(currentUser).Wait();
                            break;
                        case "2":
                            EditProfile(currentUser).Wait();
                            break;
                        case "3":
                            //ChangePassword(currentUser).Wait();
                            break;
                        case "0":
                            returnToMain = true;
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

        private async Task ViewProfile(User currentUser)
        {
            Console.Clear();
            Console.WriteLine("👤 YOUR PROFILE");
            Console.WriteLine("------------------");

            if (currentUser == null)
            {
                MainMenu.ShowMessage("❌ Invalid user session.", ConsoleColor.Red);
                return;
            }

            var profile = await _profileRepository.GetByIdAsync(currentUser.ProfileId);
            if (profile == null)
            {
                MainMenu.ShowMessage("❌ Profile not found.", ConsoleColor.Red);
                return;
            }

            var gender = await _genderRepository.GetByIdAsync(profile.GenderId);
            var profession = await _professionRepository.GetByIdAsync(profile.ProfessionId);
            var status = await _statusRepository.GetByIdAsync(profile.StatusId);

            Console.WriteLine($"\nName: {profile.Name} {profile.LastName}");
            Console.WriteLine($"Identification: {profile.Identification}");
            Console.WriteLine($"Slogan: {profile.Slogan}");
            Console.WriteLine($"Gender: {gender?.Description}");
            Console.WriteLine($"Profession: {profession?.Description}");
            Console.WriteLine($"Status: {status?.Description}");
            Console.WriteLine($"Total Likes: {profile.TotalLikes}");
            Console.WriteLine($"Created: {profile.createDate}");

            Console.WriteLine("\nAccount Information:");
            Console.WriteLine($"Username: {currentUser.Username}");
            Console.WriteLine($"Birthdate: {currentUser.Birthdate}");

            MainMenu.ShowMessage("\nPress any key to return to settings...", ConsoleColor.Yellow);
            Console.ReadKey();
        }

        private async Task EditProfile(User currentUser)
        {
            Console.Clear();
            Console.WriteLine("👤 YOUR PERSONAL INFORMATION");
            Console.WriteLine("------------------");

            try
            {
                var profile = await _profileRepository.GetByIdAsync(currentUser.ProfileId);
                if (profile == null)
                {
                    MainMenu.ShowMessage("❌ Profile not found.", ConsoleColor.Red);
                    return;
                }
                else
                {
                    string name = MainMenu.ReadText($"Enter your name ({profile.Name}): ");
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        profile.Name = name;
                    } 

                    string lastname = MainMenu.ReadText($"Enter your lastname ({profile.LastName}): ");
                    if (!string.IsNullOrWhiteSpace(lastname))
                    {
                        profile.LastName = lastname;
                    } 

                    string slogan = MainMenu.ReadText($"Enter your slogan ({profile.Slogan}): ");
                    if (!string.IsNullOrWhiteSpace(slogan))
                    {
                        profile.Slogan = slogan;
                    }

                    Console.WriteLine("\n👤 STATUS SELECTION");
                    Console.WriteLine("------------------");

                    var statuses = await _statusRepository.GetAllAsync();

                    if (!statuses.Any())
                    {
                        MainMenu.ShowMessage("❌ Error: No statuses available in the system.", ConsoleColor.Red);
                        return;
                    }

                    Console.WriteLine("\nAvailable statuses:");
                    foreach (var status in statuses)
                    {
                        Console.WriteLine($"ID:  {status.Id} -  {status.Description}");
                    }

                    int statusId = MainMenu.ReadInteger("\nSelect your status ID: ");

                    var selecteStatus = statuses.FirstOrDefault(g => g.Id == statusId);

                    if (selecteStatus == null)
                    {
                        MainMenu.ShowMessage("❌ Invalid status ID. Please select an ID from the list.", ConsoleColor.Red);
                        return;
                    }

                    string confirm = MainMenu.ReadText("\nDo you want to save these changes? (Y/N): ");
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _profileRepository.UpdateAsync(profile);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ Profile updated successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to update the Profile.", ConsoleColor.Red);
                        }
                    }
                    else
                    {
                        MainMenu.ShowMessage("\n⚠️ Update cancelled.", ConsoleColor.Yellow);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error updating the information: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}