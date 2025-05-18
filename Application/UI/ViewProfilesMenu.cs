using System;
using System.Threading.Tasks;
using System.Linq;
using CampusLove.Domain.Entities;
using CampusLove.Infrastructure.Repositories;
using MySql.Data.MySqlClient;
using Spectre.Console;

namespace CampusLove.Application.UI
{
    public class ViewProfilesMenu
    {
        private readonly ProfileRepository _profileRepository;
        private readonly GenderRepository _genderRepository;
        private readonly UserRepository _userRepository;
        private readonly UserLikesRepository _userLikesRepository;
        private readonly ProfessionRepository _professionRepository;
        private readonly StatusRepository _statusRepository;
        private readonly InterestProfileRepository _interestProfileRepository;
        private readonly InterestRepository _interestRepository;

        public ViewProfilesMenu(MySqlConnection connection)
        {
            _profileRepository = new ProfileRepository(connection);
            _genderRepository = new GenderRepository(connection);
            _userRepository = new UserRepository(connection);
            _userLikesRepository = new UserLikesRepository(connection);
            _professionRepository = new ProfessionRepository(connection);
            _statusRepository = new StatusRepository(connection);
            _interestProfileRepository = new InterestProfileRepository(connection);
            _interestRepository = new InterestRepository(connection);
        }

        public async Task ShowMenu(User currentUser)
        {
            bool returnToMain = false;
            while (!returnToMain)
            {
                Console.Clear();
                var title = new FigletText("🫂 VIEW PROFILES")
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
                        "🔍 Find Profile",
                        "📋 List Profiles",
                        "↩️ Return to Main Menu"
                    });

                var option = AnsiConsole.Prompt(menu);

                try
                {
                    switch (option)
                    {
                        case "🔍 Find Profile":
                            await FindProfile();
                            break;
                        case "📋 List Profiles":
                            await ViewAllProfiles();
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

        private async Task ViewAllProfiles()
        {
            Console.Clear();
            MainMenu.ShowText("🫂 VIEW PROFILES");

            try
            {
                var profiles = (await _profileRepository.GetAllAsync()).ToList();
                var genders = (await _genderRepository.GetAllAsync()).ToList();
                var professions = (await _professionRepository.GetAllAsync()).ToList();
                var statuses = (await _statusRepository.GetAllAsync()).ToList();
                var interests = (await _interestRepository.GetAllAsync()).ToList();

                var genderDict = genders.ToDictionary(g => g.Id, g => g.Description);
                var professionDict = professions.ToDictionary(p => p.Id, p => p.Description);
                var statusDict = statuses.ToDictionary(s => s.Id, s => s.Description);

                // Load interests for each profile
                foreach (var profile in profiles)
                {
                    var interestProfiles = await _interestProfileRepository.GetAllAsync();
                    profile.Details = interestProfiles.Where(ip => ip.ProfileId == profile.Id).ToList();
                    foreach (var detail in profile.Details)
                    {
                        detail.Interest = interests.FirstOrDefault(i => i.Id == detail.InterestId);
                    }
                }

                if (!profiles.Any())
                {
                    MainMenu.ShowMessage("\nNo profiles found in the system.", ConsoleColor.Yellow);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("\nPress any key to return to menu...");
                    Console.ResetColor();
                    Console.ReadKey();
                    return;
                }

                // Create a table for the summary view
                var summaryTable = new Table();
                summaryTable.Border(TableBorder.Rounded);
                summaryTable.BorderColor(Color.Purple);
                summaryTable.Title = new TableTitle("Profiles Summary", new Style(Color.Purple, Color.Default, Decoration.Bold));
                summaryTable.Caption = new TableTitle($"Total profiles found: {profiles.Count}", new Style(Color.Yellow, Color.Default));

                // Add columns with styling
                summaryTable.AddColumn(new TableColumn("[bold cyan]ID[/]").Centered());
                summaryTable.AddColumn(new TableColumn("[bold cyan]Name[/]").LeftAligned());
                summaryTable.AddColumn(new TableColumn("[bold cyan]Gender[/]").LeftAligned());
                summaryTable.AddColumn(new TableColumn("[bold cyan]Profession[/]").LeftAligned());
                summaryTable.AddColumn(new TableColumn("[bold cyan]Status[/]").LeftAligned());
                summaryTable.AddColumn(new TableColumn("[bold cyan]Likes[/]").Centered());

                // Add rows to summary table
                foreach (var profile in profiles)
                {
                    summaryTable.AddRow(
                        $"[white]{profile.Id}[/]",
                        $"[white]{profile.Name} {profile.LastName}[/]",
                        $"[white]{genderDict.GetValueOrDefault(profile.GenderId, "N/A")}[/]",
                        $"[white]{professionDict.GetValueOrDefault(profile.ProfessionId, "N/A")}[/]",
                        $"[white]{statusDict.GetValueOrDefault(profile.StatusId, "N/A")}[/]",
                        $"[green]{profile.TotalLikes}[/]"
                    );
                }

                // Display the summary table
                AnsiConsole.Write(summaryTable);

                // Ask if user wants to see detailed information
                Console.ForegroundColor = ConsoleColor.Yellow;
                string viewDetails = MainMenu.ReadText("\nDo you want to view detailed information for a profile? (Y/N): ");
                Console.ResetColor();

                if (viewDetails.ToUpper() == "Y")
                {
                    while (true)
                    {
                        int profileId = MainMenu.ReadInteger("\nEnter profile ID to view details (0 to exit): ");
                        if (profileId == 0) break;

                        var selectedProfile = profiles.FirstOrDefault(p => p.Id == profileId);
                        if (selectedProfile == null)
                        {
                            MainMenu.ShowMessage("❌ Profile not found.", ConsoleColor.Red);
                            continue;
                        }

                        // Create a detailed view table
                        var detailTable = new Table();
                        detailTable.Border(TableBorder.Rounded);
                        detailTable.BorderColor(Color.Blue);
                        detailTable.Title = new TableTitle($"Profile Details - {selectedProfile.Name} {selectedProfile.LastName}", new Style(Color.Blue, Color.Default, Decoration.Bold));

                        // Add sections for different types of information
                        detailTable.AddColumn(new TableColumn("[bold magenta]Field[/]").LeftAligned());
                        detailTable.AddColumn(new TableColumn("[bold magenta]Value[/]").LeftAligned());

                        // Add profile information
                        detailTable.AddRow("[bold yellow]Basic Information[/]", "");
                        detailTable.AddRow("ID", $"[white]{selectedProfile.Id}[/]");
                        detailTable.AddRow("Name", $"[white]{selectedProfile.Name} {selectedProfile.LastName}[/]");
                        detailTable.AddRow("Identification", $"[white]{selectedProfile.Identification}[/]");
                        detailTable.AddRow("Slogan", $"[white]{selectedProfile.Slogan}[/]");
                        detailTable.AddRow("Created Date", $"[white]{selectedProfile.createDate.ToShortDateString()}[/]");

                        detailTable.AddRow("[bold yellow]Profile Details[/]", "");
                        detailTable.AddRow("Gender", $"[white]{genderDict.GetValueOrDefault(selectedProfile.GenderId, "N/A")}[/]");
                        detailTable.AddRow("Profession", $"[white]{professionDict.GetValueOrDefault(selectedProfile.ProfessionId, "N/A")}[/]");
                        detailTable.AddRow("Status", $"[white]{statusDict.GetValueOrDefault(selectedProfile.StatusId, "N/A")}[/]");
                        detailTable.AddRow("Total Likes", $"[green]{selectedProfile.TotalLikes}[/]");

                        detailTable.AddRow("[bold yellow]Interests[/]", "");
                        if (selectedProfile.Details.Any())
                        {
                            string interestList = string.Join("\n", selectedProfile.Details.Select(d => $"• {d.Interest?.Description ?? "N/A"}"));
                            detailTable.AddRow("", $"[white]{interestList}[/]");
                        }
                        else
                        {
                            detailTable.AddRow("", "[white]No interests listed[/]");
                        }

                        // Display the detailed table
                        AnsiConsole.Write(detailTable);

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("\nPress any key to continue...");
                        Console.ResetColor();
                        Console.ReadKey();
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError accessing database: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to return to menu...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task FindProfile()
        {
            Console.Clear();
            MainMenu.ShowText(" SEARCH PROFILE ");

            string name = MainMenu.ReadText("\nEnter name: ");

            try
            {
                var profiles = await _profileRepository.GetByNameAsync(name);
                var genders = (await _genderRepository.GetAllAsync()).ToList();
                var professions = (await _professionRepository.GetAllAsync()).ToList();
                var statuses = (await _statusRepository.GetAllAsync()).ToList();
                var interests = (await _interestRepository.GetAllAsync()).ToList();

                var genderDict = genders.ToDictionary(g => g.Id, g => g.Description);
                var professionDict = professions.ToDictionary(p => p.Id, p => p.Description);
                var statusDict = statuses.ToDictionary(s => s.Id, s => s.Description);

                if (!profiles.Any())
                {
                    MainMenu.ShowMessage("\nNo profiles found matching the search criteria.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.WriteLine($"\nFound {profiles.Count()} matching profiles:");
                    Console.WriteLine("------------------");

                    foreach (var profile in profiles)
                    {
                        Console.WriteLine("\n-----------------------------");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Name: {profile.Name} {profile.LastName}");
                        Console.ResetColor();

                        Console.WriteLine($"Identification: {profile.Identification}");
                        Console.WriteLine($"Slogan: {profile.Slogan}");

                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine($"Gender: {genderDict.GetValueOrDefault(profile.GenderId, "N/A")}");
                        Console.WriteLine($"Profession: {professionDict.GetValueOrDefault(profile.ProfessionId, "N/A")}");
                        Console.WriteLine($"Status: {statusDict.GetValueOrDefault(profile.StatusId, "N/A")}");
                        Console.ResetColor();

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Total Likes: {profile.TotalLikes}");
                        Console.WriteLine($"Created: {profile.createDate.ToShortDateString()}");
                        Console.ResetColor();

                        // Load and display interests for this profile
                        var interestProfiles = await _interestProfileRepository.GetAllAsync();
                        var profileInterests = interestProfiles.Where(ip => ip.ProfileId == profile.Id).ToList();
                        foreach (var detail in profileInterests)
                        {
                            detail.Interest = interests.FirstOrDefault(i => i.Id == detail.InterestId);
                        }

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nInterests:");
                        if (profileInterests.Any())
                        {
                            foreach (var detail in profileInterests)
                            {
                                Console.WriteLine($"- {detail.Interest?.Description ?? "N/A"}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("- No interests listed");
                        }
                        Console.ResetColor();

                        Console.WriteLine("-----------------------------");
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError searching profiles: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }

    }
}