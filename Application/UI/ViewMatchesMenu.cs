using System;
using System.Threading.Tasks;
using System.Linq;
using CampusLove.Domain.Entities;
using CampusLove.Infrastructure.Repositories;
using MySql.Data.MySqlClient;
using Spectre.Console;

namespace CampusLove.Application.UI
{
    public class ViewMatchesMenu
    {
        private readonly UserMatchRepository _userMatchRepository;
        private readonly UserRepository _userRepository;
        private readonly ProfileRepository _profileRepository;

        public ViewMatchesMenu(MySqlConnection connection)
        {
            _userMatchRepository = new UserMatchRepository(connection);
            _userRepository = new UserRepository(connection);
            _profileRepository = new ProfileRepository(connection);
        }

        public async Task ShowMenu(User currentUser)
        {
            bool returnToMain = false;
            while (!returnToMain)
            {
                Console.Clear();
                var title = new FigletText("💞 VIEW MATCHES")
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

                var menu = new SelectionPrompt<string>()
                    .Title("[bold purple]Select an option:[/]")
                    .PageSize(3)
                    .AddChoices(new[]
                    {
                        "👥 View All Matches",
                        "⏰ View Recent Matches",
                        "↩️ Return to Menu"
                    });

                var option = AnsiConsole.Prompt(menu);

                try
                {
                    switch (option)
                    {
                        case "👥 View All Matches":
                            await ViewAllMatches(currentUser);
                            break;
                        case "⏰ View Recent Matches":
                            await ViewRecentMatches(currentUser);
                            break;
                        case "↩️ Return to Menu":
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

        private async Task ViewAllMatches(User currentUser)
        {
            Console.Clear();
            var title = new FigletText("💞 ALL MATCHES")
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

            try
            {
                var matches = await _userMatchRepository.GetAllAsync();
                var userMatches = matches.Where(m => m.User1_id == currentUser.Id || m.User2_id == currentUser.Id).ToList();

                if (!userMatches.Any())
                {
                    var noMatchesPanel = new Panel("[yellow]No matches found.[/]")
                    {
                        Border = BoxBorder.Rounded,
                        BorderStyle = new Style(Color.Yellow),
                        Padding = new Padding(1, 1, 1, 1)
                    };
                    AnsiConsole.Write(noMatchesPanel);
                }
                else
                {
                    var table = new Table();
                    table.Border(TableBorder.Rounded);
                    table.BorderColor(Color.Purple);
                    table.Title = new TableTitle("[bold purple]Your Matches[/]", new Style(Color.Purple, Color.Default, Decoration.Bold));
                    table.Caption = new TableTitle($"[yellow]Total matches: {userMatches.Count}[/]");

                    table.AddColumn(new TableColumn("[bold cyan]Match Date[/]").Centered());
                    table.AddColumn(new TableColumn("[bold cyan]Matched With[/]").LeftAligned());
                    table.AddColumn(new TableColumn("[bold cyan]Username[/]").LeftAligned());
                    table.AddColumn(new TableColumn("[bold cyan]Status[/]").Centered());

                    foreach (var match in userMatches)
                    {
                        int matchedUserId = match.User1_id == currentUser.Id ? match.User2_id : match.User1_id;
                        var matchedUser = await _userRepository.GetByIdAsync(matchedUserId);
                        if (matchedUser == null) continue;
                        var matchedProfile = await _profileRepository.GetByIdAsync(matchedUser.ProfileId);
                        if (matchedProfile == null) continue;

                        var status = match.MatchDate >= DateTime.Now.AddDays(-1) 
                            ? "[green]New![/]" 
                            : match.MatchDate >= DateTime.Now.AddDays(-7) 
                                ? "[yellow]Recent[/]" 
                                : "[blue]Older[/]";

                        table.AddRow(
                            $"[white]{match.MatchDate:dd/MM/yyyy HH:mm}[/]",
                            $"[white]{matchedProfile.Name} {matchedProfile.LastName}[/]",
                            $"[white]{matchedUser.Username}[/]",
                            status
                        );
                    }

                    AnsiConsole.Write(table);
                }
            }
            catch (Exception ex)
            {
                var errorPanel = new Panel($"[red]❌ Error viewing matches: {ex.Message}[/]")
                {
                    Border = BoxBorder.Rounded,
                    BorderStyle = new Style(Color.Red),
                    Padding = new Padding(1, 1, 1, 1)
                };
                AnsiConsole.Write(errorPanel);
            }

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            Console.ReadKey();
        }

        private async Task ViewRecentMatches(User currentUser)
        {
            Console.Clear();
            var title = new FigletText("⏰ RECENT MATCHES")
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

            try
            {
                var matches = await _userMatchRepository.GetAllAsync();
                var userMatches = matches
                    .Where(m => (m.User1_id == currentUser.Id || m.User2_id == currentUser.Id) &&
                               m.MatchDate >= DateTime.Now.AddDays(-7))
                    .OrderByDescending(m => m.MatchDate)
                    .ToList();

                if (!userMatches.Any())
                {
                    var noMatchesPanel = new Panel("[yellow]No recent matches found in the last 7 days.[/]")
                    {
                        Border = BoxBorder.Rounded,
                        BorderStyle = new Style(Color.Yellow),
                        Padding = new Padding(1, 1, 1, 1)
                    };
                    AnsiConsole.Write(noMatchesPanel);
                }
                else
                {
                    var table = new Table();
                    table.Border(TableBorder.Rounded);
                    table.BorderColor(Color.Blue);
                    table.Title = new TableTitle("[bold blue]Recent Matches (Last 7 Days)[/]", new Style(Color.Blue, Color.Default, Decoration.Bold));
                    table.Caption = new TableTitle($"[yellow]Total recent matches: {userMatches.Count}[/]");

                    table.AddColumn(new TableColumn("[bold cyan]Match Date[/]").Centered());
                    table.AddColumn(new TableColumn("[bold cyan]Matched With[/]").LeftAligned());
                    table.AddColumn(new TableColumn("[bold cyan]Username[/]").LeftAligned());
                    table.AddColumn(new TableColumn("[bold cyan]Time Ago[/]").LeftAligned());
                    table.AddColumn(new TableColumn("[bold cyan]Status[/]").Centered());

                    foreach (var match in userMatches)
                    {
                        int matchedUserId = match.User1_id == currentUser.Id ? match.User2_id : match.User1_id;
                        var matchedUser = await _userRepository.GetByIdAsync(matchedUserId);
                        if (matchedUser == null) continue;
                        var matchedProfile = await _profileRepository.GetByIdAsync(matchedUser.ProfileId);
                        if (matchedProfile == null) continue;
                        var timeAgo = DateTime.Now - match.MatchDate;

                        string timeAgoStr = timeAgo.TotalDays >= 1 
                            ? $"{(int)timeAgo.TotalDays} days ago"
                            : timeAgo.TotalHours >= 1 
                                ? $"{(int)timeAgo.TotalHours} hours ago"
                                : $"{(int)timeAgo.TotalMinutes} minutes ago";

                        var status = match.MatchDate >= DateTime.Now.AddDays(-1) 
                            ? "[green]New![/]" 
                            : "[yellow]Recent[/]";

                        table.AddRow(
                            $"[white]{match.MatchDate:dd/MM/yyyy HH:mm}[/]",
                            $"[white]{matchedProfile.Name} {matchedProfile.LastName}[/]",
                            $"[white]{matchedUser.Username}[/]",
                            $"[white]{timeAgoStr}[/]",
                            status
                        );
                    }

                    AnsiConsole.Write(table);
                }
            }
            catch (Exception ex)
            {
                var errorPanel = new Panel($"[red]❌ Error viewing recent matches: {ex.Message}[/]")
                {
                    Border = BoxBorder.Rounded,
                    BorderStyle = new Style(Color.Red),
                    Padding = new Padding(1, 1, 1, 1)
                };
                AnsiConsole.Write(errorPanel);
            }

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            Console.ReadKey();
        }
    }
} 