using System;
using System.Threading.Tasks;
using CampusLove.Infrastructure.Repositories;
using CampusLove.Infrastructure.Configuration;
using MySql.Data.MySqlClient;
using CampusLove.Application.UI;
using CampusLove.Domain.Entities;
using Figgle;
using Spectre.Console;

namespace CampusLove.Application.UI
{
    public class MainMenu
    {
        private readonly SignUpMenu _signupMenu;
        private readonly LogInMenu _loginMenu;
        private readonly LogInAdminMenu _logInAdminMenu;

        public MainMenu()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "  CampusLove ... Where is Love 💞   ";

            var connection = DatabaseConfig.GetConnection();
            _signupMenu = new SignUpMenu(connection);
            _loginMenu = new LogInMenu(connection);
            _logInAdminMenu = new LogInAdminMenu(connection);
        }

        public async Task ShowMenu()
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                var title = new FigletText("💞 CampusLove")
                    .Centered()
                    .Color(Color.Blue);

                var panel = new Panel(title)
                {
                    Border = BoxBorder.Rounded,
                    Padding = new Padding(1, 1, 1, 1),
                    Header = new PanelHeader(" Where is Love 💞 ", Justify.Center),
                };

                AnsiConsole.Write(panel);
                AnsiConsole.WriteLine();

                var menu = new SelectionPrompt<string>()
                    .Title("[bold blue]Select an option:[/]")
                    .PageSize(5)
                    .AddChoices(new[]
                    {
                        "📰  Sign Up",
                        "☑️   Log In",
                        "🔑  Administrator",
                        "❌  Exit"
                    });

                var option = AnsiConsole.Prompt(menu);

                try
                {
                    switch (option)
                    {
                        case "📰  Sign Up":
                            await _signupMenu.RegisterUser();
                            break;
                        case "☑️   Log In":
                            await _loginMenu.ValidateUser();
                            break;
                        case "🔑  Administrator":
                            await _logInAdminMenu.ValidateAdmin();
                            break;
                        case "❌  Exit":
                            exit = true;
                            var exitPanel = new Panel("[blue]👋 Exiting the application...[/]")
                            {
                                Border = BoxBorder.Rounded,
                                BorderStyle = new Style(Color.Blue),
                                Padding = new Padding(1, 1, 1, 1)
                            };
                            AnsiConsole.Write(exitPanel);
                            await Task.Delay(1000);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    var errorPanel = new Panel($"[red]❌ Error: {ex.Message}[/]")
                    {
                        Border = BoxBorder.Rounded,
                        BorderStyle = new Style(Color.Red),
                        Padding = new Padding(1, 1, 1, 1)
                    };
                    AnsiConsole.Write(errorPanel);
                    Console.ReadKey();
                }
            }

            ShowMessage("\n👋 Thank you for using the application! Have a great day! 🌟", ConsoleColor.Blue);
        }

        public static void ShowMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"\n{message}");
            Console.ResetColor();
        }

        public static void ShowHeader(string message)
        {
            var figlet = new FigletText(message)
                .Centered()
                .Color(Color.DarkMagenta);

            var panel = new Panel(figlet)
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 1, 1, 1),
                Header = new PanelHeader(" 💞 CampusLove 💞 ", Justify.Center),
            };

            AnsiConsole.Write(panel);
        }

        public static void ShowTitle(string message)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Rule($"[bold blue on white blink]{message}[/]").Centered());
            AnsiConsole.WriteLine();
        }

        public static void ShowText(string message)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Rule($"[bold purple on white blink]{message}[/]").LeftJustified());
            AnsiConsole.WriteLine();
        }

        public static string ReadText(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine() ?? string.Empty;
        }

        public static string ReadSecurePassword(string prompt)
        {
            int startLeft = Console.CursorLeft;
            int startTop = Console.CursorTop;
            Console.Write(prompt);
            var password = new System.Text.StringBuilder();
            bool showPassword = false;
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Tab)
                {
                    showPassword = !showPassword;
                    Console.SetCursorPosition(startLeft, startTop);
                    Console.Write(prompt);
                    Console.Write(showPassword ? password.ToString() : new string('*', password.Length));
                    continue;
                }

                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.Length--;
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    password.Append(key.KeyChar);
                    Console.Write(showPassword ? key.KeyChar : '*');
                }
            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password.ToString();
        }

        public static int ReadInteger(string prompt)
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(prompt);
                Console.ResetColor();
                if (int.TryParse(Console.ReadLine(), out int value) && value >= 0)
                {
                    return value;
                }

                ShowMessage("❌ Error: Please enter a positive integer.", ConsoleColor.Red);
            }
        }

        public static DateTime ReadDate(string prompt)
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(prompt);
                Console.ResetColor();
                if (DateTime.TryParse(Console.ReadLine(), out DateTime date))
                {
                    return date;
                }

                ShowMessage("❌ Error: Please enter a valid date (DD/MM/YYYY).", ConsoleColor.Red);
            }
        }
    }
}