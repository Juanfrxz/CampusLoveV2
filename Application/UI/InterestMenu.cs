using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using CampusLove.Infrastructure.Repositories;
using MySql.Data.MySqlClient;

namespace CampusLove.Application.UI
{
    public class InterestMenu
    {
        private readonly InterestRepository _interestRepository;

        public InterestMenu(MySqlConnection connection)
        {
            _interestRepository = new InterestRepository(connection);
        }

        public void ShowMenu()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool returnTo = false; 

            while (!returnTo)
            {
                Console.Clear();
                Console.WriteLine(" 🚴 INTEREST MENU   ");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("  ╔════════════════════════════════════════════╗");
                Console.WriteLine("  ║               🚴  INTEREST MENU            ║");
                Console.WriteLine("  ╠════════════════════════════════════════════╣");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  ║       1️⃣  List Interest            📋       ║");
                Console.WriteLine("  ║       2️⃣  Create Interest          ➕       ║");
                Console.WriteLine("  ║       3️⃣  Update Interest          ✏️        ║");
                Console.WriteLine("  ║       4️⃣  Delete Interest          ✖️        ║");
                Console.WriteLine("  ║       0️⃣  Return to Admin Menu     ↩️        ║");
                Console.WriteLine("  ╚════════════════════════════════════════════╝");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                string option = MainMenu.ReadText("\n✨ Select an option: ");

                switch (option)
                {
                    case "1":
                        ListInterest().Wait();
                        break;
                    case "2":
                        CreateInterest().Wait();
                        break;
                    case "3":
                        UpdateInterest().Wait();
                        break;
                    case "4":
                        DeleteInterest().Wait();
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

        private async Task ListInterest()
        {
            Console.Clear();
            Console.WriteLine("INTERESTES LIST");

            try
            {
                var interestes = await _interestRepository.GetAllAsync();

                if (!interestes.Any())
                {
                    MainMenu.ShowMessage("\nNo interestes registered.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\n{0,-5} {1,-12}",
                        "ID", "Description");

                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(new string('-', 50));
                    Console.ResetColor();

                    foreach (var interest in interestes)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("{0,-5} {1,-12}",
                            interest.Id,
                            interest.Description);
                    }

                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(new string('-', 50));
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error listing interest: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task CreateInterest()
        {
            Console.Clear();
            Console.WriteLine("REGISTER NEW INTEREST");

            try
            {
                string nombre = MainMenu.ReadText("\nDescription interest: ").Trim();
                if (string.IsNullOrEmpty(nombre))
                {
                    MainMenu.ShowMessage("Description interest cannot be empty.", ConsoleColor.Red);
                    return;
                }

                var interest = new Interest
                {
                    Description = nombre
                };

                Console.Clear();
                Console.WriteLine("INTEREST INFORMATION");

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Description: {nombre}");

                string confirm = MainMenu.ReadText("\nDo you want to register this Interest? (Y/N): ");
                if (confirm.ToUpper() == "Y")
                {
                    bool result = await _interestRepository.InsertAsync(interest);

                    if (result)
                    {
                        MainMenu.ShowMessage("\n✅ Interest registered successfully.", ConsoleColor.Green);
                    }
                    else
                    {
                        MainMenu.ShowMessage("\n❌ Interest registration failed.", ConsoleColor.Red);
                    }
                }
                else
                {
                    MainMenu.ShowMessage("\n⚠️ Operation cancelled.", ConsoleColor.Yellow);
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error registering interest: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task UpdateInterest()
        {
            Console.Clear();
            Console.WriteLine("UPDATE INTEREST");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter interest ID to update: ");
                var interest = await _interestRepository.GetByIdAsync(id);

                if (interest == null)
                {
                    MainMenu.ShowMessage("\n❌ The interest doesn't exist", ConsoleColor.Red);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nCurrent Information:");
                    Console.WriteLine($"Interest ID: {interest.Id}");
                    Console.WriteLine($"Description: {interest.Description}");
                    Console.ResetColor();
                    
                    string nombre = MainMenu.ReadText($"Enter new name interest ({interest.Description}): ");
                    if (!string.IsNullOrWhiteSpace(nombre))
                    {
                        interest.Description = nombre;
                    }

                    Console.Clear();
                    Console.WriteLine("UPDATED INTEREST INFORMATION");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"ID: {interest.Id}");
                    Console.WriteLine($"Description: {interest.Description}");
                    Console.ResetColor();

                    string confirm = MainMenu.ReadText("\nDo you want to save these changes? (Y/N): ");
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _interestRepository.UpdateAsync(interest);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ interest updated successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to update the interest.", ConsoleColor.Red);
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
                MainMenu.ShowMessage($"\n❌ Error updating the interest: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task DeleteInterest()
        {
            Console.Clear();
            Console.WriteLine("DELETE INTEREST");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter the Interest ID to delete: ");
                var interest = await _interestRepository.GetByIdAsync(id);

                if (interest == null)
                {
                    MainMenu.ShowMessage("\n❌ The Interest does not exist.", ConsoleColor.Red);
                }
                else 
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nInterest Information:");
                    Console.WriteLine($"ID: {interest.Id}");
                    Console.WriteLine($"Description: {interest.Description}");
                    Console.ResetColor();
                    
                    string confirm = MainMenu.ReadText("\n⚠️ Are you sure you want to delete this Interest? (Y/N): ");
                    
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _interestRepository.DeleteAsync(id);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ interest deleted successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to delete the interest.", ConsoleColor.Red);
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
                MainMenu.ShowMessage($"\n❌ Error deleting the interest: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}