namespace LocalSearchEngine.ViewModels.Components
{
    public class SearchResultItemViewModel : ViewModelBase
    {
        private string title;
        private string excerpt;

        public string Title { get => title; set => this.SetValue(ref title, value); }
        public string Excerpt { get => excerpt; set => this.SetValue(ref excerpt, value); }
        public int DocumentId { get; set; }
        public int Relevance { get; set; }
        public IReadOnlyCollection<string> HighlightedWords { get; set; }
    }
}
