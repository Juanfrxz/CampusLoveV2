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
        private readonly UserLikesRepository _userLikesRepository;
        private int? _preferredGenderId;
        private const int MAX_LIKES = 10;

        public InteractWithProfilesMenu(MySqlConnection connection)
        {
            _profileRepository = new ProfileRepository(connection);
            _genderRepository = new GenderRepository(connection);
            _userRepository = new UserRepository(connection);
            _userLikesRepository = new UserLikesRepository(connection);
        }

        public async Task ShowMenu(User currentUser)
        {
            bool returnToMain = false;
            while (!returnToMain)
            {
                Console.Clear();
                Console.WriteLine("💘  INTERACT WITH PROFILES");
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
            Console.WriteLine("♀️ ♂️  SELECT GENDER PREFERENCE");
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

                await ShowProfiles(filteredProfiles, currentUser);
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"❌ Error loading profiles: {ex.Message}", ConsoleColor.Red);
                Console.WriteLine($"\nDetailed error: {ex}");
                Console.ReadKey();
            }
        }

        private async Task ShowProfiles(List<Profile> profiles, User currentUser)
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

                    // Verificar si ya dio like a este perfil
                    bool hasLiked = await _userLikesRepository.HasUserLikedProfileAsync(currentUser.Id, profile.Id);
                    if (hasLiked)
                    {
                        Console.WriteLine("\n⚠️ Ya has dado like a este perfil");
                    }
                    else
                    {
                        // Verificar límite de likes
                        int likeCount = await _userLikesRepository.GetLikeCountByUserIdAsync(currentUser.Id);
                        Console.WriteLine($"\nLikes restantes hoy: {MAX_LIKES - likeCount}");

                        if (likeCount >= MAX_LIKES)
                        {
                            Console.WriteLine("\n❌ Has alcanzado el límite de likes por hoy");
                        }
                    }

                    Console.WriteLine("\nOpciones:");
                    Console.WriteLine("1. Like ❤️");
                    Console.WriteLine("2. Saltar ⏭️");
                    Console.WriteLine("3. Volver al Menú ↩️");

                    string option = MainMenu.ReadText("\nSelecciona una opción: ");
                    switch (option)
                    {
                        case "1":
                            await HandleLike(currentUser, profile);
                            break;
                        case "2":
                            continue;
                        case "3":
                            return;
                        default:
                            MainMenu.ShowMessage("⚠️ Opción inválida.", ConsoleColor.Red);
                            break;
                    }
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    MainMenu.ShowMessage($"❌ Error al mostrar el perfil: {ex.Message}", ConsoleColor.Red);
                    Console.WriteLine($"\nError detallado: {ex}");
                    Console.ReadKey();
                }
            }
        }

        private async Task HandleLike(User currentUser, Profile likedProfile)
        {
            try
            {
                // Verificar si ya dio like
                bool hasLiked = await _userLikesRepository.HasUserLikedProfileAsync(currentUser.Id, likedProfile.Id);
                if (hasLiked)
                {
                    MainMenu.ShowMessage("⚠️ Ya has dado like a este perfil", ConsoleColor.Yellow);
                    return;
                }

                // Verificar límite de likes
                int likeCount = await _userLikesRepository.GetLikeCountByUserIdAsync(currentUser.Id);
                if (likeCount >= MAX_LIKES)
                {
                    MainMenu.ShowMessage("❌ Has alcanzado el límite de likes por hoy", ConsoleColor.Red);
                    return;
                }

                // Crear el like
                var like = new UserLikes
                {
                    UserId = currentUser.Id,
                    LikedProfileId = likedProfile.Id,
                    IsMatch = false
                };

                await _userLikesRepository.CreateLikeAsync(like);

                // Verificar si hay match
                bool isMatch = await CheckForMatch(currentUser.Id, likedProfile.Id);
                if (isMatch)
                {
                    MainMenu.ShowMessage("🎉 ¡MATCH! ¡Os habéis gustado mutuamente!", ConsoleColor.Green);
                }
                else
                {
                    MainMenu.ShowMessage("❤️ ¡Like enviado!", ConsoleColor.Green);
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"❌ Error al dar like: {ex.Message}", ConsoleColor.Red);
            }
        }

        private async Task<bool> CheckForMatch(int userId, int likedProfileId)
        {
            // Verificar si el perfil que recibió el like también dio like al usuario
            var userProfile = await _profileRepository.GetByUserIdAsync(userId);
            if (userProfile == null) return false;

            return await _userLikesRepository.HasUserLikedProfileAsync(likedProfileId, userProfile.Id);
        }
    }
} 