var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<HotelDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
    db.Database.EnsureCreated();
}


app.MapGet("/hotels", async (HotelDbContext db) => await db.Hotels.ToListAsync());

app.MapGet("/hotels/{id}", async (int id, HotelDbContext db) =>
{
    return await db.Hotels.FirstOrDefaultAsync(h => h.Id == id) is Hotel hotel ?
        Results.Ok(hotel) :
        Results.BadRequest();
});

app.MapPost("/hotels", async ([FromBody] Hotel hotel, HotelDbContext db) =>
{
    db.Hotels.Add(hotel);
    await db.SaveChangesAsync();

    return Results.Created($"hotels/{hotel.Id}", hotel);
});

app.MapPut("/hotels", async ([FromBody] Hotel hotel, HotelDbContext db) =>
{
    var hotelFromDb = await db.Hotels.FindAsync(new object[] { hotel.Id });
    if (hotelFromDb == null) return Results.NotFound();

    hotelFromDb.Latitude = hotel.Latitude;
    hotelFromDb.Longitude = hotel.Longitude;
    hotelFromDb.Name = hotel.Name;

    await db.SaveChangesAsync();

    return Results.NoContent();

});

app.MapDelete("/hotels/{id}", async (int id, HotelDbContext db) =>
{
    var hotelFromDb = await db.Hotels.FindAsync(new object[] { id });
    if (hotelFromDb == null) return Results.NotFound();

    db.Hotels.Remove(hotelFromDb);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.UseHttpsRedirection();

app.Run();


public class HotelDbContext : DbContext
{
    public DbSet<Hotel> Hotels => Set<Hotel>();

    public HotelDbContext(DbContextOptions<HotelDbContext> options)
        : base(options) { }

}

public class Hotel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}