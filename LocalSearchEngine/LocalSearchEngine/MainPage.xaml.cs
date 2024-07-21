using LocalSearchEngine.Views;

namespace LocalSearchEngine
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnModifyDataButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DataPage());
        }
    }

}
