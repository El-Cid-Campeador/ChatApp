using ApiServer.Repositories;
using ApiServer.Services;
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
            var chatService = new ChatService(chatRepository);

            var res = await chatService.GetRoomId("dbf09d26-6251-445f-b349-8d9560e0ab10", "6d7b6703-0e2d-474b-811f-602572dee1dc");

            Assert.Equal("17dfc230-ef6b-4ed5-a478-42a238b8349c", res);
        }
    }
}
