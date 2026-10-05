using GameStore.Api.Dtos;

namespace GanmeStore.API.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndpointName = "GetGame";
    private static List<GameDto> games = [
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
        app.MapGet("/", () => games);

        //GET /games/1
        group.MapGet("/{id}", (int id) =>
        {
            var game = games.Find(game => game.Id == id);

            return game is not null ? Results.Ok(game) : Results.NotFound();

        }).WithName(GetGameEndpointName);

        //POST /games
        group.MapPost("/", (CreateGameDto newGame) =>
        {
            GameDto game = new(
                games.Count + 1,
                newGame.Name,
                newGame.Genre,
                newGame.Price,
                newGame.ReleaseDate
            );

            games.Add(game);

            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game);
        });

        // PUT /games/1
        group.MapPut("/{id}", (int id, CreateGameDto updatedGame) =>
        {
            var index = games.FindIndex(game => game.Id == id);

            if (index == -1)
            {
                return Results.NotFound();
            }

            games[index] = new GameDto(
                id,
                updatedGame.Name,
                updatedGame.Genre,
                updatedGame.Price,
                updatedGame.ReleaseDate
            );

            return Results.NoContent();
        });

        //DELETE /games/1
        group.MapDelete("/{id}", (int id) =>
        {
            return Results.NoContent();
        });
    }
}
