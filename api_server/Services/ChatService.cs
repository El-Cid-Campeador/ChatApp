using ApiServer.Data.Entities;
using ApiServer.Repositories;

namespace ApiServer.Services {
    public class ChatService {
        private readonly IChatRepository _chatRepository;
        public ChatService(IChatRepository chatRepository){
            _chatRepository = chatRepository;
        }

        public async Task<string> GetRoomId(string userId0, string userId1) {
            var id = await _chatRepository.GetRoomId(userId0, userId1);

            if (id == null) {
                var roomId = await _chatRepository.CreateRoom();

                await _chatRepository.AddChat(roomId, userId0);
                await _chatRepository.AddChat(roomId, userId1);

                return roomId;
            }

            return id;
        }

        public async Task<List<Message>> GetMessages(string roomId) {
            return await _chatRepository.GetMessagesByRoomId(roomId);
        }

        public async Task PostMessage(string roomId, Message message) {
            await _chatRepository.AddMessage(roomId, message);
        }
    }
}
