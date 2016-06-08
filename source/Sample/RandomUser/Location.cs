using System;

namespace RandomUser
{
    [Serializable]
    public class Location
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
    }
}
