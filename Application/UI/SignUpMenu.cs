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

        public SignUpMenu(MySqlConnection connection)
        {
            _userRepository = new UserRepository(connection);
            _profileRepository = new ProfileRepository(connection);
            _genderRepository = new GenderRepository(connection);
            _professionRepository = new ProfessionRepository(connection);
            _statusRepository = new StatusRepository(connection);
        }

        public async Task RegisterUser()
        {
            bool signUpSuccesful = false;

            while (!signUpSuccesful)
            {
                Console.Clear();
                Console.WriteLine("👥 SIGN UP");

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

                    Console.WriteLine("\n👤 GENDER SELECTION");
                    Console.WriteLine("------------------");

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

                    Console.WriteLine("\n👤 PROFESSION SELECTION");
                    Console.WriteLine("------------------");

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

                    Console.WriteLine("\n👤 STATUS SELECTION");
                    Console.WriteLine("------------------");

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

                    Console.WriteLine("\nPROFILE INFORMATION");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"Name: {name}");
                    Console.WriteLine($"Lastname: {lastname}");
                    Console.WriteLine($"Identification: {identification}");
                    Console.WriteLine($"Slogan: {slogan}");
                    Console.WriteLine($"Gender ID: {genderId}");
                    Console.WriteLine($"Profession ID: {professionId}");
                    Console.WriteLine($"Status ID: {statusId}");

                    string confirm = MainMenu.ReadText("\nDo you want to register this profile? (Y/N): ");
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _profileRepository.InsertAsync(profile);

                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ Profile registered successfully.", ConsoleColor.Green);
                            
                            // Get the newly created profile
                            var lastProfile = await _profileRepository.GetLastProfileAsync();
                            if (lastProfile == null)
                            {
                                MainMenu.ShowMessage("\n❌ Error: Could not retrieve profile information.", ConsoleColor.Red);
                                return;
                            }

                            Console.Clear();
                            Console.WriteLine("👤 USER REGISTRATION");
                            Console.WriteLine("------------------");
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

                            Console.WriteLine("\nUser Information Summary:");
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
}