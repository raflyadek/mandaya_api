using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("ping")]
public class PingController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("pong");
    }
}