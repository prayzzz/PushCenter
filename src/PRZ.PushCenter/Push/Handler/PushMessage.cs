namespace PRZ.PushCenter.Push.Handler
{
    public class PushMessage
    {
        public PushMessage(string body, string title)
        {
            Body = body;
            Title = title;
        }

        public string Body { get; }

        public string Title { get; }
    }
}