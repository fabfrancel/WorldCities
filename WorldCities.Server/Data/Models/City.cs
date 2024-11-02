using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace WorldCities.Server.Data.Models;

[Table("Cities")]
[Index(nameof(Name))]
[Index(nameof(Lat))]
[Index(nameof(Lon))]
public class City
{
     /// <summary>
    /// The unique id and primary key for this City
    /// </summary>
    [Key]
    [Required]
    public int Id {  get; set; }

    /// <summary>
    /// Citi name (in UTF8 format)
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// City latitude
    /// </summary>
    [Column(TypeName = "decimal(7,4)")]
    public decimal Lat { get; set; }

    /// <summary>
    /// City longitude
    /// </summary>
    [Column(TypeName = "decimal(7,4)")]
    public decimal Lon { get; set; }

    /// <summary>
    /// Country Id (Foreign Key)
    /// </summary>
    [ForeignKey(nameof(Country))]
    public int CountryId {  get; set; }

    /// <summary>
    /// The country related to this city
    /// </summary>
    public Country? Country { get; set; }
}