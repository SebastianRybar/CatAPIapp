using CatAPIfetcher.Services;
using CatAPIfetcher.Model;


namespace CatAPIfetcher.Views
{
    public partial class AddCatPage : ContentPage
    {
        private readonly DatabaseService _databaseService;

        public AddCatPage(DatabaseService databaseService)
        {
            InitializeComponent();
            _databaseService = databaseService;
        }

        private void OnPreviewClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(UrlEntry.Text))
            {
                PreviewImage.Source = UrlEntry.Text;
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameEntry.Text))
            {
                await DisplayAlert("Validation Error", "Please enter a name", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(UrlEntry.Text))
            {
                await DisplayAlert("Validation Error", "Please enter an image URL", "OK");
                return;
            }

            var cat = new Cat
            {
                Id = Guid.NewGuid().ToString(),
                Name = NameEntry.Text,
                Url = UrlEntry.Text,
                Description = DescriptionEditor.Text,
                IsLocalOnly = true,
                CreatedAt = DateTime.Now,
                Width = 0,
                Height = 0
            };

            await _databaseService.SaveCatAsync(cat);
            await DisplayAlert("Success", "Cat added successfully!", "OK");
            await Shell.Current.GoToAsync("..");
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}