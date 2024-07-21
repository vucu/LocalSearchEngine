using LocalSearchEngine.ViewModels.Components;

namespace LocalSearchEngine.ViewModels
{
    public class SearchResultViewModel : PageViewModelBase
    {
        private string html;

        public string Html { get => html; set => this.SetValue(ref html, value); }
    }
}
