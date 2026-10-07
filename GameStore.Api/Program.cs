using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Models;
using GanmeStore.API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidation();

var connString = "Data Source=GameStore.db";
builder.Services.AddSqlite<GameStoreContext>(connString,
optionsAction: options => options.UseSeeding((context, _) =>
{
    if(!context.Set<Genre>().Any())
    {
        context.Set<Genre>().AddRange(
            new Genre { Name = "Action" },
            new Genre { Name = "Adventure" },
            new Genre { Name = "RPG" },
            new Genre { Name = "Strategy" },
            new Genre { Name = "Simulation" }
        );
        context.SaveChanges();
    }
}));

var app = builder.Build();

app.MapGamesEndpoints();

app.MigrateDb();
    
app.Run();
