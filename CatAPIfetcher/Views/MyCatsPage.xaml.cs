using CatAPIfetcher.Services;
using CatAPIfetcher.Model;
using System.Diagnostics;

namespace CatAPIfetcher.Views
{
    public partial class MyCatsPage : ContentPage
    {
        private readonly DatabaseService _databaseService;

        public MyCatsPage(DatabaseService databaseService)
        {
            InitializeComponent();
            _databaseService = databaseService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadMyCatsAsync();
        }

        private async Task LoadMyCatsAsync()
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            try
            {
                var allCats = await _databaseService.GetCatsAsync();
                var myCats = allCats.Where(c => c.IsLocalOnly).ToList();

                Debug.WriteLine($"Loading {myCats.Count} local cats");
                MyCatsCollectionView.ItemsSource = myCats;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading my cats: {ex.Message}");
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

        private async void OnCatTapped(object sender, EventArgs e)
        {
            if (e is TappedEventArgs tappedArgs && tappedArgs.Parameter is Cat selectedCat)
            {
                await Shell.Current.GoToAsync($"{nameof(CatDetailPage)}?CatId={selectedCat.Id}");
            }
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            if (button.CommandParameter is Cat catToDelete)
            {
                var confirm = await DisplayAlert(
                    "Delete Cat",
                    $"Are you sure you want to delete '{catToDelete.DisplayName}'? This action cannot be undone.",
                    "Delete",
                    "Cancel");

                if (confirm)
                {
                    try
                    {
                        await _databaseService.DeleteCatAsync(catToDelete);
                        await DisplayAlert("Success", $"'{catToDelete.DisplayName}' has been deleted.", "OK");
                        await LoadMyCatsAsync();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error deleting cat: {ex.Message}");
                        await DisplayAlert("Error", $"Failed to delete cat: {ex.Message}", "OK");
                    }
                }
            }
        }
    }
}