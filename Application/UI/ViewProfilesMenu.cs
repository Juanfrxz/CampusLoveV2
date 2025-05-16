using System;
using System.Threading.Tasks;
using System.Linq;
using CampusLove.Domain.Entities;
using CampusLove.Infrastructure.Repositories;
using MySql.Data.MySqlClient;

namespace CampusLove.Application.UI
{
    public class ViewProfilesMenu
    {
        private readonly ProfileRepository _profileRepository;
        private readonly GenderRepository _genderRepository;
        private readonly UserRepository _userRepository;
        private readonly UserLikesRepository _userLikesRepository;
        private readonly ProfessionRepository _professionRepository;
        private readonly StatusRepository _statusRepository;
        private readonly InterestProfileRepository _interestProfileRepository;
        private readonly InterestRepository _interestRepository;

        public ViewProfilesMenu(MySqlConnection connection)
        {
            _profileRepository = new ProfileRepository(connection);
            _genderRepository = new GenderRepository(connection);
            _userRepository = new UserRepository(connection);
            _userLikesRepository = new UserLikesRepository(connection);
            _professionRepository = new ProfessionRepository(connection);
            _statusRepository = new StatusRepository(connection);
            _interestProfileRepository = new InterestProfileRepository(connection);
            _interestRepository = new InterestRepository(connection);
        }

        public async Task ShowMenu(User currentUser)
        {
            bool returnToMain = false;
            while (!returnToMain)
            {
                Console.Clear();
                Console.WriteLine(" 🫂 VIEW PROFILES ");

                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("  ╔════════════════════════════════════════════╗");
                Console.WriteLine("  ║               🫂  VIEW PROFILES             ║");
                Console.WriteLine("  ╠════════════════════════════════════════════╣");
                Console.WriteLine("  ║     1️⃣  View Profiles             👥        ║");
                Console.WriteLine("  ║     2️⃣  Find people               🕵️‍♂️        ║");
                Console.WriteLine("  ║     0️⃣  Return to Menu           ↩️          ║");
                Console.WriteLine("  ╚════════════════════════════════════════════╝");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Gray;
                string option = MainMenu.ReadText("\n✨ Select an option: ");

                switch (option)
                {
                    case "1":
                        await ViewAllProfiles();
                        break;
                    case "2":
                        await FindProfile();
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
        }

        private async Task ViewAllProfiles()
        {
            Console.Clear();
            Console.WriteLine("🫂 VIEW PROFILES");
            Console.WriteLine("------------------");

            try
            {
                var profiles = (await _profileRepository.GetAllAsync()).ToList();
                var genders = (await _genderRepository.GetAllAsync()).ToList();
                var professions = (await _professionRepository.GetAllAsync()).ToList();
                var statuses = (await _statusRepository.GetAllAsync()).ToList();
                var interests = (await _interestRepository.GetAllAsync()).ToList();

                var genderDict = genders.ToDictionary(g => g.Id, g => g.Description);
                var professionDict = professions.ToDictionary(p => p.Id, p => p.Description);
                var statusDict = statuses.ToDictionary(s => s.Id, s => s.Description);
                var interestDict = interests.ToDictionary(i => i.Id, i => i.Description);

                // Load interests for each profile
                foreach (var profile in profiles)
                {
                    var interestProfiles = await _interestProfileRepository.GetAllAsync();
                    profile.Details = interestProfiles.Where(ip => ip.ProfileId == profile.Id).ToList();
                    foreach (var detail in profile.Details)
                    {
                        detail.Interest = interests.FirstOrDefault(i => i.Id == detail.InterestId);
                    }
                }

                if (!profiles.Any())
                {
                    MainMenu.ShowMessage("\nNo profiles found in the system.", ConsoleColor.Yellow);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("\nPress any key to return to menu...");
                    Console.ResetColor();
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine($"\nTotal profiles found: {profiles.Count}");
                Console.WriteLine("------------------");

                int counter = 0;
                foreach (var profile in profiles)
                {
                    try
                    {
                        Console.WriteLine("\n-----------------------------");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Name: {profile.Name ?? "N/A"} {profile.LastName ?? "N/A"}");
                        Console.ResetColor();

                        Console.WriteLine($"Identification: {profile.Identification ?? "N/A"}");
                        Console.WriteLine($"Slogan: {profile.Slogan ?? "N/A"}");

                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine($"Gender: {genderDict.GetValueOrDefault(profile.GenderId, "N/A")}");
                        Console.WriteLine($"Profession: {professionDict.GetValueOrDefault(profile.ProfessionId, "N/A")}");
                        Console.WriteLine($"Status: {statusDict.GetValueOrDefault(profile.StatusId, "N/A")}");
                        Console.ResetColor();

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Total Likes: {profile.TotalLikes}");
                        Console.WriteLine($"Created: {profile.createDate.ToShortDateString()}");
                        Console.ResetColor();

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nInterests:");
                        if (profile.Details.Any())
                        {
                            foreach (var detail in profile.Details)
                            {
                                Console.WriteLine($"- {detail.Interest?.Description ?? "N/A"}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("- No interests listed");
                        }
                        Console.ResetColor();

                        Console.WriteLine("-----------------------------");

                        if (++counter % 3 == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("\nPress any key to continue... (ESC to return to menu)");
                            Console.ResetColor();

                            var key = Console.ReadKey(true);
                            if (key.Key == ConsoleKey.Escape)
                            {
                                return;
                            }

                            Console.Clear();
                            Console.WriteLine("🫂 VIEW PROFILES");
                            Console.WriteLine("------------------");
                            Console.WriteLine($"Total profiles found: {profiles.Count}");
                            Console.WriteLine("------------------");
                        }
                    }
                    catch (Exception ex)
                    {
                        MainMenu.ShowMessage($"\nError displaying profile {profile.Id}: {ex.Message}", ConsoleColor.Red);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("\nPress any key to continue to next profile...");
                        Console.ResetColor();
                        Console.ReadKey();
                    }
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\nPress any key to return to menu...");
                Console.ResetColor();
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError accessing database: {ex.Message}", ConsoleColor.Red);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\nPress any key to return to menu...");
                Console.ResetColor();
                Console.ReadKey();
            }
        }

        private async Task FindProfile()
        {
            Console.Clear();
            Console.WriteLine(" SEARCH PROFILE ");

            string name = MainMenu.ReadText("\nEnter name: ");

            try
            {
                var profiles = await _profileRepository.GetByNameAsync(name);
                var genders = (await _genderRepository.GetAllAsync()).ToList();
                var professions = (await _professionRepository.GetAllAsync()).ToList();
                var statuses = (await _statusRepository.GetAllAsync()).ToList();
                var interests = (await _interestRepository.GetAllAsync()).ToList();

                var genderDict = genders.ToDictionary(g => g.Id, g => g.Description);
                var professionDict = professions.ToDictionary(p => p.Id, p => p.Description);
                var statusDict = statuses.ToDictionary(s => s.Id, s => s.Description);

                if (!profiles.Any())
                {
                    MainMenu.ShowMessage("\nNo profiles found matching the search criteria.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.WriteLine($"\nFound {profiles.Count()} matching profiles:");
                    Console.WriteLine("------------------");

                    foreach (var profile in profiles)
                    {
                        Console.WriteLine("\n-----------------------------");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Name: {profile.Name} {profile.LastName}");
                        Console.ResetColor();

                        Console.WriteLine($"Identification: {profile.Identification}");
                        Console.WriteLine($"Slogan: {profile.Slogan}");

                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine($"Gender: {genderDict.GetValueOrDefault(profile.GenderId, "N/A")}");
                        Console.WriteLine($"Profession: {professionDict.GetValueOrDefault(profile.ProfessionId, "N/A")}");
                        Console.WriteLine($"Status: {statusDict.GetValueOrDefault(profile.StatusId, "N/A")}");
                        Console.ResetColor();

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Total Likes: {profile.TotalLikes}");
                        Console.WriteLine($"Created: {profile.createDate.ToShortDateString()}");
                        Console.ResetColor();

                        // Load and display interests for this profile
                        var interestProfiles = await _interestProfileRepository.GetAllAsync();
                        var profileInterests = interestProfiles.Where(ip => ip.ProfileId == profile.Id).ToList();
                        foreach (var detail in profileInterests)
                        {
                            detail.Interest = interests.FirstOrDefault(i => i.Id == detail.InterestId);
                        }

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nInterests:");
                        if (profileInterests.Any())
                        {
                            foreach (var detail in profileInterests)
                            {
                                Console.WriteLine($"- {detail.Interest?.Description ?? "N/A"}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("- No interests listed");
                        }
                        Console.ResetColor();

                        Console.WriteLine("-----------------------------");
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError searching profiles: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }

    }
}