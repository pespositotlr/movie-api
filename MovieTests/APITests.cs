using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using MoviesAPI.Controllers;
using Xunit;
using MoviesAPI;
using MoviesAPI.Services;
using MoviesAPI.Data;

namespace MovieTests
{
    public class APITests
    {
        private TestServer _server;
        public const string DatabaseFileName = "MoviesDB.db";

        public HttpClient Client { get; private set; }

        public APITests()
        {
            SetUpClient();
        }

        private void SetUpClient()
        {

            var builder = new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureServices(services =>
                {
                    var context = new MovieDBContext(new DbContextOptionsBuilder<MovieDBContext>()
                        .UseSqlite("DataSource=:memory:")
                        .EnableSensitiveDataLogging()
                        .Options);

                    services.RemoveAll(typeof(MovieDBContext));
                    services.AddSingleton(context);

                    context.Database.OpenConnection();
                    context.Database.EnsureCreated();

                    context.AddRange(GetMoviesSeedData());
                    context.AddRange(GetUsersSeedData());
                    context.AddRange(GetGenresSeedData());
                    context.AddRange(GetUserReviewsSeedData());
                    context.AddRange(GetMovieGenreAssignmentSeedData());

                    context.SaveChanges();

                    // Clear local context cache
                    foreach (var entity in context.ChangeTracker.Entries().ToList())
                    {
                        entity.State = EntityState.Detached;
                    }
                });

            _server = new TestServer(builder);

            Client = _server.CreateClient();
        }

        private List<Movie> GetMoviesSeedData()
        {
            var movies = new List<Movie>();
            movies.Add(new Movie() { Id = 1, Title = "Star Wars", YearOfRelease = 1977, RunningTime = 121 });
            movies.Add(new Movie() { Id = 2, Title = "A Fistful of Dollars", YearOfRelease = 1964, RunningTime = 99 });
            movies.Add(new Movie() { Id = 3, Title = "Alien", YearOfRelease = 1979, RunningTime = 117 });
            movies.Add(new Movie() { Id = 4, Title = "The Godfather", YearOfRelease = 1972, RunningTime = 177 });
            movies.Add(new Movie() { Id = 5, Title = "Mission: Impossible", YearOfRelease = 1996, RunningTime = 110 });
            movies.Add(new Movie() { Id = 6, Title = "Citizen Kane", YearOfRelease = 1941, RunningTime = 119 });
            movies.Add(new Movie() { Id = 7, Title = "Back to the Future", YearOfRelease = 1985, RunningTime = 116 });

            return movies;
        }

        private List<User> GetUsersSeedData()
        {
            var users = new List<User>();
            users.Add(new User() { Id = 1, Name = "User 1"});
            users.Add(new User() { Id = 2, Name = "User 2"});
            users.Add(new User() { Id = 3, Name = "User 3"});
            users.Add(new User() { Id = 4, Name = "User 4" });

            return users;
        }

        private List<Genre> GetGenresSeedData()
        {
            var genres = new List<Genre>();
            genres.Add(new Genre() { Id = 1, Name = "Genre 1" });
            genres.Add(new Genre() { Id = 2, Name = "Genre 2" });
            genres.Add(new Genre() { Id = 3, Name = "Genre 3" });

            return genres;
        }

        private List<UserReview> GetUserReviewsSeedData()
        {
            var userReviews = new List<UserReview>();
            userReviews.Add(new UserReview() { ReviewId = 1, UserId = 1, MovieId = 1, Rating = 3.0 });
            userReviews.Add(new UserReview() { ReviewId = 2, UserId = 2, MovieId = 1, Rating = 4.0 });
            userReviews.Add(new UserReview() { ReviewId = 3, UserId = 3, MovieId = 1, Rating = 3.0 });

            userReviews.Add(new UserReview() { ReviewId = 4, UserId = 1, MovieId = 2, Rating = 4.0 });
            userReviews.Add(new UserReview() { ReviewId = 5, UserId = 2, MovieId = 2, Rating = 4.0 });
            userReviews.Add(new UserReview() { ReviewId = 6, UserId = 3, MovieId = 2, Rating = 5.0 });

            userReviews.Add(new UserReview() { ReviewId = 7, UserId = 1, MovieId = 3, Rating = 1.0 });
            userReviews.Add(new UserReview() { ReviewId = 8, UserId = 2, MovieId = 3, Rating = 2.0 });
            userReviews.Add(new UserReview() { ReviewId = 9, UserId = 3, MovieId = 3, Rating = 5.0 });

            userReviews.Add(new UserReview() { ReviewId = 10, UserId = 1, MovieId = 4, Rating = 2.0 });
            userReviews.Add(new UserReview() { ReviewId = 11, UserId = 2, MovieId = 4, Rating = 3.0 });
            userReviews.Add(new UserReview() { ReviewId = 12, UserId = 3, MovieId = 4, Rating = 4.0 });

            userReviews.Add(new UserReview() { ReviewId = 13, UserId = 1, MovieId = 4, Rating = 3.0 });
            userReviews.Add(new UserReview() { ReviewId = 14, UserId = 2, MovieId = 4, Rating = 4.0 });
            userReviews.Add(new UserReview() { ReviewId = 15, UserId = 3, MovieId = 4, Rating = 4.0 });

            userReviews.Add(new UserReview() { ReviewId = 16, UserId = 1, MovieId = 5, Rating = 4.0 });
            userReviews.Add(new UserReview() { ReviewId = 17, UserId = 2, MovieId = 5, Rating = 2.0 });
            userReviews.Add(new UserReview() { ReviewId = 18, UserId = 3, MovieId = 5, Rating = 1.0 });

            userReviews.Add(new UserReview() { ReviewId = 19, UserId = 1, MovieId = 6, Rating = 3.0 });
            userReviews.Add(new UserReview() { ReviewId = 20, UserId = 2, MovieId = 6, Rating = 4.0 });
            userReviews.Add(new UserReview() { ReviewId = 21, UserId = 3, MovieId = 6, Rating = 5.0 });

            userReviews.Add(new UserReview() { ReviewId = 22, UserId = 1, MovieId = 7, Rating = 4.0 });

            return userReviews;
        }

        private List<MovieGenreAssignment> GetMovieGenreAssignmentSeedData()
        {
            var gmas = new List<MovieGenreAssignment>();
            gmas.Add(new MovieGenreAssignment() { AssignmentId = 1, GenreId = 1, MovieId = 1 });
            gmas.Add(new MovieGenreAssignment() { AssignmentId = 2, GenreId = 1, MovieId = 2 });
            gmas.Add(new MovieGenreAssignment() { AssignmentId = 3, GenreId = 1, MovieId = 3 });
            gmas.Add(new MovieGenreAssignment() { AssignmentId = 4, GenreId = 1, MovieId = 4 });
            gmas.Add(new MovieGenreAssignment() { AssignmentId = 5, GenreId = 1, MovieId = 5 });
            gmas.Add(new MovieGenreAssignment() { AssignmentId = 6, GenreId = 1, MovieId = 6 });
            gmas.Add(new MovieGenreAssignment() { AssignmentId = 7, GenreId = 1, MovieId = 7 });
            gmas.Add(new MovieGenreAssignment() { AssignmentId = 8, GenreId = 2, MovieId = 1 });
            gmas.Add(new MovieGenreAssignment() { AssignmentId = 9, GenreId = 2, MovieId = 2 });
            gmas.Add(new MovieGenreAssignment() { AssignmentId = 10, GenreId = 3, MovieId = 3 });

            return gmas;
        }

        private class APIGetResponse
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public int YearOfRelease { get; set; }
            public int RunningTime { get; set; }
            public string Genres { get; set; }
            public decimal AverageRating { get; set; }
        }

        [Fact]
        public async Task TestGetMovie()
        {
            var response0 = await Client.GetAsync("/movies/get-movie/1");
            Assert.Equal(System.Net.HttpStatusCode.OK, response0.StatusCode);

            var realData0 = JsonConvert.DeserializeObject(response0.Content.ReadAsStringAsync().Result);
            var expectedData0 = JsonConvert.DeserializeObject("{\"id\":1,\"title\":\"Star Wars\",\"yearOfRelease\":1977,\"runningTime\":121,\"genres\":\"1,2\",\"averageRating\":3.5}");
            Assert.Equal(expectedData0, realData0);
        }

        [Fact]
        public async Task Test_API_A_YearOfRelease()
        {
            var response0 = await Client.GetAsync("/movies/A?yearOfRelease=1977");
            Assert.Equal(System.Net.HttpStatusCode.OK, response0.StatusCode);

            var realData0 = JsonConvert.DeserializeObject(response0.Content.ReadAsStringAsync().Result);
            var expectedData0 = JsonConvert.DeserializeObject("[{\"id\":1,\"title\":\"Star Wars\",\"yearOfRelease\":1977,\"runningTime\":121,\"genres\":\"1,2\",\"averageRating\":3.5}]");
            Assert.Equal(expectedData0, realData0);
        }

        [Fact]
        public async Task Test_API_A_Title()
        {
            var response0 = await Client.GetAsync("/movies/A?title=Alien");
            Assert.Equal(System.Net.HttpStatusCode.OK, response0.StatusCode);

            var realData0 = JsonConvert.DeserializeObject(response0.Content.ReadAsStringAsync().Result);
            var expectedData0 = JsonConvert.DeserializeObject("[{\"id\":3,\"title\":\"Alien\",\"yearOfRelease\":1979,\"runningTime\":117,\"genres\":\"1,3\",\"averageRating\":2.5}]");
            Assert.Equal(expectedData0, realData0);
        }

        [Fact]
        public async Task Test_API_A_Genres()
        {
            var response0 = await Client.GetAsync("/movies/A?genres=2&genres=3");
            Assert.Equal(System.Net.HttpStatusCode.OK, response0.StatusCode);

            var realData0 = JsonConvert.DeserializeObject(response0.Content.ReadAsStringAsync().Result);
            var expectedData0 = JsonConvert.DeserializeObject("[{\"id\":2,\"title\":\"A Fistful of Dollars\",\"yearOfRelease\":1964,\"runningTime\":99,\"genres\":\"1,2\",\"averageRating\":4.5},{\"id\":3,\"title\":\"Alien\",\"yearOfRelease\":1979,\"runningTime\":117,\"genres\":\"1,3\",\"averageRating\":2.5},{\"id\":1,\"title\":\"Star Wars\",\"yearOfRelease\":1977,\"runningTime\":121,\"genres\":\"1,2\",\"averageRating\":3.5}]");
            Assert.Equal(expectedData0, realData0);
        }

        [Fact]
        public async Task Test_API_A_NotFound()
        {
            var response0 = await Client.GetAsync("/movies/A?title=ABCDEGF");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response0.StatusCode);
        }

        [Fact]
        public async Task Test_API_A_BadRequest()
        {
            var response0 = await Client.GetAsync("/movies/A");
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response0.StatusCode);
        }

        [Fact]
        public async Task Test_API_B()
        {
            var response0 = await Client.GetAsync("/movies/B");
            Assert.Equal(System.Net.HttpStatusCode.OK, response0.StatusCode);

            var realData0 = JsonConvert.DeserializeObject<List<APIGetResponse>>(response0.Content.ReadAsStringAsync().Result);
            Assert.Equal(5, realData0.Count);
        }

        [Fact]
        public async Task Test_API_C()
        {
            var response0 = await Client.GetAsync("/movies/C?userId=1");
            Assert.Equal(System.Net.HttpStatusCode.OK, response0.StatusCode);

            var realData0 = JsonConvert.DeserializeObject<List<APIGetResponse>>(response0.Content.ReadAsStringAsync().Result);
            Assert.Equal(5, realData0.Count);
        }

        [Fact]
        public async Task Test_API_C_BadRequest()
        {
            var response0 = await Client.GetAsync("/movies/C?userId=0");
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response0.StatusCode);
        }

        [Fact]
        public async Task Test_API_C_NoReviews()
        {
            var response0 = await Client.GetAsync("/movies/C?userId=4");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response0.StatusCode);
        }

        [Fact]
        public async Task Test_API_D_AddReview()
        {
            //Add a User 4 review for movie 1 at a rating of 2
            var addRequest0 = new MovieReviewRequest() { MovieId = 1, Rating = 2, UserId = 4 };
            var response0 = await Client.PutAsync("/movies/D", new StringContent(JsonConvert.SerializeObject(addRequest0), Encoding.UTF8, "application/json"));
            Assert.Equal(System.Net.HttpStatusCode.OK, response0.StatusCode);
        }

        [Fact]
        public async Task Test_API_D_UpdateReview()
        {
            //Update user 1's review of movie 1 from 3 to 4
            var addRequest0 = new MovieReviewRequest() { MovieId = 1, Rating = 1, UserId = 4 };
            var response0 = await Client.PutAsync("/movies/D", new StringContent(JsonConvert.SerializeObject(addRequest0), Encoding.UTF8, "application/json"));
            Assert.Equal(System.Net.HttpStatusCode.OK, response0.StatusCode);
        }

        [Fact]
        public async Task Test_API_D_NotFound()
        {
            var addRequest0 = new MovieReviewRequest() { MovieId = 123, Rating = 1, UserId = 4 };
            var response0 = await Client.PutAsync("/movies/D", new StringContent(JsonConvert.SerializeObject(addRequest0), Encoding.UTF8, "application/json"));
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response0.StatusCode);
        }

        [Fact]
        public async Task Test_API_D_BadRequest()
        {
            var addRequest0 = new MovieReviewRequest() { MovieId = 1, Rating = 6, UserId = 4 };
            var response0 = await Client.PutAsync("/movies/D", new StringContent(JsonConvert.SerializeObject(addRequest0), Encoding.UTF8, "application/json"));
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response0.StatusCode);
        }
    }
}
