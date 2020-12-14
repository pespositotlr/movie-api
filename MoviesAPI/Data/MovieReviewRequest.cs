using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Data
{
    public class MovieReviewRequest
    {
        public int UserId { get; set; }

        public int MovieId { get; set; }

        public double Rating { get; set; }

    }
}
