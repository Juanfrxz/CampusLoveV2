using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using CampusLove.Infrastructure.Repositories;
using MySql.Data.MySqlClient;
using Spectre.Console;
using StatusEntity = CampusLove.Domain.Entities.Status;

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

                var title = new FigletText("👩‍❤️‍👩 STATUS MENU")
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

                var menu = new SelectionPrompt<string>()
                    .Title("[bold blue]Select an option:[/]")
                    .PageSize(5)
                    .AddChoices(new[]
                    {
                        "📋  List Status",
                        "➕  Create Status",
                        "✏️   Update Status",
                        "✖️   Delete Status",
                        "↩️   Return to Admin Menu"
                    });

                var option = AnsiConsole.Prompt(menu);

                switch (option)
                {
                    case "📋  List Status":
                        ListStatus().Wait();
                        break;
                    case "➕  Create Status":
                        CreateStatus().Wait();
                        break;
                    case "✏️   Update Status":
                        UpdateStatus().Wait();
                        break;
                    case "✖️   Delete Status":
                        DeleteStatus().Wait();
                        break;
                    case "↩️   Return to Admin Menu":
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
            MainMenu.ShowText("STATUS LIST");

            try
            {
                var statuses = await _statusRepository.GetAllAsync();

                if (!statuses.Any())
                {
                    MainMenu.ShowMessage("\nNo statuses registered.", ConsoleColor.Yellow);
                }
                else
                {
                    var table = new Table();
                    
                    table.Border(TableBorder.Rounded);
                    table.BorderColor(Color.White);
                    table.Title("[bold magenta]Gender List[/]");
                    
                    table.AddColumn(new TableColumn("[bold cyan]ID[/]").Centered());
                    table.AddColumn(new TableColumn("[bold cyan]Description[/]").LeftAligned());

                    foreach (var status in statuses)
                    {
                        table.AddRow(
                            $"[white]{status.Id}[/]",
                            $"[white]{status.Description}[/]"
                        );
                    }

                    AnsiConsole.Write(table);
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
            MainMenu.ShowText("REGISTER NEW STATUS");

            try
            {
                string nombre = MainMenu.ReadText("\nDescription status: ").Trim();
                if (string.IsNullOrEmpty(nombre))
                {
                    MainMenu.ShowMessage("Description status cannot be empty.", ConsoleColor.Red);
                    return;
                }

                var status = new StatusEntity
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
            MainMenu.ShowText("UPDATE STATUS");
            
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
            MainMenu.ShowText("DELETE STATUS");
            
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