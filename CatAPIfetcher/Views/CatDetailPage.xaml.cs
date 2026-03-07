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

                BreedLabel.Text = string.IsNullOrEmpty(cat.Name) ? "Unknown" : cat.Name;
                OriginLabel.Text = string.IsNullOrEmpty(cat.Origin) ? "Unknown" : cat.Origin;
                TemperamentLabel.Text = string.IsNullOrEmpty(cat.Temperament) ? "Unknown" : cat.Temperament;
                LifeSpanLabel.Text = string.IsNullOrEmpty(cat.LifeSpan) ? "Unknown" : $"{cat.LifeSpan} years";

                WidthLabel.Text = $"{cat.Width} px";
                HeightLabel.Text = $"{cat.Height} px";
                LocalOnlyLabel.Text = cat.IsLocalOnly ? "Yes" : "No";
                CreatedLabel.Text = cat.CreatedAt.ToString("dd.MM.yyyy HH:mm");
                DescriptionLabel.Text = string.IsNullOrEmpty(cat.Description)
                    ? "No description available"
                    : cat.Description;
            }
        }
    }
}