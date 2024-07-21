using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Xaml;
using LocalSearchEngine.ViewModels;
using LocalSearchEngine.ViewModels.Components;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace LocalSearchEngine.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DataPage : ContentPage
    {
        public DataPage()
        {
            InitializeComponent();

            ((PageViewModelBase)this.BindingContext).ExceptionThrown += this.OnExceptionThrown;
        }

        private void OnExceptionThrown(object sender, Exception e)
        {
            DisplayAlert(e.GetType().ToString(), e.Message + "\r\n" + e.StackTrace, "OK") ;
        }
    }
}