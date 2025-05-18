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
            bool returnToMain = false;

            while (!returnToMain)
            {
                Console.Clear();

                var title = new FigletText("🚴 INTERESES")
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
                    .Title("[bold blue]Selecciona una opción:[/]")
                    .PageSize(5)
                    .AddChoices(new[]
                    {
                        "📋  Listar intereses",
                        "➕  Crear interés",
                        "✏️   Actualizar interés",
                        "✖️   Eliminar interés",
                        "↩️   Volver al menú anterior"
                    });

                var option = AnsiConsole.Prompt(menu);

                switch (option)
                {
                    case "📋  Listar intereses":
                        ListInterest().Wait();
                        break;
                    case "➕  Crear interés":
                        CreateInterest().Wait();
                        break;
                    case "✏️   Actualizar interés":
                        UpdateInterest().Wait();
                        break;
                    case "✖️   Eliminar interés":
                        DeleteInterest().Wait();
                        break;
                    case "↩️   Volver al menú anterior":
                        returnToMain = true;
                        break;
                    default:
                        MainMenu.ShowMessage("⚠️ Opción inválida. Intenta de nuevo.", ConsoleColor.Red);
                        Console.ReadKey();
                        break;
                }
            }
        }

        private async Task ListInterest()
        {
            Console.Clear();
            MainMenu.ShowText("INTERESTES LIST");

            try
            {
                var interestes = await _interestRepository.GetAllAsync();

                if (!interestes.Any())
                {
                    MainMenu.ShowMessage("\nNo interestes registered.", ConsoleColor.Yellow);
                }
                else
                {
                    var table = new Table();

                    table.Border(TableBorder.Rounded);
                    table.BorderColor(Color.White);
                    table.Title("[bold magenta]Gender List[/]");
                    
                    table.AddColumn(new TableColumn("[bold cyan]ID[/]").Centered());
                    table.AddColumn(new TableColumn("[bold cyan]Description[/]").LeftAligned());

                    foreach (var interest in interestes)
                    {
                        table.AddRow(
                            $"[white]{interest.Id}[/]",
                            $"[white]{interest.Description}[/]"
                        );
                    }

                    AnsiConsole.Write(table);
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
            MainMenu.ShowText("REGISTER NEW INTEREST");

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
                MainMenu.ShowText("INTEREST INFORMATION");

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
            MainMenu.ShowText("UPDATE INTEREST");
            
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
                    MainMenu.ShowText($"\nCurrent Information:");
                    Console.WriteLine($"Interest ID: {interest.Id}");
                    Console.WriteLine($"Description: {interest.Description}");
                    Console.ResetColor();
                    
                    string nombre = MainMenu.ReadText($"Enter new name interest ({interest.Description}): ");
                    if (!string.IsNullOrWhiteSpace(nombre))
                    {
                        interest.Description = nombre;
                    }

                    Console.Clear();
                    MainMenu.ShowText("UPDATED INTEREST INFORMATION");
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
            MainMenu.ShowText("DELETE INTEREST");
            
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