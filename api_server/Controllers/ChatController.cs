using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ApiServer.Services;
using ApiServer.Data.Entities;

namespace ApiServer.Controllers {
    [Authorize]
    [ApiController]
    public class ChatController : ControllerBase {
        private readonly ChatService _chatService;
    
        public ChatController(ChatService chatService) {
            _chatService = chatService;
        }

        [HttpGet("api/rooms")]
        public async Task<IActionResult> GetRoomId(string userId0, string userId1) {
            try {
                return Ok(new {
                    Result = await _chatService.GetRoomId(userId0, userId1)
                });
            } catch (Exception ex) {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        [HttpGet("api/messages/{roomId}")]
        public async Task<IActionResult> GetMessages(string roomId) {
            try {
                return Ok(await _chatService.GetMessages(roomId));
            } catch (Exception ex) {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("api/messages/{roomId}")]
        public async Task<IActionResult> PostMessage(string roomId, Message message) {
            try {
               await _chatService.PostMessage(roomId, message);

                return Ok();
            } catch (Exception ex) {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

