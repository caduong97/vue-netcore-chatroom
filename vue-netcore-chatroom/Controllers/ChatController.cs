using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using vue_netcore_chatroom.Models;
using vue_netcore_chatroom.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace vue_netcore_chatroom.Controllers
{
    [Authorize]
    public class ChatController : BaseController
    {
        private readonly IChatService _chatService;

        public ChatController(ILogger<BaseController> logger, IConfiguration configuration, IChatService chatService) : base(logger, configuration)
        {
            _chatService = chatService;

        }

        [HttpGet("")]
        public async Task<IActionResult> GetChats()
        {
            List<Chat> chats = await _chatService.GetChats(HttpContext.User);

            List<ChatDto> chatDtos = chats.Select(c => ChatDto.FromDbModel(c)).ToList();

            return Ok(chatDtos);
        }

        [HttpPost("CreateOrUpdate")]
        public async Task<IActionResult> CreateOrUpdateChat([FromBody] ChatDto dto)
        {
            if (!ModelState.IsValid)
                throw new Exception("CreateOrUpdateChat error: ModelState is invalid.");

            Chat chat = await _chatService.CreateOrUpdateChat(dto);

            ChatDto chatDto = ChatDto.FromDbModel(chat);

            return Ok(chatDto);
        }
    }
}

