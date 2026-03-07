namespace CatAPIfetcher
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnGetStartedClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CatsListPage");
        }
    }
}