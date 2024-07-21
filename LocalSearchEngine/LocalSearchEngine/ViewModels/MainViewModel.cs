using LocalSearchEngine.Core;
using LocalSearchEngine.ExtensionMethods;
using LocalSearchEngine.ViewModels.Components;
using System.Collections.ObjectModel;

namespace LocalSearchEngine.ViewModels
{
    public class MainViewModel : PageViewModelBase
    {
        private readonly IHtmlStorageService htmlStorageService = DependencyService.Resolve<IHtmlStorageService>();

        public MainViewModel()
        {
            this.SearchCommand = new Command(Search);
            this.ClearCommand = new Command(Clear);
            this.AndCommand = new Command(And);
            this.OrCommand = new Command(Or);
            this.NotCommand = new Command(Not);
            this.Clear();

            this.SearchResultsViewModel = new SearchResultsViewModel(this.SearchResults);
        }

        public SearchResultsViewModel SearchResultsViewModel { get; }
        public ObservableCollection<SearchQueryItemViewModel> SearchQueries { get; } = new ObservableCollection<SearchQueryItemViewModel>();
        public ObservableCollection<SearchResultItemViewModel> SearchResults { get; } = new ObservableCollection<SearchResultItemViewModel>();

        public Command AndCommand { get; }
        public Command OrCommand { get; }
        public Command NotCommand { get; }
        public Command ClearCommand { get; }
        public Command SearchCommand { get; }

        private void Search()
        {
            var term = this.SearchQueries[0].Term;
            var isClosedBegin = this.SearchQueries[0].Begin == BoundingMode.ClosedBegin;
            var isClosedEnd = this.SearchQueries[0].End == BoundingMode.ClosedEnd;
            var searcher = new Searcher(term, isClosedBegin, isClosedEnd);
            for (var i=1;i<this.SearchQueries.Count;i+=2)
            {
                if (this.SearchQueries[i].Type != SearchQueryItemType.Operator)
                {
                    this.RaiseExceptionThrownEvent(new Exception($"Search query item {i} is not an operator"));
                    return;
                }

                if (this.SearchQueries[i+1].Type != SearchQueryItemType.Term)
                {
                    this.RaiseExceptionThrownEvent(new Exception($"Search query item {i+1} is not an term"));
                    return;
                }

                var op = this.SearchQueries[i].Op;
                term = this.SearchQueries[i + 1].Term;
                isClosedBegin = this.SearchQueries[i + 1].Begin == BoundingMode.ClosedBegin;
                isClosedEnd = this.SearchQueries[i + 1].End == BoundingMode.ClosedEnd;
                switch (op)
                {
                    case SearchQueryOperator.AND:
                        searcher.And(term, isClosedBegin, isClosedEnd);
                        break;
                    case SearchQueryOperator.OR:
                        searcher.Or(term, isClosedBegin, isClosedEnd);
                        break;
                    case SearchQueryOperator.NOT:
                        searcher.Not(term, isClosedBegin, isClosedEnd);
                        break;
                    default:
                        this.RaiseExceptionThrownEvent(new NotImplementedException(string.Empty));
                        return;
                }
            }

            var results = new List<SearchResultItemViewModel>();
            using (var indexReader = new IndexReader())
            {
                var searchResults = searcher.Search(indexReader);
                foreach (var result in searchResults)
                {
                    if (result.Success)
                    {
                        var searchResult = new SearchResultItemViewModel()
                        {
                            Excerpt = result.Excerpt,
                            Title = result.Title,
                            DocumentId = result.DocumentId,
                            Relevance = result.Relevance,
                            HighlightedWords = searcher.HighlightedWords
                        };

                        results.Add(searchResult);
                    }
                }
            }
            

            results.Sort((a, b) => b.Relevance - a.Relevance);
            this.SearchResults.ReplaceContentWith(results);
        }

        private void Clear()
        {
            this.SearchResults.Clear();
            this.SearchQueries.Clear();
            this.SearchQueries.Add(new SearchQueryItemViewModel { Type = SearchQueryItemType.Term });
        }

        private void And()
        {
            this.SearchQueries.Add(new SearchQueryItemViewModel { Type = SearchQueryItemType.Operator, Op = SearchQueryOperator.AND });
            this.SearchQueries.Add(new SearchQueryItemViewModel { Type = SearchQueryItemType.Term });
        }

        private void Or()
        {
            this.SearchQueries.Add(new SearchQueryItemViewModel { Type = SearchQueryItemType.Operator, Op = SearchQueryOperator.OR });
            this.SearchQueries.Add(new SearchQueryItemViewModel { Type = SearchQueryItemType.Term });
        }

        private void Not()
        {
            this.SearchQueries.Add(new SearchQueryItemViewModel { Type = SearchQueryItemType.Operator, Op = SearchQueryOperator.NOT });
            this.SearchQueries.Add(new SearchQueryItemViewModel { Type = SearchQueryItemType.Term });
        }
    }
}
