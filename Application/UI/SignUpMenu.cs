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

                Console.WriteLine("\n👤 GENDER SELECTION");
                Console.WriteLine("------------------");

                var genders = await _genderRepository.GetAllAsync();

                if (!genders.Any())
                {
                    MainMenu.ShowMessage("❌ Error: No genders available in the system.", ConsoleColor.Red);
                    return;
                }

                // Mostrar la lista de géneros disponibles
                Console.WriteLine("\nAvailable genders:");
                foreach (var gender in genders)
                {
                    Console.WriteLine($"ID: {gender.Id} - {gender.Description}");
                }

                // Leer el ID ingresado
                int genderId = MainMenu.ReadInteger("\nSelect your gender ID: ");

                // Validar si el ID existe en la lista
                var selectedGender = genders.FirstOrDefault(g => g.Id == genderId);

                if (selectedGender == null)
                {
                    MainMenu.ShowMessage("❌ Invalid gender ID. Please select an ID from the list.", ConsoleColor.Red);
                    return;
                }

                Console.WriteLine("\n👤 PROFESSION SELECTION");
                Console.WriteLine("------------------");

                var professions = await _professionRepository.GetAllAsync();

                if (!professions.Any())
                {
                    MainMenu.ShowMessage("❌ Error: No professions available in the system.", ConsoleColor.Red);
                    return;
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
                    return;
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