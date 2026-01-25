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

    [HttpGet("environment")]
    public async Task<IActionResult> GetByEnvironment(
        // this to get value from query param, but the case is matter/sensitive
        [FromQuery] int min_temp,
        [FromQuery] int max_temp
    )
    {
        //min temp cannot higher than max_temp
        if (min_temp > max_temp)
        {
            return StatusCode(400, new { message = "min temp cannot higher than max temp"});
        }
        if (min_temp == 0 && max_temp == 0)
        {
            return BadRequest(new {message = "cannot 0"});
        }
        try
        {
            var planets = await _service
                .GetByTemperatureRangeAsync(min_temp, max_temp);

            var response = planets.Select(p => new 
            {
                name = p.Name,
                surface_temp_c = p.SurfaceTempC 
            });

            return Ok(response);
        }
        catch
        {
            return StatusCode(500, new { message = "Internal server error"});
        }
    }

    //comparison
    [HttpGet("comparison")]
    public async Task<IActionResult> Compare(
        [FromQuery] string planet_a,
        [FromQuery] string planet_b
    )
    {
        try
        {
            var (a, b) = await _service.CompareAsync(planet_a, planet_b);

            return Ok(new
            {
                planet_a = new
                {
                    name = a.Name,
                    diameter_km = a.DiameterKm,
                    gravity_m_s2 = a.GravityMS2,
                    surface_temp_c = a.SurfaceTempC
                },
                planet_b = new
                {
                    name = b.Name,
                    diameter_km = b.DiameterKm,
                    gravity_m_s2 = b.GravityMS2,
                    surface_temp_c = b.SurfaceTempC
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch
        {
            return StatusCode(500, new { message = "internal server error" });
        }
    }

    //journey from a to b
    [HttpGet("journey")]
    public async Task<IActionResult> Journey(
        [FromQuery] string from,
        [FromQuery] string to,
        [FromQuery] double speed_km_per_s
    )
    {
        if (from == to)
            return BadRequest(new { message = "from and to planet cannot be the same" });

        if (speed_km_per_s <= 0)
            return BadRequest(new { message = "speed must be greater than zero" });

        try
        {
            var result = await _service.CalculateJourneyAsync(from, to, speed_km_per_s);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch
        {
            return StatusCode(500, new { message = "internal server error" });
        }
    }

    //grouping
    [HttpGet("zones")]
    public async Task<IActionResult> Zones()
    {
        try
        {
            var result = await _service.PlanetGrouping();
            return Ok(result);
        }
        catch
        {
            return StatusCode(500, new { message = "Internal server error"});
        }
    }
}