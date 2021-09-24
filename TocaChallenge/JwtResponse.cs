namespace TocaChallenge
{
    public class JwtResponse
    {
        public string SessionId { get; set; }
        public string ServiceId { get; set; }
        public string Status { get; set; }
        public string Exception { get; set; }
        public JwtContent Content { get; set; }
       
    }

    public class JwtContent
    {
        public string Token { get; set; }
    }
}
