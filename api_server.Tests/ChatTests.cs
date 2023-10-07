using ApiServer.Repositories;
using Microsoft.Extensions.Configuration;

namespace api_server.Tests {
    public class ChatTests {
        [Fact]
        public async Task ShouldPass() {
            var dbConfig = new List<KeyValuePair<string, string?>> {
                new("ConnectionStrings:PostgresConnection", "Host=127.0.0.1;Port=5431;User Id=postgres;Password=root;Database=chat_app_db"),
            }; 

            var configuration = new ConfigurationBuilder().AddInMemoryCollection(dbConfig).Build();

            var chatRepository = new ChatRepository(configuration);

            var res = await chatRepository.GetRoomId("17a482f4-3022-417f-80cb-9cb994403866", "6b05a96a-2b2e-435c-9c52-76bb6ed98f53");

            Assert.Equal("d44d1757-11d0-4074-af56-eed13e46bb92", res);
        }
    }
}
