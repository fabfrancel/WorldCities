using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using WorldCities.Server.Data.Models;

namespace WorldCities.Server.Data;

public static class ContextSeed
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {

        if (!context.Cities.Any())
        {
            var cities = await context.Cities.ToListAsync();

            foreach (var city in cities)
            {
                if (city.Lat >= -90 && city.Lat <= 90 && city.Lon >= -180 && city.Lon <= 180)
                {
                    //SRID represents the spatial reference ID of the geography instance that you wish to return.
                    var point = new Point((double)city.Lon, (double)city.Lat) { SRID = 4326 };
                    city.Location = point;
                }
                else
                {
                    Console.WriteLine($"Invalid coordinates for city: {city.Name}");
                }
            }

            Console.WriteLine($"TOTAL OF CITIES: {cities.Count}");

            await context.SaveChangesAsync();
        }
    }
}
