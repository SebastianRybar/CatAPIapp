using CatAPIfetcher.Services;
using CatAPIfetcher.Model;

namespace CatAPIfetcher.Views
{
    public partial class CatsListPage : ContentPage
    {
        private readonly CatAPIservice _catApiService;
        private readonly DatabaseService _databaseService;

        public CatsListPage(CatAPIservice catApiService, DatabaseService databaseService)
        {
            InitializeComponent();
            _catApiService = catApiService;
            _databaseService = databaseService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadCatsFromDatabaseAsync();
        }

        private async Task LoadCatsFromDatabaseAsync()
        {
            var cats = await _databaseService.GetCatsAsync();
            CatsCollectionView.ItemsSource = cats;
        }

        private async void OnRefreshClicked(object sender, EventArgs e)
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            try
            {
                var cats = await _catApiService.GetRandomCatsAsync(20);
                await _databaseService.SaveCatsAsync(cats);
                await LoadCatsFromDatabaseAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load cats: {ex.Message}", "OK");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        }

        private async void OnAddCatClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(AddCatPage));
        }

        private async void OnCatSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Cat selectedCat)
            {
                await Shell.Current.GoToAsync($"{nameof(CatDetailPage)}?CatId={selectedCat.Id}");

                CatsCollectionView.SelectedItem = null;
            }
        }
    }
}