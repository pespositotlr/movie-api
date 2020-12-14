using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoviesAPI.Data;
using MoviesAPI.Helpers;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MoviesController : ControllerBase
    {

        private readonly ILogger<MoviesController> _logger;

        MovieDBContext _dbContext;
        IMovieCollectionService _movieCollectionService;

        public MoviesController(ILogger<MoviesController> logger, MovieDBContext dbcontext, IMovieCollectionService movieCollectionService)
        {
            _logger = logger;
            _dbContext = dbcontext;
            _movieCollectionService = movieCollectionService;
        }

        [HttpGet]
        [Route("get-movie/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MovieViewModel>> GetMovie(int id)
        {
            var movie = await _dbContext.Movies.Where(x => x.Id == id)
                .Include(x => x.MovieGenreAssignments).ThenInclude(x => x.Genre)
                .Include(x => x.UserReviews)
                .FirstOrDefaultAsync();

            if (movie != null)
            {
                return Ok(MovieHelper.GetMovieViewModel(movie));
            }

            return NotFound();
        }

        [HttpGet]
        [Route("A")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MovieViewModel>> API_A([FromQuery] MovieFilter filter)
        {
            if (!MovieHelper.IsValidMovieFilter(filter))
            {
                return BadRequest();
            }

            List<Movie> movies = await _movieCollectionService.GetMoviesByFilter(filter);

            if (movies.Count > 0)
            {
                return Ok(MovieHelper.GetMovieViewModels(movies));
            }

            return NotFound();
        }

        [HttpGet]
        [Route("B")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MovieViewModel>> API_B([FromQuery] int numberToRetrieve = 5)
        {
            List<Movie> movies = await _movieCollectionService.GetTopRatedMovies(numberToRetrieve);

            if (movies.Count > 0)
            {
                return Ok(MovieHelper.GetMovieViewModels(movies));
            }

            return NotFound();
        }

        [HttpGet]
        [Route("C")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MovieViewModel>> API_C([FromQuery] int userID = 0, int numberToRetrieve = 5)
        {
            if (userID == 0)
            {
                return BadRequest();
            }

            List<Movie> movies = await _movieCollectionService.GetTopRatedMoviesByUser(userID, numberToRetrieve);

            if (movies.Count > 0)
            {
                return Ok(MovieHelper.GetMovieViewModels(movies));
            }

            return NotFound();
        }


        [HttpPut]
        [Route("D")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MovieViewModel>> API_D([FromBody] MovieReviewRequest request)
        {
            if (!MovieHelper.IsValidMovieId(request.MovieId, _dbContext))
            {
                return NotFound();
            }

            if (!MovieHelper.IsValidUserId(request.UserId, _dbContext))
            {
                return NotFound();
            }

            if (!MovieHelper.IsValidMovieRating(request.Rating))
            {
                return BadRequest();
            }

            await _movieCollectionService.AddUpdateReview(request);

            return Ok();
        }
    }
}
