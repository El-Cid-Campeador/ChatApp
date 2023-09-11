using ApiServer.Data;
using Npgsql;

namespace ApiServer.Repositories {
    public class UserRepository : IUserRepository {
        private readonly string _connectionString;

        public UserRepository(){
            _connectionString = "Host=127.0.0.1;Port=5432;User Id=postgres;Password=root;Database=chat_app_db";
        }

        public async Task PostUser(UserSignUpCred user) {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password, 12);

            using var command = new NpgsqlCommand(
                "INSERT INTO users (firstName, lastName, username, email, passwordHash, createdOn) VALUES (@firstName, @lastName, @username, @email, @passwordHash, NOW())"
                , connection
            );
            command.Parameters.AddWithValue("@firstName", user.FirstName);
            command.Parameters.AddWithValue("@lastName", user.LastName);
            command.Parameters.AddWithValue("@username", user.Username);
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@passwordHash", passwordHash);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<LoggedInUserInfo?> FindUser(UserSignInCred user) {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(
                "SELECT id, username, passwordHash, profilePicPath FROM users WHERE username = @usernameOrEmail OR email = @usernameOrEmail"
                , connection
            );
            command.Parameters.AddWithValue("@usernameOrEmail", user.UsernameOrEmail);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync()) {
                if (BCrypt.Net.BCrypt.EnhancedVerify(user.Password, reader["passwordHash"].ToString(), BCrypt.Net.HashType.SHA384)) {
                    return new LoggedInUserInfo {
                        Id = reader["id"].ToString()!,
                        Username = reader["username"].ToString()!,
                        ProfilePicPath = reader["profilePicPath"].ToString()
                    };
                };
            }

            return null; 
        }
    }
}
