namespace MV4.Models
{
    public class Vote
    {
        public string TrackId { get; set; }
        public int Count { get; set; }
        public List<string> IPs { get; set; }
    }
}
