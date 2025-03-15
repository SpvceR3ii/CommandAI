namespace CommandAI.Services
{
    public class GitIgnoreService
    {
        private readonly HashSet<string> _patterns = new();

        public void LoadGitIgnore(string directory)
        {
            string gitIgnorePath = Path.Combine(directory, ".gitignore");
            if (!File.Exists(gitIgnorePath)) return;

            string[] patterns = File.ReadAllLines(gitIgnorePath);
            foreach (string pattern in patterns)
            {
                if (string.IsNullOrWhiteSpace(pattern) || pattern.StartsWith("#")) continue;
                _patterns.Add(pattern.Trim());
            }
        }

        public bool ShouldIgnore(string path)
        {
            foreach (string pattern in _patterns)
            {
                if (IsMatch(path, pattern)) return true;
            }
            return false;
        }

        private bool IsMatch(string path, string pattern)
        {
            // Simple pattern matching - you might want to use a more robust solution
            if (pattern.EndsWith("/"))
            {
                return path.Contains(pattern) || path.Contains(pattern.TrimEnd('/'));
            }
            if (pattern.StartsWith("*"))
            {
                return path.EndsWith(pattern.TrimStart('*'));
            }
            return path.Contains(pattern);
        }
    }
}