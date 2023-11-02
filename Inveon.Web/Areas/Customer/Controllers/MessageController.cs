using Inveon.Web.Hubs;
using Inveon.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Inveon.Web.Areas.Customer.Controllers
{
	[Area("Customer")]
	public class MessageController : Controller
	{
		private readonly IHubContext<MessageHub> _messageHub;

		public MessageController(IHubContext<MessageHub> messageHub)
		{
			_messageHub = messageHub;
		}

		[Authorize]
		public async Task<IActionResult> Index()
		{
			return View();
		}

		[Authorize]
        [Route("[Controller]")]
        [HttpPost]
		public async Task<IActionResult> SendMessage([FromBody] Message message)
		{
            var sender = User.Claims.Where(u => u.Type == "given_name")?.FirstOrDefault()?.Value;
			var send_time = DateTime.Now.ToString("HH:mm:ss");
			var obj = new { sender=sender, message=message.Text, time=send_time };
			await _messageHub.Clients.All.SendAsync("messages", obj);
            return Accepted();
        }
	}	
}
