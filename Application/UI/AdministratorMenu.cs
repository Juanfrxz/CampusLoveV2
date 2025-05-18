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
    public class AdministratorMenu
    {
        private readonly AdministratorRepository _administratorRepository;

        public AdministratorMenu(MySqlConnection connection)
        {
            _administratorRepository = new AdministratorRepository(connection);
        }

        public void ShowMenu()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool returnTo = false;

            while (!returnTo)
            {
                Console.Clear();

                // Título con Figlet y Panel, igual que MainMenu
                var title = new FigletText("📱 ADMINISTRATOR MENU")
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

                // Menú interactivo con SelectionPrompt
                var menu = new SelectionPrompt<string>()
                    .Title("[bold blue]Selecciona una opción:[/]")
                    .PageSize(6)
                    .AddChoices(new[]
                    {
                        "📋  Listar Administradores",
                        "➕  Crear Administrador",
                        "✏️   Actualizar Administrador",
                        "✖️   Eliminar Administrador",
                        "↩️   Volver al menú anterior"
                    });

                var option = AnsiConsole.Prompt(menu);

                switch (option)
                {
                    case "📋  Listar Administradores":
                        ListAdministrator().Wait();
                        break;
                    case "➕  Crear Administrador":
                        CreateAdministrator().Wait();
                        break;
                    case "✏️   Actualizar Administrador":
                        UpdateAdministrator().Wait();
                        break;
                    case "✖️   Eliminar Administrador":
                        DeleteAdministrator().Wait();
                        break;
                    case "↩️   Volver al menú anterior":
                        returnTo = true;
                        break;
                    default:
                        MainMenu.ShowMessage("⚠️ Opción inválida. Intenta de nuevo.", ConsoleColor.Red);
                        Console.ReadKey();
                        break;
                }
            }

            MainMenu.ShowMessage("\n👋 ¡Gracias por usar la aplicación! ¡Que tengas un gran día! 🌟", ConsoleColor.Green);
        }

        private async Task ListAdministrator()
        {
            Console.Clear();
            MainMenu.ShowText("📋 ADMINISTRATOR LIST");

            try
            {
                var administrators = await _administratorRepository.GetAllAsync();

                if (!administrators.Any())
                {
                    MainMenu.ShowMessage("\nNo administrators registered.", ConsoleColor.Yellow);
                }
                else
                {
                    var table = new Table();
                    
                    table.Border(TableBorder.Rounded);
                    table.BorderColor(Color.White);
                    table.Title("[bold magenta]Gender List[/]");
                    
                    table.AddColumn(new TableColumn("[bold cyan]ID[/]").Centered());
                    table.AddColumn(new TableColumn("[bold cyan]NAME[/]").LeftAligned());
                    table.AddColumn(new TableColumn("[bold cyan]LASTNAME[/]").LeftAligned());
                    table.AddColumn(new TableColumn("[bold cyan]IDENTIFICATION[/]").LeftAligned());
                    table.AddColumn(new TableColumn("[bold cyan]USERNAME[/]").LeftAligned());

                    foreach (var administrator in administrators)
                    {
                        table.AddRow(
                            $"[white]{administrator.Id}[/]",
                            $"[white]{administrator.Name}[/]",
                            $"[white]{administrator.LastName}[/]",
                            $"[white]{administrator.Identification}[/]",
                            $"[white]{administrator.Username}[/]"
                        );
                    }

                    AnsiConsole.Write(table);
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error listing administrators: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task CreateAdministrator()
        {
            Console.Clear();
            MainMenu.ShowText("REGISTER NEW ADMINISTRATOR");

            try
            {
                string nombre = MainMenu.ReadText("\nName: ").Trim();
                if (string.IsNullOrEmpty(nombre))
                {
                    MainMenu.ShowMessage("Name cannot be empty.", ConsoleColor.Red);
                    return;
                }

                string apellido = MainMenu.ReadText("\nLastName: ").Trim();
                if (string.IsNullOrEmpty(apellido))
                {
                    MainMenu.ShowMessage("LastName cannot be empty.", ConsoleColor.Red);
                    return;
                }

                string identificacion = MainMenu.ReadText("\nIdentification: ").Trim();
                if (string.IsNullOrEmpty(identificacion))
                {
                    MainMenu.ShowMessage("Idetification cannot be empty.", ConsoleColor.Red);
                    return;
                }

                string username = MainMenu.ReadText("\nUsername: ").Trim();
                if (string.IsNullOrEmpty(username))
                {
                    MainMenu.ShowMessage("Username cannot be empty.", ConsoleColor.Red);
                    return;
                }

                string password = MainMenu.ReadText("\nPassword: ").Trim();
                if (string.IsNullOrEmpty(password))
                {
                    MainMenu.ShowMessage("Password cannot be empty.", ConsoleColor.Red);
                    return;
                }

                var administrator = new Administrator
                {
                    Name = nombre,
                    LastName = apellido,
                    Identification = identificacion,
                    Username = username,
                    Password = password,
                    ApplicationId = 1
                };

                Console.Clear();
                MainMenu.ShowText("ADMNISTRATOR INFORMATION");

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Name: {nombre}");
                Console.WriteLine($"Lastname: {apellido}");
                Console.WriteLine($"Identification: {identificacion}");
                Console.WriteLine($"Username: {username}");

                string confirm = MainMenu.ReadText("\nDo you want to register this administrator? (Y/N): ");
                if (confirm.ToUpper() == "Y")
                {
                    bool result = await _administratorRepository.InsertAsync(administrator);

                    if (result)
                    {
                        MainMenu.ShowMessage("\n✅ Administrator registered successfully.", ConsoleColor.Green);
                    }
                    else
                    {
                        MainMenu.ShowMessage("\n❌ Administrator registration failed.", ConsoleColor.Red);
                    }
                }
                else
                {
                    MainMenu.ShowMessage("\n⚠️ Operation cancelled.", ConsoleColor.Yellow);
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error registering administrator: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task UpdateAdministrator()
        {
            Console.Clear();
            MainMenu.ShowText("UPDATE ADMINISTRATOR");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter administrator ID to update: ");
                var administrator = await _administratorRepository.GetByIdAsync(id);

                if (administrator == null)
                {
                    MainMenu.ShowMessage("\n❌ The administrator doesn't exist", ConsoleColor.Red);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    MainMenu.ShowText($"\nCurrent Information:");
                    Console.WriteLine($"Name: {administrator.Name}");
                    Console.WriteLine($"Lastname: {administrator.LastName}");
                    Console.WriteLine($"Identification: {administrator.Identification}");
                    Console.WriteLine($"Username: {administrator.Username}");
                    Console.ResetColor();

                    string nombre = MainMenu.ReadText($"\nName ({administrator.Name}): ").Trim();
                    if (!string.IsNullOrWhiteSpace(nombre))
                    {
                        administrator.Name = nombre;
                    }

                    string apellido = MainMenu.ReadText($"\nLastName ({administrator.LastName}): ").Trim();
                    if (!string.IsNullOrWhiteSpace(apellido))
                    {
                        administrator.LastName = apellido;
                    }

                    string identificacion = MainMenu.ReadText($"\nIdentification ({administrator.Identification}): ").Trim();
                    if (!string.IsNullOrWhiteSpace(identificacion))
                    {
                        administrator.Identification = identificacion;
                    }

                    string username = MainMenu.ReadText($"\nUsername ({administrator.Username}): ").Trim();
                    if (!string.IsNullOrWhiteSpace(username))
                    {
                        administrator.Username = username;
                    }

                    string password = MainMenu.ReadText("\nNew Password (leave empty to keep current): ").Trim();
                    if (!string.IsNullOrWhiteSpace(password))
                    {
                        administrator.Password = password;
                    }

                    Console.Clear();
                    Console.WriteLine("UPDATED ADMINISTRATOR INFORMATION");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"Name: {administrator.Name}");
                    Console.WriteLine($"Lastname: {administrator.LastName}");
                    Console.WriteLine($"Identification: {administrator.Identification}");
                    Console.WriteLine($"Username: {administrator.Username}");
                    Console.WriteLine($"Password: {"*".PadRight(password?.Length ?? 0, '*')}");

                    string confirm = MainMenu.ReadText("\nDo you want to update this administrator? (Y/N): ");
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _administratorRepository.UpdateAsync(administrator);

                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ Administrator updated successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Administrator update failed.", ConsoleColor.Red);
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
                MainMenu.ShowMessage($"\n❌ Error updating administrator: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task DeleteAdministrator()
        {
            Console.Clear();
            MainMenu.ShowText("DELETE ADMINISTRATOR");

            try
            {
                int id = MainMenu.ReadInteger("\nEnter administrator ID to delete: ");
                var administrator = await _administratorRepository.GetByIdAsync(id);

                if (administrator == null)
                {
                    MainMenu.ShowMessage("\n❌ The administrator doesn't exist", ConsoleColor.Red);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nAdministrator Information:");
                    Console.WriteLine($"Name: {administrator.Name}");
                    Console.WriteLine($"Lastname: {administrator.LastName}");
                    Console.WriteLine($"Identification: {administrator.Identification}");
                    Console.WriteLine($"Username: {administrator.Username}");
                    Console.ResetColor();

                    string confirm = MainMenu.ReadText("\nAre you sure you want to delete this administrator? (Y/N): ");
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _administratorRepository.DeleteAsync(id);

                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ Administrator deleted successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Administrator deletion failed.", ConsoleColor.Red);
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
                MainMenu.ShowMessage($"\n❌ Error deleting administrator: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}