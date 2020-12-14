using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesAPI.Data
{
    public class MovieDBContext : DbContext
    {
        public MovieDBContext(DbContextOptions<MovieDBContext> options)
            : base(options)
        { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<MovieGenreAssignment> MovieGenreAssignments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserReview> UserReviews { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //Setup table relationships
            builder.Entity<User>()
                .HasMany(c => c.UserReviews)
                .WithOne(e => e.User);

            builder.Entity<UserReview>().HasKey(p => new { p.ReviewId });
            builder.Entity<UserReview>()
                .HasOne(m => m.Movie)
                .WithMany(u => u.UserReviews)
                .HasForeignKey(r => r.MovieId);

            builder.Entity<MovieGenreAssignment>().HasKey(p => new { p.MovieId, p.GenreId });
            builder.Entity<MovieGenreAssignment>()
                .HasOne(m => m.Movie)
                .WithMany(g => g.MovieGenreAssignments)
                .HasForeignKey(mga => mga.MovieId);

            base.OnModelCreating(builder);
        }
    }

    [Table("Genre")]
    public class Genre
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [Table("Movie")]
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public int YearOfRelease { get; set; }

        public int RunningTime { get; set; }

        public ICollection<MovieGenreAssignment> MovieGenreAssignments { get; set; }

        public ICollection<UserReview> UserReviews { get; set; }

        [NotMapped]
        private double? _averageScore = null;

        [NotMapped]
        public double AverageScore { get
            {
                if (_averageScore != null)
                    return (double)this._averageScore;

                if (this.UserReviews.Count == 0)
                    return 0;

                double totalReviewScore = 0;
                foreach (UserReview review in this.UserReviews)
                    totalReviewScore += review.Rating;

                this._averageScore = totalReviewScore / this.UserReviews.Count;

                return (double)this._averageScore;
            } 
        }
    }

    [Table("MovieGenreAssignment")]
    public class MovieGenreAssignment
    {
        [Key]
        public int AssignmentId { get; set; }

        public int MovieId { get; set; }

        public int GenreId { get; set; }

        public Movie Movie { get; set; }

        public Genre Genre { get; set; }
    }

    [Table("User")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<UserReview> UserReviews { get; set; }
    }

    [Table("UserReview")]
    public class UserReview
    {
        [Key]
        public int ReviewId { get; set; }

        public int UserId { get; set; }

        public int MovieId { get; set; }

        public double Rating { get; set; }

        public User User { get; set; }

        public Movie Movie { get; set; }
    }

}