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
    public class SignUpMenu
    {
        private readonly UserRepository _userRepository;
        private readonly ProfileRepository _profileRepository;
        private readonly GenderRepository _genderRepository;
        private readonly ProfessionRepository _professionRepository;
        private readonly StatusRepository _statusRepository;
        private readonly InterestRepository _interestRepository;
        private readonly InterestProfileRepository _interestProfileRepository;

        public SignUpMenu(MySqlConnection connection)
        {
            _userRepository = new UserRepository(connection);
            _profileRepository = new ProfileRepository(connection);
            _genderRepository = new GenderRepository(connection);
            _professionRepository = new ProfessionRepository(connection);
            _statusRepository = new StatusRepository(connection);
            _interestRepository = new InterestRepository(connection);
            _interestProfileRepository = new InterestProfileRepository(connection);
        }

        public async Task RegisterUser()
        {
            bool signUpSuccesful = false;

            while (!signUpSuccesful)
            {
                Console.Clear();
                MainMenu.ShowHeader("👥 SIGN UP");

                try
                {
                    string name = MainMenu.ReadText("\nName: ").Trim();
                    string lastname = MainMenu.ReadText("\nLastname: ").Trim();
                    string identification = MainMenu.ReadText("\nIdentification: ").Trim();
                    string slogan = MainMenu.ReadText("\nSlogan: ").Trim();

                    if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(lastname) || string.IsNullOrEmpty(identification) || string.IsNullOrEmpty(slogan))
                    {
                        MainMenu.ShowMessage("❌ Fields cannot be empty.", ConsoleColor.Red);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("\nPress any key to continue... (ESC to return to menu)");
                        Console.ResetColor();

                        var keyr = Console.ReadKey(true);
                        if (keyr.Key == ConsoleKey.Escape)
                        {
                            return;
                        }
                        continue;
                    }

                    MainMenu.ShowText("\n♂️  GENDER SELECTION  ♀️");

                    var genders = await _genderRepository.GetAllAsync();

                    if (!genders.Any())
                    {
                        MainMenu.ShowMessage("❌ Error: No genders available in the system.", ConsoleColor.Red);
                    }

                    // Show available genders list
                    Console.WriteLine("\nAvailable genders:");
                    foreach (var gender in genders)
                    {
                        Console.WriteLine($"ID: {gender.Id} - {gender.Description}");
                    }

                    // Read the entered ID
                    int genderId = MainMenu.ReadInteger("\nSelect your gender ID: ");

                    // Validate if the ID exists in the list
                    var selectedGender = genders.FirstOrDefault(g => g.Id == genderId);

                    if (selectedGender == null)
                    {
                        MainMenu.ShowMessage("❌ Invalid gender ID. Please select an ID from the list.", ConsoleColor.Red);
                    }

                    MainMenu.ShowText("\n🤓 PROFESSION SELECTION");

                    var professions = await _professionRepository.GetAllAsync();

                    if (!professions.Any())
                    {
                        MainMenu.ShowMessage("❌ Error: No professions available in the system.", ConsoleColor.Red);
                    }
                    
                    Console.WriteLine("\nAvailable professions:");
                    foreach (var profession in professions)
                    {
                        Console.WriteLine($"ID: {profession.Id} - {profession.Description}");
                    }

                    int professionId = MainMenu.ReadInteger("\nSelect your profession ID: ");

                    var selectedProfession = professions.FirstOrDefault(g => g.Id == professionId);

                    if (selectedProfession == null)
                    {
                        MainMenu.ShowMessage("❌ Invalid profession ID. Please select an ID from the list.", ConsoleColor.Red);
                    }

                    MainMenu.ShowText("\n💍 STATUS SELECTION");

                    var statuses = await _statusRepository.GetAllAsync();

                    if (!statuses.Any())
                    {
                        MainMenu.ShowMessage("❌ Error: No statuses available in the system.", ConsoleColor.Red);
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
                    }

                    var profile = new Profile
                    {
                        Name = name,
                        LastName = lastname,
                        Identification = identification,
                        GenderId = genderId,
                        Slogan = slogan,
                        StatusId = statusId,
                        createDate = DateTime.Now,
                        ProfessionId = professionId,
                        TotalLikes = 0
                    };

                    MainMenu.ShowText("\n🚴 INTERESTS SELECTION");

                    bool addInterestes = true;
                    while (addInterestes)
                    {
                        var interestes = await _interestRepository.GetAllAsync();

                        if (!interestes.Any())
                        {
                            MainMenu.ShowMessage("❌ Error: No interestes available in the system.", ConsoleColor.Red);
                        }

                        Console.WriteLine("\nAvailable interestes:");
                        foreach (var interest in interestes)
                        {
                            Console.WriteLine($"ID:  {interest.Id} -  {interest.Description}");
                        }

                        int interestId = MainMenu.ReadInteger("\nSelect your interest ID: ");

                        var selectedInterest = interestes.FirstOrDefault(i => i.Id == interestId);

                        if (selectedInterest == null)
                        {
                            MainMenu.ShowMessage("❌ Invalid interest ID. Please select an ID from the list.", ConsoleColor.Red);
                            continue;
                        }

                        profile.Details.Add(new InterestProfile
                        {
                            InterestId = interestId,
                            Interest = selectedInterest
                        });

                        string response = MainMenu.ReadText("\nDo you want to add another interest? (Y/N): ");
                        addInterestes = response.ToUpper() == "Y";
                    }

                    MainMenu.ShowText("\nPROFILE INFORMATION");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"Name: {name}");
                    Console.WriteLine($"Lastname: {lastname}");
                    Console.WriteLine($"Identification: {identification}");
                    Console.WriteLine($"Slogan: {slogan}");
                    Console.WriteLine($"Gender ID: {genderId}");
                    Console.WriteLine($"Profession ID: {professionId}");
                    Console.WriteLine($"Status ID: {statusId}");

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\nInterests:");
                    foreach (var detail in profile.Details)
                    {
                        Console.WriteLine($"- {detail.Interest?.Description ?? "N/A"}");
                    }

                    string confirm = MainMenu.ReadText("\nDo you want to register this profile? (Y/N): ");
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _profileRepository.InsertAsync(profile);

                        if (result)
                        {
                            // Get the newly created profile
                            var lastProfile = await _profileRepository.GetLastProfileAsync();
                            if (lastProfile == null)
                            {
                                MainMenu.ShowMessage("\n❌ Error: Could not retrieve profile information.", ConsoleColor.Red);
                                return;
                            }

                            // Save the interests
                            foreach (var detail in profile.Details)
                            {
                                detail.ProfileId = lastProfile.Id;
                                await _interestProfileRepository.InsertAsync(detail);
                            }

                            MainMenu.ShowMessage("\n✅ Profile registered successfully.", ConsoleColor.Green);
                            
                            Console.Clear();
                            MainMenu.ShowHeader("👤 USER REGISTRATION");
                            Console.WriteLine("\nPress TAB to toggle password visibility");

                            string username = MainMenu.ReadText("\nUsername: ").Trim();
                            if (string.IsNullOrEmpty(username))
                            {
                                MainMenu.ShowMessage("❌ Username cannot be empty.", ConsoleColor.Red);
                            }

                            try 
                            {
                                var existingUser = await _userRepository.GetByUsernameAsync(username);
                                if(existingUser != null)
                                {
                                    MainMenu.ShowMessage("❌ The username already exists.", ConsoleColor.Red);
                                }
                            }
                            catch (Exception ex)
                            {
                                MainMenu.ShowMessage($"❌ Error validating username: {ex.Message}", ConsoleColor.Red);
                            }
                            
                            string password = MainMenu.ReadSecurePassword("\nPassword: ").Trim();
                            if (string.IsNullOrEmpty(password))
                            {
                                MainMenu.ShowMessage("❌ Password cannot be empty.", ConsoleColor.Red);
                            }

                            DateTime birthDate = MainMenu.ReadDate("\nBirthdate (DD/MM/YYYY): ");
                            if (birthDate > DateTime.Now)
                            {
                                MainMenu.ShowMessage("❌ Birthdate cannot be in the future.", ConsoleColor.Red);
                            }

                            MainMenu.ShowText("\nUser Information:");
                            Console.WriteLine($"Username: {username}");
                            Console.WriteLine($"Birthdate: {birthDate.ToShortDateString()}");

                            string response = MainMenu.ReadText("\nDo you want to register this user? (Y/N): ");
                            if (response.ToUpper() == "Y")
                            {
                                var user = new User
                                {
                                    Username = username,
                                    Password = password,
                                    Birthdate = birthDate,
                                    ProfileId = lastProfile.Id
                                };

                                bool userResult = await _userRepository.InsertAsync(user);
                                if (userResult)
                                {
                                    MainMenu.ShowMessage("\n✅ User registered successfully!", ConsoleColor.Green);
                                }
                                else
                                {
                                    MainMenu.ShowMessage("\n❌ Error registering user.", ConsoleColor.Red);
                                }
                            }
                            else
                            {
                                MainMenu.ShowMessage("\n⚠️ User registration cancelled.", ConsoleColor.Yellow);
                            }
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Profile registration failed.", ConsoleColor.Red);
                        }
                    }
                    else
                    {
                        MainMenu.ShowMessage("\n⚠️ Operation cancelled.", ConsoleColor.Yellow);
                    }
                }
                catch (Exception ex)
                {
                    MainMenu.ShowMessage($"\n❌ Error registering profile: {ex.Message}", ConsoleColor.Red);
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\nPress any key to continue...");
                Console.ResetColor();
                Console.ReadKey();
            }
        }
    }
}