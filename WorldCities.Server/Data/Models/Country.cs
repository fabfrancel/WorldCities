﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WorldCities.Server.Data.Models;

[Table("Countries")]
[Index(nameof(Name))]
[Index(nameof(ISO2))]
[Index(nameof(ISO3))]
public class Country
{
    /// <summary>
    /// The unique Id and primary key for this country
    /// </summary>
    [Key]
    [Required]
    public int Id {  get; set; }

    /// <summary>
    /// Country name (in UFT8 format)
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Country code (in ISO 3166-1 ALPHA-2 format)
    /// </summary>
    [JsonPropertyName("iso2")]
    public required string ISO2 { get; set; }

    /// <summary>
    /// Country code (in ISO 3166-1 ALPHA-3 format)
    /// </summary>
    [JsonPropertyName("iso3")]
    public required string ISO3 {  get; set; }

    /// <summary>
    /// A collection of all the cities related to this country
    /// </summary>
    public ICollection<City>? Cities { get; set; }
}
