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
    //(/comparison?) get 2 planet and comparison with diameter, gravity, surface_temp, name planet
    //(/journey?) get data to journey from planet a to b 
    //(/zones) grouping the planet if distance > 300 million km is outer else is inner planet
}