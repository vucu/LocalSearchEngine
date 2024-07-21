using Prism.Commands;
using LocalSearchEngine.Core;

namespace LocalSearchEngine.ViewModels.Components
{
    public class SearchQueryItemViewModel : ViewModelBase
    {
        private SearchQueryItemType type;
        private string term = string.Empty;
        private SearchQueryOperator op;
        private BoundingMode begin = BoundingMode.ClosedBegin;
        private BoundingMode end = BoundingMode.ClosedEnd;

        public SearchQueryItemViewModel()
        {
            this.ToggleBeginCommand = new DelegateCommand(this.ToggleBegin);
            this.ToggleEndCommand = new DelegateCommand(this.ToggleEnd);
        }

        public SearchQueryItemType Type { get => type; set => this.SetValue(ref type, value); }
        public string Term { get => term; set => this.SetValue(ref term, value); }
        public BoundingMode Begin { get => begin; private set => this.SetValue(ref begin, value); }
        public BoundingMode End { get => end; private set => this.SetValue(ref end, value); }
        public SearchQueryOperator Op { get => op; set => this.SetValue(ref op, value); }

        public DelegateCommand ToggleBeginCommand { get; }

        public DelegateCommand ToggleEndCommand { get; }

        private void ToggleBegin()
        {
            if (Begin == BoundingMode.Open)
            {
                Begin = BoundingMode.ClosedBegin;
            }
            else
            {
                Begin = BoundingMode.Open;
            }
        }

        private void ToggleEnd()
        {
            if (End == BoundingMode.Open)
            {
                End = BoundingMode.ClosedEnd;
            }
            else
            {
                End = BoundingMode.Open;
            }
        }
    }
}
