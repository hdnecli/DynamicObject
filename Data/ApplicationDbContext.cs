using Microsoft.EntityFrameworkCore;

public class DynamicObjectContext : DbContext
{
    public DynamicObjectContext(DbContextOptions<DynamicObjectContext> options) : base(options) { }

    public DbSet<DynamicObject> DynamicObjects { get; set; }
}