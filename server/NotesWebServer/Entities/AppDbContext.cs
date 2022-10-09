using Microsoft.EntityFrameworkCore;

namespace NotesWebServer.Entities;

public class AppDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public AppDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(u => u.Username)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(u => u.HashedPassword)
            .IsRequired();

        modelBuilder.Entity<Role>()
            .Property(r => r.Name)
            .IsRequired();


        modelBuilder.Entity<Role>()
            .HasData(new Role()
                {
                    Id = 1,
                    Name = "User"
                },
                new Role()
                {
                    Id = 2,
                    Name = "Admin"
                });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_configuration.GetConnectionString("connectionString"));
    }
}