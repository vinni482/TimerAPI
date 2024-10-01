namespace TimerAPITest.Repositories.Entities
{
    public class TimerDBEntity
    {
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
        public string WebhookUrl { get; set; }
        public string Status { get; set; }

        public DateTime ExpirationTime => DateCreated.AddHours(Hours).AddMinutes(Minutes).AddSeconds(Seconds);
        public bool HasExpired => DateTime.UtcNow >= ExpirationTime;
    }
}
