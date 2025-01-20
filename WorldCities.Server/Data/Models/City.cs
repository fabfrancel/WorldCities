using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace WorldCities.Server.Data.Models;

[Table("Cities")]
[Index(nameof(Name))]
public class City
{
     /// <summary>
    /// The unique id and primary key for this City
    /// </summary>
    [Key]
    [Required]
    public int Id {  get; set; }

    public required string Name { get; set; }

    [Column(TypeName = "decimal(7,4)")]
    public decimal Lat { get; set; }

    [Column(TypeName = "decimal(7,4)")]
    public decimal Lon { get; set; }

    public Point? Location { get; set; }

    [ForeignKey(nameof(Country))]
    public int CountryId {  get; set; }

    public Country? Country { get; set; }
}

public class CityViewModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public double Lat { get; set; }
    public double Lon { get; set; }
    public int CountryId { get; set; }
    public Country? Country { get; set; }
}