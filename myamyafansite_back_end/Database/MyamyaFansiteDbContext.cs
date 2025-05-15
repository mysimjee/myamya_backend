using Microsoft.EntityFrameworkCore;
using myamyafansite_back_end.Database.Model;

namespace myamyafansite_back_end.Database;

public class MyamyaFanSiteDbContext : DbContext
{
 
    public string DbPath { get; } = Path.Join(Environment.CurrentDirectory, "MyaMyaFanSiteDatabase.db");

    public DbSet<Account> Accounts { get; set; }
    public DbSet<LoginHistory> LoginHistories { get; set; }
    public DbSet<Video> Videos { get; set; }
    public DbSet<Picture> Pictures { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<ViewingHistory> ViewingHistories { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        try
        {
             base.OnModelCreating(modelBuilder);
             
             modelBuilder.Entity<Rating>()
                 .HasOne<Video>()
                 .WithMany()
                 .HasForeignKey("VideoId")
                 .OnDelete(DeleteBehavior.Cascade);

             modelBuilder.Entity<Rating>()
                 .HasOne<Picture>()
                 .WithMany()
                 .HasForeignKey("PictureId")
                 .OnDelete(DeleteBehavior.Cascade);

             modelBuilder.Entity<Comment>()
                 .HasOne<Video>()
                 .WithMany()
                 .HasForeignKey("VideoId")
                 .OnDelete(DeleteBehavior.Cascade);

             modelBuilder.Entity<Comment>()
                 .HasOne<Picture>()
                 .WithMany()
                 .HasForeignKey("PictureId")
                 .OnDelete(DeleteBehavior.Cascade);

             modelBuilder.Entity<ViewingHistory>()
                 .HasOne<Video>()
                 .WithMany()
                 .HasForeignKey("VideoId")
                 .OnDelete(DeleteBehavior.Cascade);

             modelBuilder.Entity<ViewingHistory>()
                 .HasOne<Picture>()
                 .WithMany()
                 .HasForeignKey("PictureId")
                 .OnDelete(DeleteBehavior.Cascade);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}