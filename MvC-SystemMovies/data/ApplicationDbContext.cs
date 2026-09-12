using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MvC_SystemMovies.Models;

namespace MvC_SystemMovies.data
{
    // Inheriting from IdentityDbContext<ApplicationUser> instead of plain DbContext
    // adds the AspNetUsers / AspNetRoles / AspNetUserRoles ... tables to our schema
    // and wires our custom ApplicationUser as the Identity user type.
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Actors> Actors { get; set; }
        public DbSet<MovieActors> MovieActors { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ShippingCompany> ShippingCompanies { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Required so the Identity tables get configured correctly.
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MovieActors>()
                .HasKey(ma => new { ma.MovieId, ma.ActorId });

            // A user can only have one cart row / one favorite row per movie.
            modelBuilder.Entity<CartItem>()
                .HasIndex(c => new { c.UserId, c.MovieId })
                .IsUnique();

            modelBuilder.Entity<Favorite>()
                .HasIndex(f => new { f.UserId, f.MovieId })
                .IsUnique();

            // Restrict on Movie deletes so SQL Server doesn't reject the cascade
            // chain (Order -> OrderItem -> Movie plus Order -> Movie would be a
            // multiple cascade path otherwise).
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Movie)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.ShippingCompany)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.Movie)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Movie)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Movie)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cinema>().HasData(
                new Cinema { Id = 1, Name = "Cinema 1", image = "1.jpg" },
                new Cinema { Id = 2, Name = "Cinema 2", image = "2.jpg" },
                new Cinema { Id = 3, Name = "Cinema 3", image = "3.jpg" }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action" },
                new Category { Id = 2, Name = "Comedy" },
                new Category { Id = 3, Name = "Drama" }
            );

            modelBuilder.Entity<Actors>().HasData(
                new Actors { Id = 1, Name = "Actor 1", Img = "1.jpg" },
                new Actors { Id = 2, Name = "Actor 2", Img = "2.jpg" },
                new Actors { Id = 3, Name = "Actor 3", Img = "3.jpg" }
            );

            modelBuilder.Entity<Movie>().HasData(
                new Movie { Id = 1, Title = "Movie 1", Description = "Description 1", Price = 10.0m, Minigm = "1.jpg", Status = true, DateTime = new DateOnly(2026, 9, 8), CinemaId = 1, CategoryId = 1 },
                new Movie { Id = 2, Title = "Movie 2", Description = "Description 2", Price = 12.0m, Subimges = "2.jpg", Status = true, DateTime = new DateOnly(2026, 9, 8), CinemaId = 2, CategoryId = 2 },
                new Movie { Id = 3, Title = "Movie 3", Description = "Description 3", Price = 15.0m, Subimges = "3.jpg", Status = true, DateTime = new DateOnly(2026, 9, 8), CinemaId = 3, CategoryId = 3 }
            );
        }
    }
}
