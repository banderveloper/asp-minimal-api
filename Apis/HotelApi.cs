public class HotelApi : IApi
{
    public void Register(WebApplication app)
    {
        app.MapGet("/hotels", Get)
            .Produces<List<Hotel>>(StatusCodes.Status200OK)
            .WithName("GetAllHotels")
            .WithTags("Getters");

        app.MapGet("/hotels/{id}", GetById)
            .Produces<Hotel>(StatusCodes.Status200OK)
            .WithName("GetHotel")
            .WithTags("Getters");

        app.MapGet("/hotels/search/name/{query}", SearchByName)
            .Produces<List<Hotel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("SearchHotels")
            .WithTags("Getters")
            .ExcludeFromDescription();

        app.MapGet("hotels/search/location/{coordinate}", SearchByCoordinate)
            .ExcludeFromDescription();


        app.MapPost("/hotels", Post)
            .Accepts<Hotel>("application/json")
            .Produces<Hotel>(StatusCodes.Status201Created)
            .WithName("CreateHotel")
            .WithTags("Creators");

        app.MapPut("/hotels", Put)
            .Accepts<Hotel>("application/json")
            .WithName("UpdateHotel")
            .WithTags("Updaters"); ;

        app.MapDelete("/hotels/{id}", Delete)
            .WithName("DeleteHotel")
            .WithTags("Deleters");
    }

    [Authorize]
    private async Task<ActionResult<List<Hotel>>> Get(IHotelRepository repos) =>
        await repos.GetHotelsAsync();

    [Authorize]
    private async Task<IResult> GetById(int id, IHotelRepository repos) =>
        await repos.GetHotelAsync(id) is Hotel hotel ?
            Results.Ok(hotel) :
            Results.BadRequest();

    [Authorize]
    private async Task<IResult> Post([FromBody] Hotel hotel, IHotelRepository repos)
    {
        await repos.InsertHotelAsync(hotel);
        await repos.SaveAsync();
        return Results.Created($"hotels/{hotel.Id}", hotel);
    }

    [Authorize]
    private async Task<IResult> Put([FromBody] Hotel hotel, IHotelRepository repos)
    {
        await repos.UpdateHotelAsync(hotel);
        await repos.SaveAsync();
        return Results.NoContent();
    }

    [Authorize]
    private async Task<IResult> Delete(int id, IHotelRepository repos)
    {
        await repos.DeleteHotelAsync(id);
        await repos.SaveAsync();
        return Results.NoContent();
    }

    [Authorize]
    private async Task<IResult> SearchByName(string query, IHotelRepository repos) =>
        await repos.GetHotelsAsync(query) is IEnumerable<Hotel> hotels
            ? Results.Ok(hotels)
            : Results.NotFound(Array.Empty<Hotel>());


    [Authorize]
    private async Task<IResult> SearchByCoordinate([FromBody] Coordinate coordinate, IHotelRepository repos) =>
        await repos.GetHotelsAsync(coordinate) is IEnumerable<Hotel> hotels
            ? Results.Ok(hotels)
            : Results.NotFound(Array.Empty<Hotel>());
}
