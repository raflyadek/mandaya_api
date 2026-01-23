using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;

namespace Domain;

[Table("planet")]
public class Planet
{
    public int Id { get; set;}
    public string Name { get; set;}
    public int DiameterKm { get; set;}
    public int DiscoveredYear { get; set;}
    public bool HasRing { get; set;}
    public int NumberOfMoons { get; set;}
    public float DistanceFromSunMillionKm { get; set;}
    public int SurfaceTempC { get; set;}
    public float GravityMS2 { get; set;}
    public string PlanetType { get; set;}
}