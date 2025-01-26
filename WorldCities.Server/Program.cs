using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WorldCities.Server.Data;


var builder = WebApplication.CreateBuilder(args);


//https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions?view=net-9.0
builder.Services.AddControllers()
    .AddJsonOptions(options =>{
        options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection"),
    x => x.UseNetTopologySuite()
    ));

var app = builder.Build();


app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

// Seed db
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    var context = services.GetRequiredService<ApplicationDbContext>();
//    context.Database.Migrate();
//    await ContextSeed.SeedAsync(context);
//}

app.Run();
