using System;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using CampusLove.Infrastructure.Repositories;
using MySql.Data.MySqlClient;
using Spectre.Console;

namespace CampusLove.Application.UI
{
    public class ChatMenu
    {
        private readonly MessageRepository _messageRepo;
        private readonly UserRepository _userRepo;

        public ChatMenu(MySqlConnection connection)
        {
            _messageRepo = new MessageRepository(connection);
            _userRepo = new UserRepository(connection);
        }

        public async Task ShowMenu(User currentUser)
        {
            // Prepara pantalla de chat
            Console.Clear();
            bool exitChat = false;
            Console.CursorVisible = false;
            
            var users = (await _userRepo.GetAllAsync()).Where(u => u.Id != currentUser.Id).ToList();
            if (!users.Any())
            {
                MainMenu.ShowMessage("No hay otros usuarios disponibles.", ConsoleColor.Yellow);
                Console.ReadKey();
                return;
            }

            var otherUsername = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold blue]Seleccione usuario para chatear:[/]")
                    .AddChoices(users.Select(u => u.Username!)));

            var otherUser = users.First(u => u.Username == otherUsername);

            // Panel de cabecera dentro de la región Live con usuario seleccionado
            var headerPanel = new Panel($"💬 Chat con {otherUser.Username}")
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 1),
                BorderStyle = new Style(Color.Blue)
            };

            while (!exitChat)
            {
                Console.Clear();
                MainMenu.ShowHeader($" 💬 Chat con {otherUser.Username} ");

                var history = (await _messageRepo.GetAllAsync())
                    .Where(m => (m.SenderId == currentUser.Id && m.ReceiverId == otherUser.Id) ||
                                (m.SenderId == otherUser.Id && m.ReceiverId == currentUser.Id))
                    .OrderBy(m => m.SentAt)
                    .ToList();

                // Mostrar chat estilo WhatsApp con burbujas
                var chatGrid = new Grid().Expand();
                chatGrid.AddColumn(); chatGrid.AddColumn();

                foreach (var msg in history)
                {
                    var bubble = new Panel(msg.Text)
                    {
                        Border = BoxBorder.Rounded,
                        Padding = new Padding(1, 0, 1, 0),
                        BorderStyle = new Style(msg.SenderId == currentUser.Id ? Color.Green : Color.Blue),
                        Header = new PanelHeader(msg.SentAt.ToString("HH:mm"), msg.SenderId == currentUser.Id ? Justify.Right : Justify.Left)
                    };

                    if (msg.SenderId == currentUser.Id)
                        chatGrid.AddRow(new Markup(""), bubble);
                    else
                        chatGrid.AddRow(bubble, new Markup(""));
                }

                AnsiConsole.Write(chatGrid);

                var text = MainMenu.ReadText("\nEscribe mensaje (o escribir '/exit' para salir): ");
                if (text.Trim().ToLower() == "/exit")
                    exitChat = true;
                else if (!string.IsNullOrWhiteSpace(text))
                {
                    var message = new Message
                    {
                        SenderId = currentUser.Id,
                        ReceiverId = otherUser.Id,
                        Text = text,
                        SentAt = DateTime.Now
                    };
                    await _messageRepo.InsertAsync(message);
                }
            }
            Console.CursorVisible = true;
        }
    }
} 