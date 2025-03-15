namespace CommandAI.Models
{
    public class Configuration
    {
        public string Model { get; set; } = "qwen2.5-coder:3b";
        public string Endpoint { get; set; } = "http://localhost:11434/api/generate";
        public int TimeoutSeconds { get; set; } = 300;
        public string[] ExcludePatterns { get; set; } = Array.Empty<string>();
        public string GitIgnoreFile { get; set; } = ".gitignore";
    }
}