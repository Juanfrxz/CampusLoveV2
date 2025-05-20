using System;
using System.Threading.Tasks;
using System.Linq;
using CampusLove.Domain.Entities;
using CampusLove.Infrastructure.Repositories;
using MySql.Data.MySqlClient;
using Spectre.Console;

namespace CampusLove.Application.UI
{
    public class InteractWithProfilesMenu
    {
        private readonly ProfileRepository _profileRepository;
        private readonly GenderRepository _genderRepository;
        private readonly UserRepository _userRepository;
        private readonly UserLikesRepository _userLikesRepository;
        private readonly UserMatchRepository _userMatchRepository;
        private readonly UserDislikesRepository _userdislikesRepository;
        private int? _preferredGenderId;
        private const int MAX_LIKES = 10;

        public InteractWithProfilesMenu(MySqlConnection connection)
        {
            _profileRepository = new ProfileRepository(connection);
            _genderRepository = new GenderRepository(connection);
            _userRepository = new UserRepository(connection);
            _userLikesRepository = new UserLikesRepository(connection);
            _userMatchRepository = new UserMatchRepository(connection);
            _userdislikesRepository = new UserDislikesRepository(connection);
        }

        public async Task ShowMenu(User currentUser)
        {
            bool returnToMain = false;
            while (!returnToMain)
            {
                Console.Clear();

                // Título con Figlet y Panel
                var title = new FigletText("💘 INTERACT")
                    .Centered()
                    .Color(Color.Blue);

                var panel = new Panel(title)
                {
                    Border = BoxBorder.Rounded,
                    Padding = new Padding(1, 1, 1, 1),
                    Header = new PanelHeader(" 💞 CampusLove 💞 ", Justify.Center),
                };

                AnsiConsole.Write(panel);
                AnsiConsole.WriteLine();

                // Menú interactivo con SelectionPrompt
                var menu = new SelectionPrompt<string>()
                    .Title("[bold blue]Select an option:[/]")
                    .PageSize(4)
                    .AddChoices(new[]
                    {
                        "👥  Browse Profiles",
                        "♀️ ♂️  Change Gender Preference",
                        "↩️  Return to Menu"
                    });

                var option = AnsiConsole.Prompt(menu);

                try
                {
                    switch (option)
                    {
                        case "👥  Browse Profiles":
                            if (_preferredGenderId == null)
                            {
                                await SelectGenderPreference();
                            }
                            await BrowseProfiles(currentUser);
                            break;
                        case "♀️ ♂️  Change Gender Preference":
                            await SelectGenderPreference();
                            break;
                        case "↩️  Return to Menu":
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
            MainMenu.ShowText("♀️ ♂️  SELECT GENDER PREFERENCE");

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
                MainMenu.ShowText("👥 Browsing Profiles...");
                
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

        private async Task ShowProfiles(System.Collections.Generic.List<CampusLove.Domain.Entities.Profile> profiles, User currentUser)
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

                    // Check if already liked this profile
                    bool hasLiked = await _userLikesRepository.HasUserLikedProfileAsync(currentUser.Id, profile.Id);
                    if (hasLiked)
                    {
                        Console.WriteLine("\n⚠️ You have already liked this profile");
                    }
                    else
                    {
                        // Calculate remaining likes including bonus
                        int likeCount = await _userLikesRepository.GetLikeCountByUserIdAsync(currentUser.Id);
                        int totalAllowed = MAX_LIKES + currentUser.BonusLikes;
                        Console.WriteLine($"\nRemaining likes today: {totalAllowed - likeCount} (Base {MAX_LIKES} + Bonus {currentUser.BonusLikes})");

                        if (likeCount >= totalAllowed)
                        {
                            Console.WriteLine("\n❌ You have reached your daily like limit");
                        }
                    }

                    MainMenu.ShowText("\nOptions:");
                    Console.WriteLine("1. Like ❤️");
                    Console.WriteLine("2. Dislike 👎");
                    Console.WriteLine("3. Skip ⏭️");
                    Console.WriteLine("4. Return to Menu ↩️");

                    string option = MainMenu.ReadText("\nSelect an option: ");
                    switch (option)
                    {
                        case "1":
                            await HandleLike(currentUser, profile);
                            break;
                        case "2":
                            await HandleDislike(currentUser, profile);
                            break;
                        case "3":
                            continue;
                        case "4":
                            return;
                        default:
                            MainMenu.ShowMessage("⚠️ Invalid option.", ConsoleColor.Red);
                            break;
                    }
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    MainMenu.ShowMessage($"❌ Error displaying profile: {ex.Message}", ConsoleColor.Red);
                    Console.WriteLine($"\nDetailed error: {ex}");
                    Console.ReadKey();
                }
            }
        }

        private async Task HandleLike(User currentUser, CampusLove.Domain.Entities.Profile likedProfile)
        {
            try
            {
                // Check if already liked
                bool hasLiked = await _userLikesRepository.HasUserLikedProfileAsync(currentUser.Id, likedProfile.Id);
                if (hasLiked)
                {
                    MainMenu.ShowMessage("⚠️ You have already liked this profile", ConsoleColor.Yellow);
                    return;
                }

                // Check like limit including bonus
                int likeCount = await _userLikesRepository.GetLikeCountByUserIdAsync(currentUser.Id);
                int totalAllowed = MAX_LIKES + currentUser.BonusLikes;
                if (likeCount >= totalAllowed)
                {
                    MainMenu.ShowMessage("❌ You have reached your daily like limit", ConsoleColor.Red);
                    return;
                }

                // Create the like
                var like = new UserLikes
                {
                    UserId = currentUser.Id,
                    LikedProfileId = likedProfile.Id,
                    IsMatch = false
                };

                await _userLikesRepository.CreateLikeAsync(like);
                // Deduct bonus like if using bonus beyond base limit
                if (likeCount >= MAX_LIKES)
                {
                    currentUser.BonusLikes--;
                    await _userRepository.UpdateAsync(currentUser);
                }

                // Check for match
                bool isMatch = await CheckForMatch(currentUser.Id, likedProfile.Id);
                if (isMatch)
                {
                    MainMenu.ShowMessage("🎉 MATCH! You both liked each other!", ConsoleColor.Green);
                }
                else
                {
                    MainMenu.ShowMessage("❤️ Like sent!", ConsoleColor.Green);
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"❌ Error sending like: {ex.Message}", ConsoleColor.Red);
            }
        }

        private async Task<bool> CheckForMatch(int userId, int likedProfileId)
        {
            try
            {
                var userProfile = await _profileRepository.GetByUserIdAsync(userId);
                if (userProfile == null) return false;

                var otherUser = await _userRepository.GetByProfileIdAsync(likedProfileId);
                if (otherUser == null) return false;

                bool hasLikedBack = await _userLikesRepository.HasUserLikedProfileAsync(otherUser.Id, userProfile.Id);

                if (hasLikedBack)
                {
                    var match = new UserMatch
                    {
                        User1_id = userId,
                        User2_id = otherUser.Id,
                        MatchDate = DateTime.Now
                    };

                    await _userMatchRepository.InsertAsync(match);

                    // Update the like status to mark it as a match
                    var likes = await _userLikesRepository.GetLikesByUserIdAsync(userId);
                    var thisLike = likes.FirstOrDefault(l => l.LikedProfileId == likedProfileId);
                    if (thisLike != null)
                    {
                        await _userLikesRepository.UpdateMatchStatusAsync(thisLike.Id, true);
                    }

                    var otherLikes = await _userLikesRepository.GetLikesByUserIdAsync(otherUser.Id);
                    var otherLike = otherLikes.FirstOrDefault(l => l.LikedProfileId == userProfile.Id);
                    if (otherLike != null)
                    {
                        await _userLikesRepository.UpdateMatchStatusAsync(otherLike.Id, true);
                    }
                }

                return hasLikedBack;
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"❌ Error checking for match: {ex.Message}", ConsoleColor.Red);
                return false;
            }
        }

        private async Task HandleDislike(User currentUser, CampusLove.Domain.Entities.Profile dislikedProfile)
        {
            try
            {
                // Check if already disliked
                bool hasDisliked = await _userdislikesRepository.HasUserDislikedProfileAsync(currentUser.Id, dislikedProfile.Id);
                if (hasDisliked)
                {
                    MainMenu.ShowMessage("⚠️ You have already disliked this profile", ConsoleColor.Yellow);
                    return;
                }

                // Create the dislike
                var dislike = new UserDislikes
                {
                    UserId = currentUser.Id,
                    DislikedProfileId = dislikedProfile.Id,
                    DislikeDate = DateTime.UtcNow
                };

                var createdDislike = await _userdislikesRepository.CreateDislikeAsync(dislike);

                if (createdDislike != null && createdDislike.Id > 0)
                {
                    MainMenu.ShowMessage("👎 Dislike sent!", ConsoleColor.Green);
                }
                else
                {
                    MainMenu.ShowMessage("❌ Failed to send dislike", ConsoleColor.Red);
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"❌ Error sending dislike: {ex.Message}", ConsoleColor.Red);
            }
        }
    }
}