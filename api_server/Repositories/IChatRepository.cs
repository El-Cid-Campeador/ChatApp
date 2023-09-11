using ApiServer.Data.Entities;

namespace ApiServer.Repositories {
    public interface IChatRepository {
        Task<string?> GetRoomId(string userId0, string userId1);
        Task<string> CreateRoom();
        Task AddChat(string roomId, string userId);
        Task<List<Message>> GetMessagesByRoomId(string roomId);
        Task AddMessage(string roomId, Message message);
    }
}
