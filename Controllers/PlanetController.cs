using Application;
using Domain;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("planets")]
public class PlanetController : ControllerBase
{
    private readonly PlanetService _service;

    public PlanetController(PlanetService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var planet = await _service.GetAllAsync();
        return Ok(planet);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Planet planet)
    {
        //try catch (same with golang if err != nil)
        try
        {
            var result = await _service.CreateAsync(planet);
            return Created(
                string.Empty,
                new 
                { 
                    message = $"Planet {result.Name} successfully added",
                    id = result.Id
                });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch
        {
            return StatusCode(500);
        }
    }

    [HttpPut("{name}")]
    public async Task<IActionResult> Update(string name, Planet planet)
    {
        try
        {
            var result = await _service.UpdateAsync(name, planet);
            return Ok(
                new
                {
                    message = $"Planet {result.Name} data updated successfully"
                }
            );
        }
        catch
        {
            return StatusCode(500, new { message = "Internal server error"});
        }
    }
}