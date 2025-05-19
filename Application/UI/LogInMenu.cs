using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using CampusLove.Infrastructure.Repositories;
using CampusLove.Infrastructure.Configuration;
using MySql.Data.MySqlClient;
using Spectre.Console;

namespace CampusLove.Application.UI
{
    public class LogInMenu
    {
        private readonly UserRepository _userRepository;
        private readonly SettingsMenu _settingsMenu;
        private readonly ProfileRepository _profileRepository;
        private readonly ReactionRepository _reactionRepository;
        private readonly UserMatchRepository _usermatchRepository;
        private readonly InteractWithProfilesMenu _interactMenu;
        private readonly ViewProfilesMenu _viewprofilesMenu;
        private readonly ViewMatchesMenu _viewMatchesMenu;
        private readonly PurchaseLikesMenu _purchaseLikesMenu;

        public LogInMenu(MySqlConnection connection)
        {
            _userRepository = new UserRepository(connection);
            _profileRepository = new ProfileRepository(connection);
            _reactionRepository = new ReactionRepository(connection);
            _usermatchRepository = new UserMatchRepository(connection);
            _settingsMenu = new SettingsMenu(connection);
            _interactMenu = new InteractWithProfilesMenu(connection);
            _viewprofilesMenu = new ViewProfilesMenu(connection);
            _viewMatchesMenu = new ViewMatchesMenu(connection);
            _purchaseLikesMenu = new PurchaseLikesMenu(connection);
        }

        public async Task ValidateUser()
        {
            bool loginSuccessful = false;

            while (!loginSuccessful)
            {
                Console.Clear();
                MainMenu.ShowHeader(" 👥 LOG IN");
                Console.WriteLine("\nPress TAB to toggle password visibility");

                try
                {
                    string username = MainMenu.ReadText("\nUsername: ").Trim();
                    string password = MainMenu.ReadSecurePassword("Password: ").Trim();

                    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    {
                        MainMenu.ShowMessage("❌ Fields cannot be empty.", ConsoleColor.Red);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("\nPress any key to continue... (ESC to return to menu)");
                        Console.ResetColor();

                        var key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Escape)
                        {
                            return;
                        }
                        continue;
                    }

                    var user = await _userRepository.GetByUsernameAsync(username);
                    if (user == null)
                    {
                        MainMenu.ShowMessage("❌ User not found.", ConsoleColor.Red);
                        Console.ForegroundColor = ConsoleColor.Yellow;

                        Console.Write("\nPress any key to continue... (ESC to return to menu)");
                        Console.ResetColor();

                        var key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Escape)
                        {
                            return;
                        }

                        continue;
                    }

                    if (user.Password != password)
                    {
                        MainMenu.ShowMessage("❌ Incorrect password.", ConsoleColor.Red);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("\nPress any key to continue... (ESC to return to menu)");
                        Console.ResetColor();

                        var key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Escape)
                        {
                            return;
                        }
                        continue;
                    }

                    MainMenu.ShowMessage($"\n✅ Welcome {user.Username}!", ConsoleColor.Green);
                    loginSuccessful = true;
                    await ShowUserMenu(user);
                }
                catch (Exception ex)
                {
                    MainMenu.ShowMessage($"\n❌ Error during login: {ex.Message}", ConsoleColor.Red);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("\nPress any key to continue... (ESC to return to menu)");
                    Console.ResetColor();

                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Escape)
                    {
                        return;
                    }
                }
            }
        }

        public async Task ShowUserMenu(User currentUser)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool returnToMain = false;

            while (!returnToMain)
            {
                Console.Clear();
                var title = new FigletText($"👤 USER MENU ")
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
                    .PageSize(6)
                    .AddChoices(new[]
                    {
                "👥  View Profiles",
                "😍  Interact with Profiles",
                "💞  View Matches",
                "💳  Buy likes",
                "⚙️   Settings",
                "❌  Logout"
                    });

                var option = AnsiConsole.Prompt(menu);

                try
                {
                    switch (option)
                    {
                        case "👥  View Profiles":
                            await _viewprofilesMenu.ShowMenu(currentUser);
                            break;
                        case "😍  Interact with Profiles":
                            await _interactMenu.ShowMenu(currentUser);
                            break;
                        case "💞  View Matches":
                            await _viewMatchesMenu.ShowMenu(currentUser);
                            break;
                        case "💳  Buy likes":
                            await _purchaseLikesMenu.ShowMenu(currentUser);
                            break;
                        case "⚙️   Settings":
                            await _settingsMenu.ShowMenu(currentUser);
                            break;
                        case "❌  Logout":
                            returnToMain = true;
                            var logoutPanel = new Panel("[blue]👋 Logging out...[/]")
                            {
                                Border = BoxBorder.Rounded,
                                BorderStyle = new Style(Color.Blue),
                                Padding = new Padding(1, 1, 1, 1)
                            };
                            AnsiConsole.Write(logoutPanel);
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
        }

    }
}