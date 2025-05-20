using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using MySql.Data.MySqlClient;

namespace CampusLove.Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly MySqlConnection _connection;

        public MessageRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Message>> GetAllAsync()
        {
            var messages = new List<Message>();
            const string query = "SELECT id, sender_id, receiver_id, text, sent_at FROM message";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                messages.Add(new Message
                {
                    Id = Convert.ToInt32(reader["id"]),
                    SenderId = Convert.ToInt32(reader["sender_id"]),
                    ReceiverId = Convert.ToInt32(reader["receiver_id"]),
                    Text = reader["text"]?.ToString() ?? string.Empty,
                    SentAt = Convert.ToDateTime(reader["sent_at"])
                });
            }

            return messages;
        }

        public async Task<Message?> GetByIdAsync(object id)
        {
            const string query = "SELECT id, sender_id, receiver_id, text, sent_at FROM message WHERE id = @Id";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Message
                {
                    Id = Convert.ToInt32(reader["id"]),
                    SenderId = Convert.ToInt32(reader["sender_id"]),
                    ReceiverId = Convert.ToInt32(reader["receiver_id"]),
                    Text = reader["text"]?.ToString() ?? string.Empty,
                    SentAt = Convert.ToDateTime(reader["sent_at"])
                };
            }

            return null;
        }

        public async Task<bool> InsertAsync(Message message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            const string query = "INSERT INTO message (sender_id, receiver_id, text, sent_at) VALUES (@SenderId, @ReceiverId, @Text, @SentAt)";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@SenderId", message.SenderId);
                command.Parameters.AddWithValue("@ReceiverId", message.ReceiverId);
                command.Parameters.AddWithValue("@Text", message.Text);
                command.Parameters.AddWithValue("@SentAt", message.SentAt);

                var result = await command.ExecuteNonQueryAsync() > 0;
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Message message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            const string query = "UPDATE message SET sender_id = @SenderId, receiver_id = @ReceiverId, text = @Text, sent_at = @SentAt WHERE id = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@SenderId", message.SenderId);
                command.Parameters.AddWithValue("@ReceiverId", message.ReceiverId);
                command.Parameters.AddWithValue("@Text", message.Text);
                command.Parameters.AddWithValue("@SentAt", message.SentAt);
                command.Parameters.AddWithValue("@Id", message.Id);

                var result = await command.ExecuteNonQueryAsync() > 0;
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(object id)
        {
            const string query = "DELETE FROM message WHERE id = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Id", id);

                var result = await command.ExecuteNonQueryAsync() > 0;
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
} 