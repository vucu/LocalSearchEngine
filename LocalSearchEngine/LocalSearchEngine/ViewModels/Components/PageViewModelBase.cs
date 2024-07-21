namespace LocalSearchEngine.ViewModels.Components
{
    public class PageViewModelBase : ViewModelBase
    {
        public event EventHandler<Exception> ExceptionThrown;

        public event EventHandler<Page> Navigating;

        protected void RaiseExceptionThrownEvent(Exception e)
        {
            this.ExceptionThrown?.Invoke(this, e);
        }

        protected void SuggestNavigate(Page nextPage)
        {
            this.Navigating?.Invoke(this, nextPage);
        }
    }
}
