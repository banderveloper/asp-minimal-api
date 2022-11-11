using HotelsWebApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<HotelDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
});
builder.Services.AddScoped<IHotelRepository, HotelRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
    db.Database.EnsureCreated();
}


app.MapGet("/hotels", async (IHotelRepository repos) => 
await repos.GetHotelsAsync());

app.MapGet("/hotels/{id}", async (int id, IHotelRepository repos) =>
{
    return await repos.GetHotelAsync(id) is Hotel hotel ?
        Results.Ok(hotel) :
        Results.BadRequest();
});

app.MapPost("/hotels", async ([FromBody] Hotel hotel, IHotelRepository repos) =>
{
    await repos.InsertHotelAsync(hotel);
    await repos.SaveAsync();

    return Results.Created($"hotels/{hotel.Id}", hotel);
});

app.MapPut("/hotels", async ([FromBody] Hotel hotel, IHotelRepository repos) =>
{
    await repos.UpdateHotelAsync(hotel);
    await repos.SaveAsync();

    return Results.NoContent();
});

app.MapDelete("/hotels/{id}", async (int id, IHotelRepository repos) =>
{
    await repos.DeleteHotelAsync(id);
    await repos.SaveAsync();

    return Results.NoContent();
});

app.UseHttpsRedirection();

app.Run();



