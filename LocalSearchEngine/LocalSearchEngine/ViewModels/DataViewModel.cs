using System;
using LocalSearchEngine.Core;
using LocalSearchEngine.ViewModels.Components;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace LocalSearchEngine.ViewModels
{
    public class DataViewModel : PageViewModelBase, IProgress<DownloadProgress>
    {
        private readonly IHtmlStorageService htmlStorageService = DependencyService.Resolve<IHtmlStorageService>();
        private string url = string.Empty;
        private string progressText = string.Empty;

        public DataViewModel()
        {
            this.DownloadCommand = new Command(Download);
        }

        public string Url 
        { 
            get => url; 
            set => this.SetValue(ref url, value); 
        }

        public string ProgressText { get => progressText; set => SetValue(ref progressText, value); }

        public Command DownloadCommand { get; }

        public void Report(DownloadProgress value)
        {
            throw new NotImplementedException();
        }

        private void Download()
        {
            try
            {
                this.htmlStorageService.DownloadAndStoreZipFile(url, this);
            }
            catch (Exception e)
            {
                this.RaiseExceptionThrownEvent(e);
            }
        }
    }
}
