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

        public SignUpMenu(MySqlConnection connection)
        {
            _userRepository = new UserRepository(connection);
            _profileRepository = new ProfileRepository(connection);
        }

        public async Task RegisterUser()
        {
            Console.Clear();
            Console.WriteLine("👥 SIGN UP");

           try
           {
                string name = MainMenu.ReadText("\nName: ").Trim();
                if (string.IsNullOrEmpty(name))
                {
                    MainMenu.ShowMessage("Please register your name.", ConsoleColor.Red);
                    return;
                }

                string lastname = MainMenu.ReadText("\nLastname: ").Trim();
                if (string.IsNullOrEmpty(lastname))
                {
                    MainMenu.ShowMessage("Please register your lastname.", ConsoleColor.Red);
                    return;
                }

                string identification = MainMenu.ReadText("\nIdentification: ").Trim();
                if (string.IsNullOrEmpty(identification))
                {
                    MainMenu.ShowMessage("Please register your identification.", ConsoleColor.Red);
                    return;
                }

                string slogan = MainMenu.ReadText("\nSlogan: ").Trim();
                if (string.IsNullOrEmpty(slogan))
                {
                    MainMenu.ShowMessage("Please register your slogan.", ConsoleColor.Red);
                    return;
                }

                //Listar los géneros que hay en la base de datos y validar la entrada

                int genderId = MainMenu.ReadInteger("Register the gender ID: ");
                if (genderId <= 0)
                {
                    MainMenu.ShowMessage("Gender ID must be greater than zero.", ConsoleColor.Red);
                    return;
                }

                //Listar las profesiones que hay en la base de datos y validar la entrada
                int professionId = MainMenu.ReadInteger("Register the Profession ID: ");
                if (professionId <= 0)
                {
                    MainMenu.ShowMessage("Profession ID must be greater than zero.", ConsoleColor.Red);
                    return;
                }

                //Listar las profesiones que hay en la base de datos y validar la entrada
                int statusId = MainMenu.ReadInteger("Register the Status ID: ");
                if (statusId <= 0)
                {
                    MainMenu.ShowMessage("Status ID must be greater than zero.", ConsoleColor.Red);
                    return;
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

                Console.WriteLine("USER INFORMATION");

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Name: {name}");
                Console.WriteLine($"Lastname: {lastname}");
                Console.WriteLine($"Identification: {identification}");
                Console.WriteLine($"Slogan: {slogan}");

                string confirm = MainMenu.ReadText("\nDo you want to register this profile? (Y/N): ");
                if (confirm.ToUpper() == "Y")
                {
                    bool profileResult = await _profileRepository.InsertAsync(profile);
                    if (profileResult)
                    {
                        MainMenu.ShowMessage("\nProfile registered successfully.", ConsoleColor.Green);
                        
                        // Obtener el ID del perfil recién creado
                        var lastProfile = await _profileRepository.GetLastProfileAsync();
                        if (lastProfile == null)
                        {
                            MainMenu.ShowMessage("\nError: Could not retrieve profile information.", ConsoleColor.Red);
                            return;
                        }

                        bool registerUser = true;
                        while (registerUser)
                        {
                            Console.Clear();
                            Console.WriteLine("REGISTER USER INFORMATION");

                            string username = MainMenu.ReadText("\nUsername: ").Trim();
                            if (string.IsNullOrEmpty(username))
                            {
                                MainMenu.ShowMessage("Username cannot be empty.", ConsoleColor.Red);
                                string retry = MainMenu.ReadText("\nDo you want to try again? (Y/N): ");
                                registerUser = retry.ToUpper() == "Y";
                                continue;
                            }

                            try 
                            {
                                var existingUser = await _userRepository.GetByUsernameAsync(username);
                                if(existingUser != null)
                                {
                                    MainMenu.ShowMessage("The username already exists. Try again", ConsoleColor.Red);
                                    string retry = MainMenu.ReadText("\nDo you want to try again? (Y/N): ");
                                    registerUser = retry.ToUpper() == "Y";
                                    continue;
                                }
                            }
                            catch (Exception ex)
                            {
                                MainMenu.ShowMessage($"Error validating username: {ex.Message}", ConsoleColor.Red);
                                string retry = MainMenu.ReadText("\nDo you want to try again? (Y/N): ");
                                registerUser = retry.ToUpper() == "Y";
                                continue;
                            }
                            
                            string password = MainMenu.ReadText("\nPassword: ").Trim();
                            if (string.IsNullOrEmpty(password))
                            {
                                MainMenu.ShowMessage("Please register the password.", ConsoleColor.Red);
                                string retry = MainMenu.ReadText("\nDo you want to try again? (Y/N): ");
                                registerUser = retry.ToUpper() == "Y";
                                continue;
                            }

                            DateTime birthDate = MainMenu.ReadDate("Birthdate (DD/MM/YYYY): ");
                            if (birthDate > DateTime.Now)
                            {
                                MainMenu.ShowMessage("Birthdate cannot be in the future.", ConsoleColor.Red);
                                string retry = MainMenu.ReadText("\nDo you want to try again? (Y/N): ");
                                registerUser = retry.ToUpper() == "Y";
                                continue;
                            }

                            Console.WriteLine("\nUser Information Summary:");
                            Console.WriteLine($"Username: {username}");
                            Console.WriteLine($"Birthdate: {birthDate.ToShortDateString()}");

                            string response = MainMenu.ReadText("\nAre you sure to register this user? (Y/N): ");
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
                                    MainMenu.ShowMessage("\nUser registered successfully!", ConsoleColor.Green);
                                    registerUser = false;
                                }
                                else
                                {
                                    MainMenu.ShowMessage("\nError registering user. Please try again.", ConsoleColor.Red);
                                    string retry = MainMenu.ReadText("\nDo you want to try again? (Y/N): ");
                                    registerUser = retry.ToUpper() == "Y";
                                }
                            }
                            else
                            {
                                string retry = MainMenu.ReadText("\nDo you want to try with a different username? (Y/N): ");
                                registerUser = retry.ToUpper() == "Y";
                            }
                        }
                    }
                    else
                    {
                        MainMenu.ShowMessage("\nProfile registration failed.", ConsoleColor.Red);
                    }
                }
                else
                {
                    MainMenu.ShowMessage("\nOperation cancelled.", ConsoleColor.Yellow);
                }
            }
           catch (Exception ex)
           {
                MainMenu.ShowMessage($"\nError registering person: {ex.Message}", ConsoleColor.Red);
           }

            Console.Write("\nPress any key to continue...");
            Console.ReadKey(); 
        }
    }
}