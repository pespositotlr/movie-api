# Movie API

## Overview

This is a test API built in .NET Core 3.1 for adding movie reivew to a data store. It doesn't currently have functionality for adding movies or users, those are setup by default. For a datastore I'm using SQLite. It uses EntityFramework for access of the database. I tried to setup requests and updates using SOLID design principles.

If a call is made where no values are found a 404 Not Found response should be returned. If the request is invalid, a 400 Bad Request response should be returned. Upon a valid request a 200 OK response should be returned.

Each movie has a calculated average rating which is rounded to the nearest half-number (1, 1.5, 2, 2.5, etc.) out of a maximum of 5.

Running Time is displayed in minutes.

### API FUnctions

There are four main API functions:

1. Get Movie

Gets a single movie by ID. This is a GET request.

Example: https://localhost:44357/movies/get-movie/1

2. Also known as "API A". Gets movies by filter. This is a GET request. The genre list is "any of these". So if you select genre 1 and genre 2, it will get movies which have either 1 OR 2.

Examples: https://localhost:44357/movies/A?yearOfRelease=1977

https://localhost:44357/movies/A?genres=1&genres=2

https://localhost:44357/movies/A?title=Alien

3. Also known as "API B". Gets the top 5 movies based on the total average rating across all users. This is a GET request.

Example: https://localhost:44357/movies/B

4. Also known as "API C". Gets the top 5 movies based on a certain user's ratings. This is a GET request.

Example: https://localhost:44357/movies/C?userId=1

5. Also known as "API D". This adds or updates a rating for a movie. This is a PUT requrst.

Example URL: https://localhost:44357/movies/D

Example JSON body:

{ 
"UserId" : 1,
"MovieId" : 7,
"Rating": 4
}

### Built-In Data

The design of this is to use existing data for movies and users, the only ability to mutate the data is by adding user reviews. You can view this data by opening the "MoviesDB.db" file located in the "MoviesAPI" folder in SQLiteStudio.

But for reference I'll put the test data here:

Movie
```
Id	Title	YearOfRelease	RunningTime
1	Star Wars	1977	121
2	A Fistful of Dollars	1964	99
3	Alien	1979	117
4	The Godfather	1972	177
5	Mission: Impossible	1996	110
6	Citizen Kane	1941	119
7	Back to the Future	1985	116
```

Genre
```
Id	Name
1	Action
2	Sci-Fi
3	Comedy
4	Western
5	Romance
6	Drama
7	Romantic comedy
8	Thriller
9	Documentary
```

User
```
Id	Username	CreateDate
1	Jim	12/9/2020
2	Bob	12/10/2020
3	Frank	12/11/2020
4	William	12/7/2020
5	Jackie	1/7/2019
6	Don	12/7/2019
7	Melissa	12/7/2020
8	Franklin	6/7/2020
9	George	1/7/2020
10	Anne	2/7/2020
```

UserReview
```
ReviewId	UserId	MovieId	Rating
1	1	1	4
2	2	1	4
3	3	1	4
4	4	1	4
5	5	1	4
6	6	1	3.5
7	7	1	4
8	8	1	4
9	9	1	4
10	10	1	3
11	1	2	4
12	1	3	4
13	1	4	4
14	1	5	4
15	1	6	4
16	1	7	4
17	2	2	3
18	2	3	2
19	2	4	3.5
20	2	5	3.5
21	2	6	3
22	2	7	3
23	3	5	3
24	3	7	3.5
25	4	7	3.5
26	5	7	3.5
27	6	7	3.5
28	6	4	1
29	6	2	4
30	7	2	4
31	8	2	4
32	8	7	3
```

GenreMovieAssignments
```
AssignmentId	MovieId	GenreId
1	1	2
2	1	1
3	2	4
4	2	6
5	1	6
6	3	2
7	3	8
8	3	1
9	4	6
10	4	8
11	5	8
12	5	1
13	6	6
14	6	5
15	7	2
16	7	1
17	7	4
```

### Tests

I also added unit tests in a separate project that seeds its own data into the SQLite database.
