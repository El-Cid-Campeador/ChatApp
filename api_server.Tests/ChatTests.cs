using ApiServer.Repositories;
using ApiServer.Services;

namespace api_server.Tests {
    public class ChatTests {
        [Fact]
        public async Task GetRoomIdByUsersId() {
            var chatRepository = new ChatRepository();
            var chatService = new ChatService(chatRepository);

            var res = await chatService.GetRoomId("6632a841-a972-4019-8811-a95216b91765", "f42a78b1-2a8d-44ac-b0d4-4258b5a1207e");

            Assert.Equal("6e48af4e-0aa5-47bd-8f41-353aedaca96b", res);
        }
    }
}
