using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using CampusLove.Infrastructure.Repositories;
using MySql.Data.MySqlClient;

namespace CampusLove.Application.UI
{
    public class GenderMenu
    {
        private readonly GenderRepository _genderRepository;

        public GenderMenu(MySqlConnection connection)
        {
            _genderRepository = new GenderRepository(connection);
        }

        public void ShowMenu()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool returnTo = false; 

            while (!returnTo)
            {
                Console.Clear();
                Console.WriteLine(" ⚧️ GENDER   MENU   ");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("  ╔════════════════════════════════════════════╗");
                Console.WriteLine("  ║               ⚧️  GENDER MENU               ║");
                Console.WriteLine("  ╠════════════════════════════════════════════╣");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  ║       1️⃣  List Gender              📋       ║");
                Console.WriteLine("  ║       2️⃣  Create Gender            ➕       ║");
                Console.WriteLine("  ║       3️⃣  Update Gender            ✏️        ║");
                Console.WriteLine("  ║       4️⃣  Delete Gender            ✖️        ║");
                Console.WriteLine("  ║       0️⃣  Return to Admin Menu     ↩️        ║");
                Console.WriteLine("  ╚════════════════════════════════════════════╝");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                string option = MainMenu.ReadText("\n✨ Select an option: ");

                switch (option)
                {
                    case "1":
                        ListGender().Wait();
                        break;
                    case "2":
                        CreateGender().Wait();
                        break;
                    case "3":
                        UpdateGender().Wait();
                        break;
                    case "4":
                        DeleteGender().Wait();
                        break;
                    case "0":
                        returnTo = true;
                        break;
                    default:
                        MainMenu.ShowMessage("⚠️ Invalid option. Please try again.", ConsoleColor.Red);
                        Console.ReadKey();
                        break;
                }
            }

            MainMenu.ShowMessage("\n👋 Thank you for using the application! Have a great day! 🌟", ConsoleColor.Green);
        }

        private async Task ListGender()
        {
            Console.Clear();
            Console.WriteLine("GENDER LIST");

            try
            {
                var genders = await _genderRepository.GetAllAsync();

                if (!genders.Any())
                {
                    MainMenu.ShowMessage("\nNo genders registered.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\n{0,-5} {1,-12}",
                        "ID", "Description");

                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(new string('-', 50));
                    Console.ResetColor();

                    foreach (var gender in genders)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("{0,-5} {1,-12}",
                            gender.Id,
                            gender.Description);
                    }

                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(new string('-', 50));
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error listing genders: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task CreateGender()
        {
            Console.Clear();
            Console.WriteLine("REGISTER NEW GENDER");

            try
            {
                string nombre = MainMenu.ReadText("\nDescription gender: ").Trim();
                if (string.IsNullOrEmpty(nombre))
                {
                    MainMenu.ShowMessage("Description gender cannot be empty.", ConsoleColor.Red);
                    return;
                }

                var gender = new Gender
                {
                    Description = nombre
                };

                Console.Clear();
                Console.WriteLine("GENDER INFORMATION");

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Description: {nombre}");

                string confirm = MainMenu.ReadText("\nDo you want to register this gender? (Y/N): ");
                if (confirm.ToUpper() == "Y")
                {
                    bool result = await _genderRepository.InsertAsync(gender);

                    if (result)
                    {
                        MainMenu.ShowMessage("\n✅ Gender registered successfully.", ConsoleColor.Green);
                    }
                    else
                    {
                        MainMenu.ShowMessage("\n❌ Gender registration failed.", ConsoleColor.Red);
                    }
                }
                else
                {
                    MainMenu.ShowMessage("\n⚠️ Operation cancelled.", ConsoleColor.Yellow);
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error registering gender: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task UpdateGender()
        {
            Console.Clear();
            Console.WriteLine("UPDATE GENDER");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter gender ID to update: ");
                var gender = await _genderRepository.GetByIdAsync(id);

                if (gender == null)
                {
                    MainMenu.ShowMessage("\n❌ The gender doesn't exist", ConsoleColor.Red);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nCurrent Information:");
                    Console.WriteLine($"Gender ID: {gender.Id}");
                    Console.WriteLine($"Description: {gender.Description}");
                    Console.ResetColor();
                    
                    string nombre = MainMenu.ReadText($"Enter new gender ({gender.Description}): ");
                    if (!string.IsNullOrWhiteSpace(nombre))
                    {
                        gender.Description = nombre;
                    }

                    Console.Clear();
                    Console.WriteLine("UPDATED GENDER INFORMATION");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"ID: {gender.Id}");
                    Console.WriteLine($"Description: {gender.Description}");
                    Console.ResetColor();

                    string confirm = MainMenu.ReadText("\nDo you want to save these changes? (Y/N): ");
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _genderRepository.UpdateAsync(gender);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ Gender updated successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to update the gender.", ConsoleColor.Red);
                        }
                    }
                    else
                    {
                        MainMenu.ShowMessage("\n⚠️ Update cancelled.", ConsoleColor.Yellow);
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error updating the gender: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task DeleteGender()
        {
            Console.Clear();
            Console.WriteLine("DELETE GENDER");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter the gender ID to delete: ");
                var gender = await _genderRepository.GetByIdAsync(id);

                if (gender == null)
                {
                    MainMenu.ShowMessage("\n❌ The gender does not exist.", ConsoleColor.Red);
                }
                else 
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nGender Information:");
                    Console.WriteLine($"ID: {gender.Id}");
                    Console.WriteLine($"Description: {gender.Description}");
                    Console.ResetColor();
                    
                    string confirm = MainMenu.ReadText("\n⚠️ Are you sure you want to delete this gender? (Y/N): ");
                    
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _genderRepository.DeleteAsync(id);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ Gender deleted successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to delete the gender.", ConsoleColor.Red);
                        }
                    }
                    else
                    {
                        MainMenu.ShowMessage("\n⚠️ Operation cancelled.", ConsoleColor.Yellow);
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error deleting the gender: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}