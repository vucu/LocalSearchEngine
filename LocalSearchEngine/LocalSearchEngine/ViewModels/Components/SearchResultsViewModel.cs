using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace LocalSearchEngine.ViewModels.Components
{
    public class SearchResultsViewModel : ViewModelBase
    {
        private const int ResultsPerPage = 10;
        private readonly ObservableCollection<SearchResultItemViewModel> allItems;
        private int currentPage = 1;

        public SearchResultsViewModel(ObservableCollection<SearchResultItemViewModel> allItems)
        {
            this.allItems = allItems;
            this.allItems.CollectionChanged += this.OnAllItemsChanged;

            this.PreviousCommand = new Command(Previous, CanPrevious);
            this.NextCommand = new Command(Next, CanNext);
        }

        public int Count => allItems.Count;

        public ObservableCollection<object> Items { get; } = new ObservableCollection<object>();

        public int CurrentPage
        {
            get => currentPage;
            set
            {
                this.SetValue(ref currentPage, value);

                this.Items.Clear();
                var index = value - 1;
                var start = index * ResultsPerPage;
                var end = (index + 1) * ResultsPerPage;
                if (end > this.allItems.Count)
                {
                    end = this.allItems.Count;
                }

                for (var i = start; i < end; i++)
                {
                    this.Items.Add(this.allItems[i]);
                }

                this.PreviousCommand.ChangeCanExecute();
                this.NextCommand.ChangeCanExecute();
            }
        }

        public ObservableCollection<int> Pages { get; } = new ObservableCollection<int> { 1 };
        public int MaxPage => this.Pages[this.Pages.Count - 1];
        public Command PreviousCommand { get; }
        public Command NextCommand { get; }

        public override void Remove()
        {
            this.allItems.CollectionChanged -= this.OnAllItemsChanged;
            base.Remove();
        }

        private void OnAllItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnPropertyChanged(nameof(Count));

            var pageCount = this.allItems.Count / ResultsPerPage + 1;
            this.Pages.Clear();
            for (var i = 0; i < pageCount; i++)
            {
                this.Pages.Add(i + 1);
            }

            this.OnPropertyChanged(nameof(MaxPage));
            this.CurrentPage = 1;
        }

        private bool CanPrevious()
        {
            return this.CurrentPage > this.Pages[0];
        }

        private void Previous()
        {
            this.CurrentPage--;
        }

        private bool CanNext()
        {
            return this.CurrentPage < MaxPage;
        }

        private void Next()
        {
            this.CurrentPage++;
        }
    }
}
