using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using CampusLove.Infrastructure.Repositories;
using MySql.Data.MySqlClient;
using Spectre.Console;

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

                var title = new FigletText("⚧️ GENDER MENU")
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
                        "📋  List Gender",
                        "➕  Create Gender",
                        "✏️   Update Gender",
                        "✖️   Delete Gender",
                        "↩️   Return to Admin Menu"
                    });

                var option = AnsiConsole.Prompt(menu);

                switch (option)
                {
                    case "📋  List Gender":
                        ListGender().Wait();
                        break;
                    case "➕  Create Gender":
                        CreateGender().Wait();
                        break;
                    case "✏️   Update Gender":
                        UpdateGender().Wait();
                        break;
                    case "✖️   Delete Gender":
                        DeleteGender().Wait();
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

        private async Task ListGender()
        {
            Console.Clear();
            MainMenu.ShowText("GENDER LIST");

            try
            {
                var genders = await _genderRepository.GetAllAsync();

                if (!genders.Any())
                {
                    MainMenu.ShowMessage("\nNo genders registered.", ConsoleColor.Yellow);
                }
                else
                {
                    var table = new Table();

                    table.Border(TableBorder.Rounded);
                    table.BorderColor(Color.White);
                    table.Title("[bold magenta]Gender List[/]");
                    
                    table.AddColumn(new TableColumn("[bold cyan]ID[/]").Centered());
                    table.AddColumn(new TableColumn("[bold cyan]Description[/]").LeftAligned());
                    
                    // Add rows
                    foreach (var gender in genders)
                    {
                        table.AddRow(
                            $"[white]{gender.Id}[/]",
                            $"[white]{gender.Description}[/]"
                        );
                    }
                    
                    // Render the table
                    AnsiConsole.Write(table);
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
            MainMenu.ShowText("REGISTER NEW GENDER");

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
                MainMenu.ShowText("GENDER INFORMATION");

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
            MainMenu.ShowText("UPDATE GENDER");
            
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
                    MainMenu.ShowText($"\nCurrent Information:");
                    Console.WriteLine($"Gender ID: {gender.Id}");
                    Console.WriteLine($"Description: {gender.Description}");
                    Console.ResetColor();
                    
                    string nombre = MainMenu.ReadText($"Enter new gender ({gender.Description}): ");
                    if (!string.IsNullOrWhiteSpace(nombre))
                    {
                        gender.Description = nombre;
                    }

                    Console.Clear();
                    MainMenu.ShowText("UPDATED GENDER INFORMATION");
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
            MainMenu.ShowText("DELETE GENDER");
            
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
                    MainMenu.ShowText($"\nGender Information:");
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