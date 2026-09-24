using GameStore.Api.Dtos;
const string GetGameEndpointName = "GetGame";

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

List<GameDto> games = [
    new (1, "BGMI", "Battle Royale", 10.55M, new DateOnly(2021, 11, 2)),
    new (2, "GTA V", "Action-Adventure", 14.88M, new DateOnly(2013, 9, 17)),
    new (3, "FIFA 23", "Sports", 9.99M, new DateOnly(2022, 9, 30)),
];
//GET /games
app.MapGet("/games", () => games);

//GET /games/1
app.MapGet("/games/{id}", (int id) => games.Find(game => game.Id == id)).WithName(GetGameEndpointName);

//POST /games
app.MapPost("/games", (CreateGameDto newGame) =>
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
app.MapPut("/games/{id}", (int id, CreateGameDto updatedGame) =>
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

    
app.Run();
