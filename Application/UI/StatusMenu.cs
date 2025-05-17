using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using CampusLove.Infrastructure.Repositories;
using MySql.Data.MySqlClient;

namespace CampusLove.Application.UI
{
    public class StatusMenu
    {
        private readonly StatusRepository _statusRepository;

        public StatusMenu(MySqlConnection connection)
        {
            _statusRepository = new StatusRepository(connection);
        }

        public void ShowMenu()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool returnTo = false; 

            while (!returnTo)
            {
                Console.Clear();
                Console.WriteLine(" 👩‍❤️‍👩 STATUS MENU   ");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("  ╔════════════════════════════════════════════╗");
                Console.WriteLine("  ║                👩‍❤️‍👩  STATUS MENU          ║");
                Console.WriteLine("  ╠════════════════════════════════════════════╣");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  ║       1️⃣  List Status              📋       ║");
                Console.WriteLine("  ║       2️⃣  Create Status            ➕       ║");
                Console.WriteLine("  ║       3️⃣  Update Status            ✏️        ║");
                Console.WriteLine("  ║       4️⃣  Delete Status            ✖️        ║");
                Console.WriteLine("  ║       0️⃣  Return to Admin Menu     ↩️        ║");
                Console.WriteLine("  ╚════════════════════════════════════════════╝");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                string option = MainMenu.ReadText("\n✨ Select an option: ");

                switch (option)
                {
                    case "1":
                        ListStatus().Wait();
                        break;
                    case "2":
                        CreateStatus().Wait();
                        break;
                    case "3":
                        UpdateStatus().Wait();
                        break;
                    case "4":
                        DeleteStatus().Wait();
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

        private async Task ListStatus()
        {
            Console.Clear();
            Console.WriteLine("STATUS LIST");

            try
            {
                var statuses = await _statusRepository.GetAllAsync();

                if (!statuses.Any())
                {
                    MainMenu.ShowMessage("\nNo statuses registered.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\n{0,-5} {1,-12}",
                        "ID", "Description");

                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(new string('-', 50));
                    Console.ResetColor();

                    foreach (var status in statuses)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("{0,-5} {1,-12}",
                        status.Id,
                        status.Description);
                    }

                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(new string('-', 50));
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error listing status: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task CreateStatus()
        {
            Console.Clear();
            Console.WriteLine("REGISTER NEW STATUS");

            try
            {
                string nombre = MainMenu.ReadText("\nDescription status: ").Trim();
                if (string.IsNullOrEmpty(nombre))
                {
                    MainMenu.ShowMessage("Description status cannot be empty.", ConsoleColor.Red);
                    return;
                }

                var status = new Status
                {
                    Description = nombre
                };

                Console.Clear();
                Console.WriteLine("STATUS INFORMATION");

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Description: {nombre}");

                string confirm = MainMenu.ReadText("\nDo you want to register this status? (Y/N): ");
                if (confirm.ToUpper() == "Y")
                {
                    bool result = await _statusRepository.InsertAsync(status);

                    if (result)
                    {
                        MainMenu.ShowMessage("\n✅ Status registered successfully.", ConsoleColor.Green);
                    }
                    else
                    {
                        MainMenu.ShowMessage("\n❌ Status registration failed.", ConsoleColor.Red);
                    }
                }
                else
                {
                    MainMenu.ShowMessage("\n⚠️ Operation cancelled.", ConsoleColor.Yellow);
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error registering status: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task UpdateStatus()
        {
            Console.Clear();
            Console.WriteLine("UPDATE STATUS");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter status ID to update: ");
                var status = await _statusRepository.GetByIdAsync(id);

                if (status == null)
                {
                    MainMenu.ShowMessage("\n❌ The status doesn't exist", ConsoleColor.Red);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nCurrent Information:");
                    Console.WriteLine($"Status ID: {status.Id}");
                    Console.WriteLine($"Description: {status.Description}");
                    Console.ResetColor();
                    
                    string nombre = MainMenu.ReadText($"Enter new status ({status.Description}): ");
                    if (!string.IsNullOrWhiteSpace(nombre))
                    {
                        status.Description = nombre;
                    }

                    Console.Clear();
                    Console.WriteLine("UPDATED STATUS INFORMATION");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"ID: {status.Id}");
                    Console.WriteLine($"Description: {status.Description}");
                    Console.ResetColor();

                    string confirm = MainMenu.ReadText("\nDo you want to save these changes? (Y/N): ");
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _statusRepository.UpdateAsync(status);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ Status updated successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to update the status.", ConsoleColor.Red);
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
                MainMenu.ShowMessage($"\n❌ Error updating the status: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task DeleteStatus()
        {
            Console.Clear();
            Console.WriteLine("DELETE STATUS");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter the status ID to delete: ");
                var status = await _statusRepository.GetByIdAsync(id);

                if (status == null)
                {
                    MainMenu.ShowMessage("\n❌ The status does not exist.", ConsoleColor.Red);
                }
                else 
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\n Status Information:");
                    Console.WriteLine($"ID:  {status.Id}");
                    Console.WriteLine($"Description:  {status.Description}");
                    Console.ResetColor();
                    
                    string confirm = MainMenu.ReadText("\n⚠️ Are you sure you want to delete this status? (Y/N): ");
                    
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _statusRepository.DeleteAsync(id);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ Status deleted successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to delete the status.", ConsoleColor.Red);
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
                MainMenu.ShowMessage($"\n❌ Error deleting the status: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}