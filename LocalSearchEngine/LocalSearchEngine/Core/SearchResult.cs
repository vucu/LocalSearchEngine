namespace LocalSearchEngine.Core
{
    public class SearchResult
    {
        public bool Success { get; set; }
        public string Title { get; set; } = "???";
        public string Excerpt { get; set; } = string.Empty;
        public int Relevance { get; set; }
        public int DocumentId { get; set; }
    }
}
