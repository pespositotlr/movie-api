using MoviesAPI.Data;
using MoviesAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Helpers
{
    public class MovieHelper
    {

        public static List<MovieViewModel> GetMovieViewModels(List<Movie> movies)
        {
            List<MovieViewModel> movieViewModels = new List<MovieViewModel>();

            foreach(Movie movie in movies)
            {
                movieViewModels.Add(GetMovieViewModel(movie));
            }

            return movieViewModels;
        }

        public static MovieViewModel GetMovieViewModel(Movie movie)
        {
            MovieViewModel movieViewModel = new MovieViewModel(){
                Id = movie.Id,
                Title = movie.Title,
                YearOfRelease = movie.YearOfRelease,
                RunningTime = movie.RunningTime,
            };

            movieViewModel.Genres = GetGenresString(movie);
            movieViewModel.AverageRating = GetAverageRating(movie);

            return movieViewModel;
        }

        private static string GetGenresString(Movie movie)
        {
            StringBuilder sb = new StringBuilder();

            foreach(MovieGenreAssignment mga in movie.MovieGenreAssignments)
            {
                sb.Append(mga.GenreId);
                sb.Append(',');
            }

            var index = sb.ToString().LastIndexOf(',');
            if (index >= 0)
                sb.Remove(index, 1);

            return sb.ToString();
        }

        private static double GetAverageRating(Movie movie)
        {
            if (movie.UserReviews.Count == 0)
                return 0;

            //All review averages should be rounded to closest half (3, 3.5, 4 etc.)
            return GetDoubleToClosesHalf(movie.AverageScore);
        }

        private static double GetDoubleToClosesHalf(double value)
        {
            return Math.Round(value * 2, MidpointRounding.AwayFromZero) / 2;
        }

        internal static bool IsValidMovieFilter(MovieFilter filter)
        {
            //If no filter values are set, then the filter is considered invalid
            if (String.IsNullOrEmpty(filter.Title) && filter.YearOfRelease == 0 && (filter.Genres == null || filter.Genres.Length == 0))
                return false;

            return true;
        }

        internal static bool IsValidMovieId(int movieId, MovieDBContext dbContext)
        {
            //Movie must exist in the database
            return dbContext.Movies.Any(x => x.Id == movieId);
        }

        internal static bool IsValidUserId(int userId, MovieDBContext dbContext)
        {
            //User must exist in the database
            return dbContext.Users.Any(x => x.Id == userId);
        }

        internal static bool IsValidMovieRating(double rating)
        {
            //Only allow whole numbers from 1 to 5
            if (rating >= 1 && rating <= 5 && (rating % 1) == 0)
                return true;

            return false;
        }
    }
}
