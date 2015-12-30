
using System;

namespace RandomUser
{
    [Serializable]
    public class User
    {
        public Name Name { get; set; }
        public Location Location { get; set; }
        public Picture Picture { get; set; }

        public string Gender { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Md5 { get; set; }
        public string Sha1 { get; set; }
        public string Sha256 { get; set; }
        public string Registered { get; set; }
        public string Dob { get; set; }
        public string Phone { get; set; }
        public string Cell { get; set; }
        public string SSN { get; set; }
        public string version { get; set; }
    }
}
