using System;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using CampusLove.Infrastructure.Repositories;
using MySql.Data.MySqlClient;
using Spectre.Console;

namespace CampusLove.Application.UI
{
    public class PurchaseLikesMenu
    {
        private readonly UserRepository _userRepository;

        public PurchaseLikesMenu(MySqlConnection connection)
        {
            _userRepository = new UserRepository(connection);
        }

        public async Task ShowMenu(User currentUser)
        {
            bool returnToMenu = false;
            while (!returnToMenu)
            {
                Console.Clear();
                var title = new FigletText("💳 BUY LIKES")
                    .Centered()
                    .Color(Color.Green);

                var panel = new Panel(title)
                {
                    Border = BoxBorder.Rounded,
                    Padding = new Padding(1, 1, 1, 1),
                    Header = new PanelHeader(" 💞 CampusLove 💞 ", Justify.Center),
                };
                AnsiConsole.Write(panel);
                AnsiConsole.WriteLine();

                var menu = new SelectionPrompt<string>()
                    .Title("[bold green]Select a package of likes:[/]")
                    .PageSize(5)
                    .AddChoices(new[]
                    {
                        "5 Likes - $1",
                        "10 Likes - $1.8",
                        "20 Likes - $3",
                        "↩️ Back to menu"
                    });
                var option = AnsiConsole.Prompt(menu);
                int likesToAdd = 0;
                switch (option)
                {
                    case "5 Likes - $1":
                        likesToAdd = 5;
                        break;
                    case "10 Likes - $1.8":
                        likesToAdd = 10;
                        break;
                    case "20 Likes - $3":
                        likesToAdd = 20;
                        break;
                    case "↩️ Back to menu":
                        returnToMenu = true;
                        continue;
                }

                // Simulación de pago
                string cardNumber = MainMenu.ReadText("\nEnter your credit/debit card number: ").Trim();
                string cardType = GetCardType(cardNumber);
                string cardEmoji = GetCardEmoji(cardType);
                MainMenu.ShowMessage($"\nDetected: {cardType} {cardEmoji}", ConsoleColor.Cyan);
                Console.Write("\nPress Enter to simulate payment...");
                Console.ReadKey(true);

                // Simulated processing
                AnsiConsole.Markup("[green]Processing payment...[/]");
                await Task.Delay(1000);
                AnsiConsole.MarkupLine(" [green]✔[/]");

                // Actualizar bonus likes
                currentUser.BonusLikes += likesToAdd;
                await _userRepository.UpdateAsync(currentUser);

                MainMenu.ShowMessage($"\n✅ Purchase successful! +{likesToAdd} bonus likes.", ConsoleColor.Green);
                Console.Write("\nPress any key to continue...");
                Console.ReadKey(true);
            }
        }

        private string GetCardType(string number)
        {
            if (string.IsNullOrWhiteSpace(number)) return "Unknown";
            if (number.StartsWith("4")) return "Visa";
            if (number.StartsWith("5")) return "MasterCard";
            if (number.StartsWith("34") || number.StartsWith("37")) return "American Express";
            if (number.StartsWith("6")) return "Discover";
            return "Unknown";
        }

        private string GetCardEmoji(string cardType)
        {
            return cardType switch
            {
                "Visa" => "💳",
                "MasterCard" => "💳",
                "American Express" => "💳",
                "Discover" => "💳",
                _ => ""
            };
        }
    }
} 