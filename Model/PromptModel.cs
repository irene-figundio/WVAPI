namespace AI_Integration.Model
{
    public class PromptModel
    {
        public string model { get; set; }
        public string name { get; set; }
        public List<Message> messages { get; set; }
        public int max_tokens { get; set; }
        public double temperature { get; set; }
        public double frequency_penalty { get; set; }
        public double presence_penalty { get; set; }
        public string stop { get; set; }
    }

    public class Message
    {
        public string role { get; set; }
        public string content { get; set; }

    }
}
