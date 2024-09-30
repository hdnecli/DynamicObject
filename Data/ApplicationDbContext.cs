using Microsoft.EntityFrameworkCore;

public class DynamicObjectContext : DbContext
{
    public DynamicObjectContext(DbContextOptions<DynamicObjectContext> options) : base(options) { }
    public DbSet<Object> Objects { get; set; }
    public DbSet<Field> Fields { get; set;}
}