using System;
using LocalSearchEngine.Core;
using LocalSearchEngine.ViewModels.Components;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace LocalSearchEngine.ViewModels
{
    public class DataViewModel : PageViewModelBase
    {
        private readonly IHtmlStorageService htmlStorageService = DependencyService.Resolve<IHtmlStorageService>();
        private string url;

        public DataViewModel()
        {
            this.DownloadCommand = new Command(Download);
        }

        public string Url 
        { 
            get => url; 
            set => this.SetValue(ref url, value); 
        }

        public Command DownloadCommand { get; }

        private void Download()
        {
            try
            {
                this.htmlStorageService.DownloadAndStoreZipFile(url);
            }
            catch (Exception e)
            {
                this.RaiseExceptionThrownEvent(e);
            }
        }
    }
}
