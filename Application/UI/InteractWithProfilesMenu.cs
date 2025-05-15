using System;
using System.Threading.Tasks;
using System.Linq;
using CampusLove.Domain.Entities;
using CampusLove.Infrastructure.Repositories;
using MySql.Data.MySqlClient;

namespace CampusLove.Application.UI
{
    public class InteractWithProfilesMenu
    {
        private readonly ProfileRepository _profileRepository;
        private readonly GenderRepository _genderRepository;
        private readonly UserRepository _userRepository;
        private int? _preferredGenderId;

        public InteractWithProfilesMenu(MySqlConnection connection)
        {
            _profileRepository = new ProfileRepository(connection);
            _genderRepository = new GenderRepository(connection);
            _userRepository = new UserRepository(connection);
        }

        public async Task ShowMenu(User currentUser)
        {
            bool returnToMain = false;
            while (!returnToMain)
            {
                Console.Clear();
                Console.WriteLine("💘 INTERACT WITH PROFILES");
                Console.WriteLine("------------------");

                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("  ╔════════════════════════════════════════════╗");
                Console.WriteLine("  ║           💘 INTERACT WITH PROFILES        ║");
                Console.WriteLine("  ╠════════════════════════════════════════════╣");
                Console.WriteLine("  ║     1️⃣  Browse Profiles           👥        ║");
                Console.WriteLine("  ║     2️⃣  Change Gender Preference  ♀️ ♂️       ║");
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
                            if (_preferredGenderId == null)
                            {
                                await SelectGenderPreference();
                            }
                            await BrowseProfiles(currentUser);
                            break;
                        case "2":
                            await SelectGenderPreference();
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

        private async Task SelectGenderPreference()
        {
            Console.Clear();
            Console.WriteLine("♀️♂️ SELECT GENDER PREFERENCE");
            Console.WriteLine("------------------");

            var genders = await _genderRepository.GetAllAsync();
            if (!genders.Any())
            {
                MainMenu.ShowMessage("❌ No genders available in the system.", ConsoleColor.Red);
                return;
            }

            Console.WriteLine("\nAvailable genders:");
            Console.WriteLine("ID: 0 - All Genders 🌈");
            foreach (var gender in genders)
            {
                Console.WriteLine($"ID: {gender.Id} - {gender.Description}");
            }

            int genderId = MainMenu.ReadInteger("\nSelect your preferred gender ID (0 for all): ");
            
            if (genderId == 0)
            {
                _preferredGenderId = null; // null means all genders
                MainMenu.ShowMessage("✅ Gender preference set to: All Genders 🌈", ConsoleColor.Green);
                Console.ReadKey();
                return;
            }

            var selectedGender = genders.FirstOrDefault(g => g.Id == genderId);

            if (selectedGender == null)
            {
                MainMenu.ShowMessage("❌ Invalid gender ID. Please select an ID from the list.", ConsoleColor.Red);
                return;
            }

            _preferredGenderId = selectedGender.Id;
            MainMenu.ShowMessage($"✅ Gender preference set to: {selectedGender.Description}", ConsoleColor.Green);
            Console.ReadKey();
        }

        private async Task BrowseProfiles(User currentUser)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("👥 Browsing Profiles...");
                
                var profiles = await _profileRepository.GetAllAsync();
                if (profiles == null || !profiles.Any())
                {
                    MainMenu.ShowMessage("❌ No profiles available in the system.", ConsoleColor.Red);
                    Console.ReadKey();
                    return;
                }

                var filteredProfiles = profiles
                    .Where(p => p != null && p.Id != currentUser.ProfileId)
                    .ToList();

                if (_preferredGenderId.HasValue)
                {
                    filteredProfiles = filteredProfiles
                        .Where(p => p.GenderId == _preferredGenderId.Value)
                        .ToList();
                }

                if (!filteredProfiles.Any())
                {
                    MainMenu.ShowMessage("❌ No profiles available matching your preferences.", ConsoleColor.Red);
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine($"Found {filteredProfiles.Count} profiles to show.");
                Console.ReadKey();

                ShowProfiles(filteredProfiles);
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"❌ Error loading profiles: {ex.Message}", ConsoleColor.Red);
                Console.WriteLine($"\nDetailed error: {ex}");
                Console.ReadKey();
            }
        }

        private void ShowProfiles(List<Profile> profiles)
        {
            foreach (var profile in profiles)
            {
                try
                {
                    Console.Clear();
                    Console.WriteLine("👤 PROFILE");
                    Console.WriteLine("------------------");
                    Console.WriteLine($"Name: {profile.Name} {profile.LastName}");
                    Console.WriteLine($"Slogan: {profile.Slogan}");
                    Console.WriteLine($"Total Likes: {profile.TotalLikes}");

                    Console.WriteLine("\nOptions:");
                    Console.WriteLine("1. Like ❤️");
                    Console.WriteLine("2. Skip ⏭️");
                    Console.WriteLine("3. Return to Menu ↩️");

                    string option = MainMenu.ReadText("\nSelect an option: ");
                    switch (option)
                    {
                        case "1":
                            // TODO: Implement like functionality
                            MainMenu.ShowMessage("❤️ You liked this profile!", ConsoleColor.Green);
                            break;
                        case "2":
                            continue;
                        case "3":
                            return;
                        default:
                            MainMenu.ShowMessage("⚠️ Invalid option.", ConsoleColor.Red);
                            break;
                    }
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    MainMenu.ShowMessage($"❌ Error showing profile: {ex.Message}", ConsoleColor.Red);
                    Console.WriteLine($"\nDetailed error: {ex}");
                    Console.ReadKey();
                }
            }
        }
    }
} 