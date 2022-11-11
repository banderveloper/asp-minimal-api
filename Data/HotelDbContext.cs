namespace HotelsWebApi.Data
{
    public class HotelDbContext : DbContext
    {
        public DbSet<Hotel> Hotels => Set<Hotel>();

        public HotelDbContext(DbContextOptions<HotelDbContext> options)
            : base(options) { }

    }
}