namespace ToDoListBlazor.Shared
{
    public class InboxMessage
    {
        public string Subject { get; set; }
        public string From { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}