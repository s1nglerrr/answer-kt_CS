using Microsoft.AspNetCore.Mvc;
using WebApplication3.Services;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoggerController(ILoggerService logger) : ControllerBase
    {
        private ILoggerService _logger = logger;

        [HttpPost]
        public IActionResult WriteLog([FromBody] string message)
        {
            _logger.Write(message);
            return Ok("Сообщение успешно записано");
        }
    }
}