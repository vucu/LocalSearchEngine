using LocalSearchEngine.Core;
using LocalSearchEngine.Services;

namespace LocalSearchEngine
{
    public partial class App : Application
    {
        public App()
        {
            DependencyService.Register<IApplicationSettingsService, ApplicationSettingsService>();
            DependencyService.Register<IUriService, UriService>();
            DependencyService.Register<IHtmlStorageService, HtmlStorageService>();
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}
