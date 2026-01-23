using System.Numerics;
using Domain;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application;

public class PlanetService
{
    private readonly AppDbContext _db;
    public PlanetService(AppDbContext db)
    {
        _db = db;
    }

    //get all planet
    public async Task<List<Planet>> GetAllAsync()
    {
        return await _db.Planets.ToListAsync();
    }

    //update
    public async Task<Planet> UpdateAsync(string name, Planet input)
    {
        var planet = await _db.Planets.FirstOrDefaultAsync(p => p.Name == name);
        
        if (planet == null)
        {
            throw new Exception("Planet not found");
        }

        //update fields
        planet.Name = input.Name;
        planet.DiameterKm = input.DiameterKm;
        planet.DiscoveredYear = input.DiscoveredYear;
        planet.HasRing = input.HasRing;
        planet.NumberOfMoons = input.NumberOfMoons;
        planet.DistanceFromSunMillionKm = input.DistanceFromSunMillionKm;
        planet.SurfaceTempC = input.SurfaceTempC;
        planet.GravityMS2 = input.GravityMS2;
        planet.PlanetType = input.PlanetType;

        await _db.SaveChangesAsync();
        return planet;
    }

    //create planet
    public async Task<Planet> CreateAsync(Planet planet)
    {
        _db.Planets.Add(planet);
        
        await _db.SaveChangesAsync();

        return planet;
    }

    //get from planet where surface_temp xx <- get from endpoint parameter
    public async Task<List<Planet>> GetByTemperatureRangeAsync(
        int minTemp,
        int maxTemp
    )
    {
        return await _db.Planets
        .Where(p => p.SurfaceTempC >= minTemp && p.SurfaceTempC <= maxTemp).ToListAsync();
    }
    //(/comparison?) get 2 planet and comparison with diameter, gravity, surface_temp, name planet
    public async Task<(Planet, Planet)> CompareAsync(
    string planetA,
    string planetB
    )
    {
        var a = await _db.Planets
            .FirstOrDefaultAsync(p => p.Name == planetA);

        var b = await _db.Planets
            .FirstOrDefaultAsync(p => p.Name == planetB);

        if (a == null || b == null)
        {
            throw new InvalidOperationException("One or both planets not found");
        }

        return (a, b);
    }

    //(/journey?) get data to journey from planet a to b 
    public async Task<object> CalculateJourneyAsync(
        string from,
        string to,
        double speedKmPerS
    )
    {
        var fromPlanet = await _db.Planets
            .FirstOrDefaultAsync(p => p.Name == from);

        var toPlanet = await _db.Planets
            .FirstOrDefaultAsync(p => p.Name == to);

        if (fromPlanet == null || toPlanet == null)
            throw new InvalidOperationException("one or both planets not found");

        var distanceMillionKm = Math.Abs(
            fromPlanet.DistanceFromSunMillionKm -
            toPlanet.DistanceFromSunMillionKm
        );

        var distanceKm = distanceMillionKm * 1_000_000;
        var timeHours = distanceKm / speedKmPerS;
        var timeDays = timeHours / 24;

        return new
        {
            from = fromPlanet.Name,
            to = toPlanet.Name,
            speed_km = speedKmPerS,
            time_days = Math.Round(timeDays, 2)
        };
    }

    //(/zones) grouping the planet if distance > 300 million km is outer else is inner planet
    public async Task<object> PlanetGrouping()
    {
        var planets = await _db.Planets.ToListAsync();

        var inner = planets
            .Where(p => p.DistanceFromSunMillionKm <= 300)
            .Select(p => p.Name)
            .ToList();
        
        var outer = planets
            .Where(p => p.DistanceFromSunMillionKm >= 300)
            .Select(p => p.Name)
            .ToList();

        return new
        {
            inner_planets = inner,
            outer_planets = outer
        };
    }
}