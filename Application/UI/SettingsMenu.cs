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
                MainMenu.ShowTitle($"  ⚙️ SETTINGS MENU {currentUser.Username}");

                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("  ╔════════════════════════════════════════════╗");
                Console.WriteLine("  ║             ⚙️  SETTINGS MENU               ║");
                Console.WriteLine("  ╠════════════════════════════════════════════╣");
                Console.WriteLine("  ║     1️⃣  View Profile             👤         ║");
                Console.WriteLine("  ║     2️⃣  Edit Profile             ✏️          ║");
                Console.WriteLine("  ║     3️⃣  Change Password          🔑         ║");
                Console.WriteLine("  ║     0️⃣  Return to Menu           ↩️          ║");
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
                            ChangePassword(currentUser).Wait();
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
            MainMenu.ShowText("👤 YOUR PROFILE");

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

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\nName: {profile.Name} {profile.LastName}");
            Console.WriteLine($"Identification: {profile.Identification}");
            Console.WriteLine($"Slogan: {profile.Slogan}");

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"Gender: {gender?.Description}");
            Console.WriteLine($"Profession: {profession?.Description}");
            Console.WriteLine($"Status: {status?.Description}");
            Console.WriteLine($"Total Likes: {profile.TotalLikes}");
            Console.WriteLine($"Created: {profile.createDate}");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nAccount Information:");
            Console.WriteLine($"Username: {currentUser.Username}");
            Console.WriteLine($"Birthdate: {currentUser.Birthdate}");

            MainMenu.ShowMessage("\nPress any key to return to settings...", ConsoleColor.Yellow);
            Console.ReadKey();
        }

        private async Task EditProfile(User currentUser)
        {
            Console.Clear();
            MainMenu.ShowText("👤 YOUR PERSONAL INFORMATION");

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

                    Console.WriteLine("\n💍 STATUS SELECTION");
                    Console.WriteLine("-------------------------");

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

                    var selectedStatus = statuses.FirstOrDefault(g => g.Id == statusId);

                    if (selectedStatus == null)
                    {
                        MainMenu.ShowMessage("❌ Invalid status ID. Please select an ID from the list.", ConsoleColor.Red);
                        return;
                    }

                    // Update the status ID
                    profile.StatusId = selectedStatus.Id;

                    Console.Clear();
                    MainMenu.ShowText("Profile Update Summary:");
                    Console.WriteLine($"Name: {profile.Name}");
                    Console.WriteLine($"Lastname: {profile.LastName}");
                    Console.WriteLine($"Slogan: {profile.Slogan}");
                    Console.WriteLine($"Status: {selectedStatus.Description}");

                    string confirm = MainMenu.ReadText("\nDo you want to save these changes? (Y/N): ");
                    if (confirm.ToUpper() == "Y")
                    {
                        try 
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
                        catch (Exception ex)
                        {
                            MainMenu.ShowMessage($"\n❌ Database error: {ex.Message}", ConsoleColor.Red);
                            // Log the full exception details for debugging
                            Console.WriteLine($"\nDetailed error: {ex}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error updating the information: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task ChangePassword(User currentUser)
        {
            Console.Clear();
            Console.WriteLine("🔑 CHANGE PASSWORD");
            Console.WriteLine("----------------------");
            Console.WriteLine("\nPress TAB to toggle password visibility");

            try
            {
                string currentPassword = MainMenu.ReadSecurePassword("\nEnter your current password: ");
                if (string.IsNullOrEmpty(currentPassword))
                {
                    MainMenu.ShowMessage("❌ Current password cannot be empty.", ConsoleColor.Red);
                    return;
                }

                // Verify current password
                var user = await _userRepository.GetByIdAsync(currentUser.Id);
                if (user == null || user.Password != currentPassword)
                {
                    MainMenu.ShowMessage("❌ Current password is incorrect.", ConsoleColor.Red);
                    return;
                }

                string newPassword = MainMenu.ReadSecurePassword("\nEnter your new password: ");
                if (string.IsNullOrEmpty(newPassword))
                {
                    MainMenu.ShowMessage("❌ New password cannot be empty.", ConsoleColor.Red);
                    return;
                }

                string confirmPassword = MainMenu.ReadSecurePassword("\nConfirm your new password: ");
                if (newPassword != confirmPassword)
                {
                    MainMenu.ShowMessage("❌ Passwords do not match.", ConsoleColor.Red);
                    return;
                }

                // Update password
                user.Password = newPassword;
                bool result = await _userRepository.UpdateAsync(user);

                if (result)
                {
                    MainMenu.ShowMessage("\n✅ Password changed successfully!", ConsoleColor.Green);
                }
                else
                {
                    MainMenu.ShowMessage("\n❌ Error changing password.", ConsoleColor.Red);
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}