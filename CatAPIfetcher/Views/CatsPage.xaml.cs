using CatAPIfetcher.Model;
using CatAPIfetcher.Services;
using System.Collections.ObjectModel;

namespace CatAPIfetcher.Views
{
    public partial class CatsPage : ContentPage
    {
        private readonly CatAPIservice _catApiService;
        private ObservableCollection<Cat> _cats;

        public CatsPage(CatAPIservice catApiService)
        {
            InitializeComponent();
            _catApiService = catApiService;
            _cats = new ObservableCollection<Cat>();
            CatsCollectionView.ItemsSource = _cats;
        }

        private async void OnLoadCatsClicked(object sender, EventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button != null)
                {
                    button.IsEnabled = false;
                    button.Text = "Loading...";
                }

                var cats = await _catApiService.GetRandomCatsAsync(20);

                _cats.Clear();
                foreach (var cat in cats)
                {
                    _cats.Add(cat);
                }

                if (button != null)
                {
                    button.IsEnabled = true;
                    button.Text = "Load Cats";
                }

                if (!cats.Any())
                {
                    await DisplayAlert("Info", "No cats loaded. Please check your internet connection.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load cats: {ex.Message}", "OK");

                var button = sender as Button;
                if (button != null)
                {
                    button.IsEnabled = true;
                    button.Text = "Load Cats";
                }
            }
        }
    }
}