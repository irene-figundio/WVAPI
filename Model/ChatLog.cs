namespace AI_Integration.Model
{
    public class ChatLog
    {
        public string Object { get; set; }
        public List<ThreadMessage> Data { get; set; }
        public string first_id { get; set; }
        public string last_id { get; set; }
        public bool has_more { get; set; }
    }
    public class ThreadMessage
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public long created_at { get; set; }
        public string assistant_id { get; set; }
        public string thread_id { get; set; }
        public string run_id { get; set; }
        public string Role { get; set; }
        public List<Content> Content { get; set; }
        public List<string> file_ids { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }

    public class Content
    {
        public string Type { get; set; }
        public Text Text { get; set; }
    }

    public class Text
    {
        public string Value { get; set; }
        public List<object> Annotations { get; set; }
    }

}
