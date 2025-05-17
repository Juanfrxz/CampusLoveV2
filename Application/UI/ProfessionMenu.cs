using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using CampusLove.Infrastructure.Repositories;
using MySql.Data.MySqlClient;

namespace CampusLove.Application.UI
{
    public class ProfessionMenu
    {
        private readonly ProfessionRepository _professionRepository;

        public ProfessionMenu(MySqlConnection connection)
        {
            _professionRepository = new ProfessionRepository(connection);
        }

        public void ShowMenu()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool returnTo = false; 

            while (!returnTo)
            {
                Console.Clear();
                Console.WriteLine(" 🤓 PROFESSION MENU   ");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("  ╔════════════════════════════════════════════╗");
                Console.WriteLine("  ║            🤓  PROFESSION MENU             ║");
                Console.WriteLine("  ╠════════════════════════════════════════════╣");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  ║       1️⃣  List Profession              📋   ║");
                Console.WriteLine("  ║       2️⃣  Create Profession            ➕   ║");
                Console.WriteLine("  ║       3️⃣  Update Profession            ✏️    ║");
                Console.WriteLine("  ║       4️⃣  Delete Profession            ✖️    ║");
                Console.WriteLine("  ║       0️⃣  Return to Admin Menu     ↩️        ║");
                Console.WriteLine("  ╚════════════════════════════════════════════╝");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                string option = MainMenu.ReadText("\n✨ Select an option: ");

                switch (option)
                {
                    case "1":
                        ListProfession().Wait();
                        break;
                    case "2":
                        CreateProfession().Wait();
                        break;
                    case "3":
                        UpdateProfession().Wait();
                        break;
                    case "4":
                        DeleteProfession().Wait();
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

        private async Task ListProfession()
        {
            Console.Clear();
            Console.WriteLine("PROFESSION LIST");

            try
            {
                var professions = await _professionRepository.GetAllAsync();

                if (!professions.Any())
                {
                    MainMenu.ShowMessage("\nNo professions registered.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\n{0,-5} {1,-12}",
                        "ID", "Description");

                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(new string('-', 50));
                    Console.ResetColor();

                    foreach (var profession in professions)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("{0,-5} {1,-12}",
                            profession.Id,
                            profession.Description);
                    }

                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(new string('-', 50));
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error listing professions: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task CreateProfession()
        {
            Console.Clear();
            Console.WriteLine("REGISTER NEW PROFESSION");

            try
            {
                string nombre = MainMenu.ReadText("\nDescription profession: ").Trim();
                if (string.IsNullOrEmpty(nombre))
                {
                    MainMenu.ShowMessage("Description profession cannot be empty.", ConsoleColor.Red);
                    return;
                }

                var profession = new Profession
                {
                    Description = nombre
                };

                Console.Clear();
                Console.WriteLine("PROFESSION INFORMATION");

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Description: {nombre}");

                string confirm = MainMenu.ReadText("\nDo you want to register this profession? (Y/N): ");
                if (confirm.ToUpper() == "Y")
                {
                    bool result = await _professionRepository.InsertAsync(profession);

                    if (result)
                    {
                        MainMenu.ShowMessage("\n✅ Profession registered successfully.", ConsoleColor.Green);
                    }
                    else
                    {
                        MainMenu.ShowMessage("\n❌ Profession registration failed.", ConsoleColor.Red);
                    }
                }
                else
                {
                    MainMenu.ShowMessage("\n⚠️ Operation cancelled.", ConsoleColor.Yellow);
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error registering profession: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task UpdateProfession()
        {
            Console.Clear();
            Console.WriteLine("UPDATE PROFESSION");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter profession ID to update: ");
                var profession = await _professionRepository.GetByIdAsync(id);

                if (profession == null)
                {
                    MainMenu.ShowMessage("\n❌ The profession doesn't exist", ConsoleColor.Red);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nCurrent Information:");
                    Console.WriteLine($"Profession ID: {profession.Id}");
                    Console.WriteLine($"Description: {profession.Description}");
                    Console.ResetColor();
                    
                    string nombre = MainMenu.ReadText($"Enter new profession ({profession.Description}): ");
                    if (!string.IsNullOrWhiteSpace(nombre))
                    {
                        profession.Description = nombre;
                    }

                    Console.Clear();
                    Console.WriteLine("UPDATED PROFESSION INFORMATION");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"ID: {profession.Id}");
                    Console.WriteLine($"Description: {profession.Description}");
                    Console.ResetColor();

                    string confirm = MainMenu.ReadText("\nDo you want to save these changes? (Y/N): ");
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _professionRepository.UpdateAsync(profession);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ Profession updated successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to update the profession.", ConsoleColor.Red);
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
                MainMenu.ShowMessage($"\n❌ Error updating the profession: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task DeleteProfession()
        {
            Console.Clear();
            Console.WriteLine("DELETE PROFESSION");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter the profession ID to delete: ");
                var profession = await _professionRepository.GetByIdAsync(id);

                if (profession == null)
                {
                    MainMenu.ShowMessage("\n❌ The profession does not exist.", ConsoleColor.Red);
                }
                else 
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nProfession Information:");
                    Console.WriteLine($"ID: {profession.Id}");
                    Console.WriteLine($"Description: {profession.Description}");
                    Console.ResetColor();
                    
                    string confirm = MainMenu.ReadText("\n⚠️ Are you sure you want to delete this profession? (Y/N): ");
                    
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _professionRepository.DeleteAsync(id);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ Profession deleted successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to delete the profession.", ConsoleColor.Red);
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
                MainMenu.ShowMessage($"\n❌ Error deleting the profession: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}