using LocalSearchEngine.Core;
using LocalSearchEngine.ExtensionMethods;
using LocalSearchEngine.ViewModels;
using LocalSearchEngine.ViewModels.Components;
using LocalSearchEngine.Views;

namespace LocalSearchEngine.Controls
{
    public class SearchResultHyperlink : HyperlinkBase
    {
        protected override async Task OnClick()
        {
            var vm = (SearchResultItemViewModel)this.BindingContext;
            var htmlStorageService = DependencyService.Get<IHtmlStorageService>();
            var html = htmlStorageService.ReadFile(vm.DocumentId);
            foreach (var word in vm.HighlightedWords)
            {
                html = html.Replace(word,x => $"<span style=\"background-color: #FFFF00\">{x}</span>", StringComparison.OrdinalIgnoreCase);
            }

            await this.Navigation.PushAsync(new SearchResultPage
            {
                BindingContext = new SearchResultViewModel 
                { 
                    Html = html
                }
            });
        }
    }
}
