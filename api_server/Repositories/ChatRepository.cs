using ApiServer.Data.Entities;
using Npgsql;

namespace ApiServer.Repositories {
    public class ChatRepository : IChatRepository {
        private readonly string _connectionString;

        public ChatRepository(){
            _connectionString = "Host=127.0.0.1;Port=5432;User Id=postgres;Password=root;Database=chat_app_db";
        }

        public async Task<string?> GetRoomId(string userId0, string userId1) {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(
                "SELECT roomId FROM chats WHERE userId = @id0::uuid AND roomId IN (SELECT roomId FROM chats WHERE userId = @id1::uuid);"
                , connection
            );
            command.Parameters.AddWithValue("@id0", userId0);
            command.Parameters.AddWithValue("@id1", userId1);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync()) {
                return reader["roomId"].ToString();
            }

            return null;
        }

        public async Task<string> CreateRoom() {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var id = Guid.NewGuid().ToString();

            using var command = new NpgsqlCommand("INSERT INTO rooms (id) VALUES (@id::uuid)", connection);

            command.Parameters.AddWithValue("@id", id);

            await command.ExecuteNonQueryAsync();

            return id;
        }

        public async Task AddChat(string roomId, string userId) {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("INSERT INTO chats (roomId, userId) VALUES (@roomId::uuid, @userId::uuid)", connection);
            command.Parameters.AddWithValue("@roomId", roomId);
            command.Parameters.AddWithValue("@userId", userId);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<Message>> GetMessagesByRoomId(string roomId) {
            List<Message> messages = new();
            
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(
                "SELECT * FROM messages WHERE roomId = @roomId::uuid"
                , connection
            );
            command.Parameters.AddWithValue("@roomId", roomId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) {
                var message = new Message() {
                    Id=reader["Id"].ToString()!,
                    SenderId=reader["SenderId"].ToString()!,
                    Content=reader["Content"].ToString()!,
                    Date=reader["date"].ToString()!
                };

                messages.Add(message);
            }

            return messages;
        }

        public async Task AddMessage(string roomId, Message message) {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(
                "INSERT INTO messages (id, senderId, roomId, content, date) VALUES (@id::uuid, @senderId::uuid, @roomId::uuid, @content, to_timestamp(@date, 'YYYY-MM-DDTHH24:MI:SS.MS'))"
                , connection
            );

            command.Parameters.AddWithValue("@id", message.Id);
            command.Parameters.AddWithValue("@senderId", message.SenderId);
            command.Parameters.AddWithValue("@roomId", roomId);
            command.Parameters.AddWithValue("@content", message.Content);
            command.Parameters.AddWithValue("@date", message.Date);

            await command.ExecuteNonQueryAsync();
        }
    }
}
