using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ApiServer.Data.Entities;
using ApiServer.Repositories;

namespace ApiServer.Controllers {
    [Authorize]
    [ApiController]
    [Route("api/messages")]
    public class ChatController : ControllerBase {
        private readonly ChatRepository _chatRepository;
    
        public ChatController(ChatRepository chatRepository) {
            _chatRepository = chatRepository;
        }

        [HttpGet("/api/rooms")]
        public async Task<IActionResult> GetRoomId(string userId0, string userId1) {
            string result;
            var id = await _chatRepository.GetRoomId(userId0, userId1);

            if (id == null) {
                var roomId = await _chatRepository.CreateRoom();

                await _chatRepository.AddChat(roomId, userId0);
                await _chatRepository.AddChat(roomId, userId1);

                result = roomId;
            } else {
                result = id;
            }

            return Ok(new {
                Result = result
            });
        }
        
        [HttpGet("{roomId}")]
        public async Task<IActionResult> GetMessages(string roomId) {
            return Ok(await _chatRepository.GetMessagesByRoomId(roomId));
        }

        [HttpPost("{roomId}")]
        public async Task<IActionResult> PostMessage(string roomId, Message message) {
            await _chatRepository.PostMessage(roomId, message);

            return Ok();
        }
    }
}

