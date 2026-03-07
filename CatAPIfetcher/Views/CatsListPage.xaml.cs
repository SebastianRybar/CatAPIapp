using CatAPIfetcher.Services;
using CatAPIfetcher.Model;
using System.Diagnostics;

namespace CatAPIfetcher.Views
{
    public partial class CatsListPage : ContentPage
    {
        private readonly CatAPIservice _catApiService;
        private readonly DatabaseService _databaseService;
        private bool _isFirstLoad = true;

        public CatsListPage(CatAPIservice catApiService, DatabaseService databaseService)
        {
            InitializeComponent();
            _catApiService = catApiService;
            _databaseService = databaseService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            Debug.WriteLine("CatsListPage - OnAppearing");
            await LoadCatsFromDatabaseAsync();

            if (_isFirstLoad)
            {
                _isFirstLoad = false;
                var cats = await _databaseService.GetCatsAsync();
                Debug.WriteLine($"Cats in database: {cats?.Count ?? 0}");

                if (cats == null || cats.Count == 0)
                {
                    Debug.WriteLine("No cats in database, loading initial cats...");
                    await LoadInitialCatsAsync();
                }
            }
        }

        private async Task LoadInitialCatsAsync()
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            try
            {
                Debug.WriteLine("Starting to fetch 5 cats from API...");
                var cats = await _catApiService.GetRandomCatsAsync(5);
                Debug.WriteLine($"Successfully fetched {cats.Count} cats");

                await _databaseService.SaveCatsAsync(cats);
                Debug.WriteLine("Cats saved to database");

                await LoadCatsFromDatabaseAsync();
                Debug.WriteLine("UI updated with cats");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in LoadInitialCatsAsync: {ex.Message}");
                await DisplayAlert("Chyba",
                    $"Nepodařilo se načíst kočky z API.\n\n" +
                    $"Chyba: {ex.Message}\n\n" +
                    $"Zkontrolujte prosím připojení k internetu.",
                    "OK");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        }

        private async Task LoadCatsFromDatabaseAsync()
        {
            var cats = await _databaseService.GetCatsAsync();
            Debug.WriteLine($"Loading {cats?.Count ?? 0} cats from database to UI");
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

                await DisplayAlert("Hotovo", $"Načteno {cats.Count} nových koček!", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Chyba",
                    $"Nepodařilo se načíst kočky: {ex.Message}",
                    "OK");
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