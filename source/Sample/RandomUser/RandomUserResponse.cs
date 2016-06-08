using System;
using System.Collections.Generic;

namespace RandomUser
{
    [Serializable]
    public class RandomUserResponse
    {
        public List<User> Results { get; set; }
        public Info Info { get; set; }
    }

    [Serializable]
    public class Info
    {
        public int Results { get; set; }
        public int Page { get; set; }
        public string Version { get; set; }
        public string Seed { get; set; }

    }

    [Serializable]
    public class Result
    {
        public User User { get; set; }
        public string Seed { get; set; }
    }
}
