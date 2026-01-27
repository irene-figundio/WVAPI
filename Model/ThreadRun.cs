namespace AI_Integration.Model
{
    public class ThreadRun
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public long created_at { get; set; }
        public string assistant_id { get; set; }
        public string thread_id { get; set; }
        public string Status { get; set; }
        public object started_at { get; set; }
        public long expires_at { get; set; }
        public object cancelled_at { get; set; }
        public object failed_at { get; set; }
        public object completed_at { get; set; }
        public object required_action { get; set; }
        public object last_error { get; set; }
        public string Model { get; set; }
        public string Instructions { get; set; }
        public List<object> Tools { get; set; }
        public List<object> file_ids { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
        public double Temperature { get; set; }
        public double top_p { get; set; }
        public object max_completion_tokens { get; set; }
        public object max_prompt_tokens { get; set; }
        public TruncationStrategy truncation_strategy { get; set; }
        public object incomplete_details { get; set; }
        public object Usage { get; set; }
        public string response_format { get; set; }
        public string tool_choice { get; set; }
    }
    public class TruncationStrategy
    {
        public string Type { get; set; }
        public object last_messages { get; set; }
    }
}
