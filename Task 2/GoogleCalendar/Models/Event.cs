namespace GoogleCalendar.Models
{
    public class Event
    {
        public string eventId { get; set; }=string.Empty;
        public string EventTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
