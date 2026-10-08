using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GanmeStore.API.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndpointName = "GetGame";
    private static List<GameSummaryDto> games = [
        new (1,
                "BGMI", "Battle Royale", 10.55M, new DateOnly(2021, 11, 2)),
            new (2,
                "GTA V", "Action-Adventure", 14.88M, new DateOnly(2013, 9, 17)),
            new (3,
                "FIFA 23", "Sports", 9.99M, new DateOnly(2022, 9, 30)),
        ];

    public static void MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/games");
        //GET /games
        group.MapGet("/", async (GameStoreContext dbContext) => await dbContext.Games
        .Include(game => game.Genre)
        .Select(game => new GameSummaryDto(
            game.Id,
            game.Name,
            game.Genre!.Name,
            game.Price,
            DateOnly.FromDateTime(game.ReleaseDate)
        )).AsNoTracking()
        .ToListAsync());

        //GET /games/1
        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            var game = await dbContext.Games.FindAsync(id);

            return game is null ? Results.NotFound() : Results.Ok
            (new GameDetailsDto(
                game.Id,
                game.Name,
                game.GenreId,
                game.Price,
                DateOnly.FromDateTime(game.ReleaseDate)
            ));

        }).WithName(GetGameEndpointName);

        //POST /games
        group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            Game game = new()
            {
                Name = newGame.Name,
                GenreId = newGame.GenreId,
                Price = newGame.Price,
                ReleaseDate = newGame.ReleaseDate.ToDateTime(TimeOnly.MinValue)
            };

            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();

            GameDetailsDto gameDto = new(
                game.Id,
                game.Name,
                game.GenreId,
                game.Price,
                DateOnly.FromDateTime(game.ReleaseDate)
            );

            return Results.CreatedAtRoute(GetGameEndpointName, new { id = gameDto.Id }, gameDto);
        });

        // PUT /games/1
        group.MapPut("/{id}", async (int id, CreateGameDto updatedGame, GameStoreContext dbContext) =>
        {
            var existingGame = await dbContext.Games.FindAsync(id);

            if (existingGame is null)
            {
                return Results.NotFound();
            }

            existingGame.Name = updatedGame.Name;
            existingGame.GenreId = updatedGame.GenreId;
            existingGame.Price = updatedGame.Price;
            existingGame.ReleaseDate = updatedGame.ReleaseDate.ToDateTime(TimeOnly.MinValue);

            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

        //DELETE /games/1
        group.MapDelete("/{id}", (int id) =>
        {
            return Results.NoContent();
        });
    }
}
