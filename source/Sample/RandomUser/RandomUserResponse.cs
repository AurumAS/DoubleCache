using System;
using System.Collections.Generic;

namespace RandomUser
{
    [Serializable]
    public class RandomUserResponse
    {
        public List<Result> Results { get; set; }
    }

    [Serializable]
    public class Result
    {
        public User User { get; set; }
        public string Seed { get; set; }
    }
}
