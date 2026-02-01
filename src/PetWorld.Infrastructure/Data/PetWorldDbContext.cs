using Microsoft.EntityFrameworkCore;
using PetWorld.Domain.Entities;

namespace PetWorld.Infrastructure.Data;

public class PetWorldDbContext : DbContext
{
    public PetWorldDbContext(DbContextOptions<PetWorldDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<ChatSession> ChatSessions => Set<ChatSession>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).HasMaxLength(200).IsRequired();
            entity.Property(p => p.Description).HasMaxLength(1000);
            entity.Property(p => p.Price).HasPrecision(10, 2);
            entity.Property(p => p.TargetPet).HasMaxLength(50);
            entity.Property(p => p.Category).HasConversion<string>().HasMaxLength(50);
        });

        modelBuilder.Entity<ChatSession>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.UserQuestion).HasMaxLength(2000).IsRequired();
            entity.Property(c => c.FinalResponse).HasColumnType("text");
            entity.HasMany(c => c.Messages)
                  .WithOne(m => m.ChatSession)
                  .HasForeignKey(m => m.ChatSessionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Role).HasMaxLength(20).IsRequired();
            entity.Property(m => m.Content).HasColumnType("text");
        });
    }
}
