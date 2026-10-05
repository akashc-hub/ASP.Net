using GameStore.Api.Dtos;
using GanmeStore.API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidation();

var app = builder.Build();

app.MapGamesEndpoints();
    
app.Run();
