using CatAPIfetcher.Services;
using CatAPIfetcher.Model;

namespace CatAPIfetcher.Views
{
    [QueryProperty(nameof(CatId), "CatId")]
    public partial class CatDetailPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private string _catId;

        public string CatId
        {
            get => _catId;
            set
            {
                _catId = value;
                LoadCatAsync();
            }
        }

        public CatDetailPage(DatabaseService databaseService)
        {
            InitializeComponent();
            _databaseService = databaseService;
        }

        private async void LoadCatAsync()
        {
            if (string.IsNullOrEmpty(CatId))
                return;

            var cat = await _databaseService.GetCatAsync(CatId);

            if (cat != null)
            {
                CatImage.Source = cat.Url;
                CatNameLabel.Text = cat.DisplayName;
                IdLabel.Text = cat.Id;
                WidthLabel.Text = cat.Width.ToString();
                HeightLabel.Text = cat.Height.ToString();
                LocalOnlyLabel.Text = cat.IsLocalOnly ? "Yes" : "No";
                CreatedLabel.Text = cat.CreatedAt.ToString("g");
                DescriptionLabel.Text = !string.IsNullOrEmpty(cat.Description)
                    ? cat.Description
                    : "No description available";
            }
        }
    }
}