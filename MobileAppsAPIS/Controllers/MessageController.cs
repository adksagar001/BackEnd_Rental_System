
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MobileAppsAPIS.Classes;
using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace MobileAppsAPIS.Controllers
{
    [Route("[controller]/[action]")]
    public class MessageController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        DataHandeler dh = new DataHandeler();

        public MessageController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }
        // Add a test method in MessageController to send message to receiver group
        [HttpGet("TestSend")]
        public async Task<IActionResult> TestSend(string toUserId)
        {
            await _hubContext.Clients.Group(toUserId).SendAsync("ReceiveMessage", "Server", "Test message from server", DateTime.Now.ToString());
            return Ok("Test message sent");
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] MessageModel model)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                new SqlParameter("@SentFrom", model.SentFrom),
                new SqlParameter("@SentTo", model.SentTo),
                new SqlParameter("@Message", model.Message),
                new SqlParameter("@MessageSentAt", DateTime.Now)
                };

                int result = dh.Insert("sp_InsertUserMessage", parameters, CommandType.StoredProcedure);

                // Send message via SignalR
                await _hubContext.Clients.Group(model.SentTo.ToString())
                    .SendAsync("ReceiveMessage", model.SentFrom.ToString(), model.Message);

                await _hubContext.Clients.Group(model.SentFrom.ToString())
                    .SendAsync("ReceiveMessage", model.SentFrom.ToString(), model.Message);

                return Ok(new { success = true, message = "Sent" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred: " + ex.Message);
                return BadRequest(new { success = false, error = ex.Message });
            }
        }
        [HttpPost]
        public IActionResult GetMessages([FromBody] MessageModel mm)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                     new SqlParameter("@SentFrom",mm.SentFrom ),
                     new SqlParameter("@SentTo", mm.SentTo)
                };

                DataTable dt = dh.ReadData("sp_GetChatMessages", parameters, CommandType.StoredProcedure);

                return Ok(dt);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }


        public class MessageModel
        {
            public int SentFrom { get; set; }
            public int SentTo { get; set; }
            public string Message { get; set; }
        }
    }
}
