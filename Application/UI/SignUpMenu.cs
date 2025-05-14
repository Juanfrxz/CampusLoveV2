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

        public SignUpMenu(MySqlConnection connection)
        {
            _userRepository = new UserRepository(connection);
            _profileRepository = new ProfileRepository(connection);
            _genderRepository = new GenderRepository(connection);
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
                    MainMenu.ShowMessage("Name cannot be empty.", ConsoleColor.Red);
                    return;
                }

                string lastname = MainMenu.ReadText("\nLastname: ").Trim();
                if (string.IsNullOrEmpty(lastname))
                {
                    MainMenu.ShowMessage("Lastname cannot be empty.", ConsoleColor.Red);
                    return;
                }

                string identification = MainMenu.ReadText("\nIdentification: ").Trim();
                if (string.IsNullOrEmpty(identification))
                {
                    MainMenu.ShowMessage("Identification cannot be empty.", ConsoleColor.Red);
                    return;
                }

                string slogan = MainMenu.ReadText("\nSlogan: ").Trim();
                if (string.IsNullOrEmpty(slogan))
                {
                    MainMenu.ShowMessage("Slogan cannot be empty.", ConsoleColor.Red);
                    return;
                }

                //Gender ID

                int professionId = MainMenu.ReadInteger("Register the Profession ID: ");
                if (professionId <= 0)
                {
                    MainMenu.ShowMessage("Profession ID must be greater than zero.", ConsoleColor.Red);
                    return;
                }

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