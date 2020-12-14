using Microsoft.EntityFrameworkCore;
using MoviesAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Services
{
    public class MovieDBService : IMovieCollectionService
    {
        private readonly MovieDBContext _dbContext;

        public MovieDBService(MovieDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Movie>> GetMoviesByFilter(MovieFilter filter)
        {
            IQueryable<Movie> movies = _dbContext.Movies
                .Include(x => x.MovieGenreAssignments).ThenInclude(x => x.Genre)
                .Include(x => x.UserReviews);

            if (filter == null)
                filter = new MovieFilter();

            if (!String.IsNullOrEmpty(filter.Title))
                movies = movies.Where(x => filter.Title == x.Title);

            if (filter.YearOfRelease > 0)
                movies = movies.Where(x => filter.YearOfRelease == x.YearOfRelease);

            //I'm making the assumption that the Genres in the filter is an "or" statement, so return any movies that contain any of the user's input genres
            if (filter.Genres != null && filter.Genres.Any())
                movies = movies.Where(x => x.MovieGenreAssignments.Any(y => filter.Genres.Contains(y.Genre.Id)));

            return await movies.OrderBy(o => o.Title).ToListAsync();
        }

        public async Task<List<Movie>> GetTopRatedMovies(int numberToRetrieve)
        {
            List<Movie> movies = await _dbContext.Movies
                .Include(x => x.MovieGenreAssignments).ThenInclude(x => x.Genre)
                .Include(x => x.UserReviews)
                .Where(x => x.UserReviews.Count > 0).ToListAsync();

            return movies.OrderByDescending(o => o.AverageScore).ThenBy(o => o.Title).Take(numberToRetrieve).ToList();
        }

        public async Task<List<Movie>> GetTopRatedMoviesByUser(int userId, int numberToRetrieve)
        {
            List<Movie> movies = await _dbContext.Movies
                .Include(x => x.MovieGenreAssignments).ThenInclude(x => x.Genre)
                .Include(x => x.UserReviews)
                .Where(x => x.UserReviews.Count > 0 && x.UserReviews.Any(y => y.UserId == userId)).ToListAsync();

            return movies.OrderByDescending(o => o.UserReviews.FirstOrDefault(x => x.UserId == userId).Rating).ThenBy(o => o.Title).Take(numberToRetrieve).ToList();
        }

        public async Task<UserReview> AddUpdateReview(MovieReviewRequest request)
        {
            if (_dbContext.UserReviews.Any(x => x.UserId == request.UserId && x.MovieId == request.MovieId))
            {
                return await UpdateReview(request);
            } else
            {
                return await AddReview(request);
            }
        }

        protected async Task<UserReview> AddReview(MovieReviewRequest request)
        {
            UserReview newReview = new UserReview()
            {
                UserId = request.UserId,
                MovieId = request.MovieId,
                Rating = request.Rating
            };

            await _dbContext.UserReviews.AddAsync(newReview);

            await _dbContext.SaveChangesAsync();

            return newReview;
        }
        protected async Task<UserReview> UpdateReview(MovieReviewRequest request)
        {
            UserReview existingReview = _dbContext.UserReviews.FirstOrDefault(x => x.UserId == request.UserId && x.MovieId == request.MovieId);

            existingReview.Rating = request.Rating;

            _dbContext.UserReviews.Update(existingReview);

            await _dbContext.SaveChangesAsync();

            return existingReview;
        }

        public async Task<Movie> AddMovie(Movie movie)
        {
            await _dbContext.Movies.AddAsync(movie);

            await _dbContext.SaveChangesAsync();

            return movie;
        }

        public async Task<bool> DeleteMovie(int id)
        {
            Movie movieToDelete = await _dbContext.Movies.Where(x => x.Id == id)
                .Include(x => x.MovieGenreAssignments)
                .ThenInclude(x => x.Genre)
                .FirstOrDefaultAsync();

            _dbContext.Movies.Remove(movieToDelete);

            await _dbContext.SaveChangesAsync();

            return true;
        }
    }

    public interface IMovieCollectionService
    {
        Task<List<Movie>> GetMoviesByFilter(MovieFilter filters);

        Task<List<Movie>> GetTopRatedMovies(int numberToRetrieve);

        Task<List<Movie>> GetTopRatedMoviesByUser(int userId, int numberToRetrieve);

        Task<UserReview> AddUpdateReview(MovieReviewRequest request);

        Task<Movie> AddMovie(Movie employee);

        Task<bool> DeleteMovie(int id);
    }

    public class MovieFilter
    {
        public string Title { get; set; }
        public int YearOfRelease { get; set; }
        public int[] Genres { get; set; }
    }
}
