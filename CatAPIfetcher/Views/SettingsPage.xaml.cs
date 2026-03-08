using CatAPIfetcher.Services;

namespace CatAPIfetcher.Views
{
    public partial class SettingsPage : ContentPage
    {
        private readonly CatAPIservice _catApiService;
        private readonly DatabaseService _databaseService;
        private int _catCount = 10;
        private const string ShowLocalCatsKey = "ShowLocalCats";

        public SettingsPage(CatAPIservice catApiService, DatabaseService databaseService)
        {
            InitializeComponent();
            _catApiService = catApiService;
            _databaseService = databaseService;
            UpdateCountLabel();
            LoadSettings();
        }

        private void LoadSettings()
        {
            ShowLocalCatsSwitch.IsToggled = Preferences.Get(ShowLocalCatsKey, true);
        }

        private void OnShowLocalCatsToggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set(ShowLocalCatsKey, e.Value);

            var status = e.Value ? "enabled" : "disabled";
            Shell.Current.DisplayAlert("Setting Updated",
                $"Local cats display in 'Cats' tab is now {status}. Reload the Cats tab to see changes.",
                "OK");
        }

        private void OnDecreaseClicked(object sender, EventArgs e)
        {
            if (_catCount > 1)
            {
                _catCount--;
                UpdateCountLabel();
            }
        }

        private void OnIncreaseClicked(object sender, EventArgs e)
        {
            if (_catCount < 50)
            {
                _catCount++;
                UpdateCountLabel();
            }
        }

        private void UpdateCountLabel()
        {
            CountLabel.Text = _catCount.ToString();
        }

        private async void OnGenerateClicked(object sender, EventArgs e)
        {
            var confirm = await DisplayAlert(
                "Confirm",
                $"This will delete all API cats and generate {_catCount} new random cats. Your local cats will be preserved.",
                "Yes",
                "No");

            if (!confirm)
                return;

            var button = (Button)sender;
            button.IsEnabled = false;
            button.Text = "Generating...";

            try
            {
                var allCats = await _databaseService.GetCatsAsync();
                var apiCats = allCats.Where(c => !c.IsLocalOnly).ToList();

                foreach (var cat in apiCats)
                {
                    await _databaseService.DeleteCatAsync(cat);
                }

                var newCats = await _catApiService.GetRandomCatsAsync(_catCount);
                await _databaseService.SaveCatsAsync(newCats);

                await DisplayAlert("Success", $"Generated {newCats.Count} new cats! Your local cats were preserved.", "OK");
                await Shell.Current.GoToAsync("//CatsListPage");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to generate cats: {ex.Message}", "OK");
            }
            finally
            {
                button.IsEnabled = true;
                button.Text = "🔄 Generate New Cats";
            }
        }
    }
}
