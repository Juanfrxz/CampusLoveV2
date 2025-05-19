using System;
using System.Threading.Tasks;
using System.Linq;
using CampusLove.Infrastructure.Repositories;
using CampusLove.Infrastructure.Configuration;
using MySql.Data.MySqlClient;
using CampusLove.Application.UI;
using CampusLove.Domain.Entities;
using Figgle;
using Spectre.Console;

namespace CampusLove.Application.UI
{
    public class StatisticsMenu
    {
        private readonly StatisticsRepository _statisticsRepository;

        public StatisticsMenu(MySqlConnection connection)
        {
            _statisticsRepository = new StatisticsRepository(connection);
        }

        public async Task ShowStatistics()
        {
            bool returnToMain = false;
            while (!returnToMain)
            {
                Console.Clear();
                Console.OutputEncoding = System.Text.Encoding.UTF8;

                var title = new FigletText("📊 STATISTICS")
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
                    .Title("[bold blue]Select a statistic to view:[/]")
                    .PageSize(7)
                    .AddChoices(new[]
                    {
                        "👑 User with Most Likes",
                        "😢 User with Least Likes",
                        "🎯 Most Popular Interests",
                        "👥 Matches by Gender",
                        "↩️ Return to Main Menu"
                    });

                var option = AnsiConsole.Prompt(menu);

                try
                {
                    switch (option)
                    {
                        case "👑 User with Most Likes":
                            await ShowUserWithMostLikes();
                            break;
                        case "😢 User with Least Likes":
                            await ShowUserWithLeastLikes();
                            break;
                        case "🎯 Most Popular Interests":
                            await ShowMostPopularInterests();
                            break;
                        case "👥 Matches by Gender":
                            await ShowMatchesByGender();
                            break;
                        case "↩️ Return to Main Menu":
                            returnToMain = true;
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

        private async Task ShowUserWithMostLikes()
        {
            Console.Clear();
            var title = new FigletText("👑 MOST LIKED USER")
                .Centered()
                .Color(Color.Gold1);

            var panel = new Panel(title)
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 1, 1, 1),
                Header = new PanelHeader(" 💞 CampusLove 💞 ", Justify.Center),
            };

            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();

            var user = await _statisticsRepository.GetUserWithMostLikes();
            if (user == null)
            {
                MainMenu.ShowMessage("\nNo users with likes found.", ConsoleColor.Yellow);
            }
            else
            {
                var table = new Table();
                table.Border(TableBorder.Rounded);
                table.BorderColor(Color.Gold1);
                table.Title = new TableTitle("[bold gold1]User with Most Likes[/]");

                table.AddColumn(new TableColumn("[bold cyan]Username[/]").LeftAligned());
                table.AddColumn(new TableColumn("[bold cyan]Total Likes[/]").Centered());

                table.AddRow(
                    $"[white]{user.Username}[/]",
                    $"[green]{user.TotalLikes}[/]"
                );

                AnsiConsole.Write(table);
            }

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            Console.ReadKey();
        }

        private async Task ShowUserWithLeastLikes()
        {
            Console.Clear();
            var title = new FigletText("😢 LEAST LIKED USER")
                .Centered()
                .Color(Color.Orange3);

            var panel = new Panel(title)
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 1, 1, 1),
                Header = new PanelHeader(" 💞 CampusLove 💞 ", Justify.Center),
            };

            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();

            var user = await _statisticsRepository.GetUserLessLikes();
            if (user == null)
            {
                MainMenu.ShowMessage("\nNo users with likes found.", ConsoleColor.Yellow);
            }
            else
            {
                var table = new Table();
                table.Border(TableBorder.Rounded);
                table.BorderColor(Color.Orange3);
                table.Title = new TableTitle("[bold orange3]User with Least Likes[/]");

                table.AddColumn(new TableColumn("[bold cyan]Username[/]").LeftAligned());
                table.AddColumn(new TableColumn("[bold cyan]Total Likes[/]").Centered());

                table.AddRow(
                    $"[white]{user.Username}[/]",
                    $"[yellow]{user.TotalLikes}[/]"
                );

                AnsiConsole.Write(table);
            }

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            Console.ReadKey();
        }

        private async Task ShowMostPopularInterests()
        {
            Console.Clear();
            var title = new FigletText("🎯 POPULAR INTERESTS")
                .Centered()
                .Color(Color.Purple);

            var panel = new Panel(title)
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 1, 1, 1),
                Header = new PanelHeader(" 💞 CampusLove 💞 ", Justify.Center),
            };

            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();

            var interests = await _statisticsRepository.GetMostPopularInterests();
            if (!interests.Any())
            {
                MainMenu.ShowMessage("\nNo interests found.", ConsoleColor.Yellow);
            }
            else
            {
                var table = new Table();
                table.Border(TableBorder.Rounded);
                table.BorderColor(Color.Purple);
                table.Title = new TableTitle("[bold purple]Top 5 Most Popular Interests[/]");

                table.AddColumn(new TableColumn("[bold cyan]Interest[/]").LeftAligned());
                table.AddColumn(new TableColumn("[bold cyan]Users Count[/]").Centered());

                foreach (var interest in interests)
                {
                    table.AddRow(
                        $"[white]{interest.Description}[/]",
                        $"[green]{interest.UsersCount}[/]"
                    );
                }

                AnsiConsole.Write(table);
            }

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            Console.ReadKey();
        }

        private async Task ShowMatchesByGender()
        {
            Console.Clear();
            var title = new FigletText("👥 MATCHES BY GENDER")
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

            var matches = await _statisticsRepository.GetMatchesByGender();
            if (!matches.Any())
            {
                MainMenu.ShowMessage("\nNo matches found.", ConsoleColor.Yellow);
            }
            else
            {
                var table = new Table();
                table.Border(TableBorder.Rounded);
                table.BorderColor(Color.Blue);
                table.Title = new TableTitle("[bold blue]Matches Distribution by Gender[/]");

                table.AddColumn(new TableColumn("[bold cyan]Gender[/]").LeftAligned());
                table.AddColumn(new TableColumn("[bold cyan]Total Matches[/]").Centered());

                foreach (var match in matches)
                {
                    table.AddRow(
                        $"[white]{match.Gender}[/]",
                        $"[green]{match.MatchCount}[/]"
                    );
                }

                AnsiConsole.Write(table);
            }

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            Console.ReadKey();
        }
    }
} 