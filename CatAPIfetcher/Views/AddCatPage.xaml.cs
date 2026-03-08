using CatAPIfetcher.Services;
using CatAPIfetcher.Model;
using System.Diagnostics;

namespace CatAPIfetcher.Views
{
    public partial class AddCatPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private string _selectedImagePath;
        private int _imageWidth;
        private int _imageHeight;

        public AddCatPage(DatabaseService databaseService)
        {
            InitializeComponent();
            _databaseService = databaseService;
        }

        private async void OnPickImageClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Pick a cat image",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    var localPath = await SaveImageToLocalStorage(result);

                    if (!string.IsNullOrEmpty(localPath))
                    {
                        _selectedImagePath = localPath;

                        await GetImageDimensions(localPath);

                        if (PreviewImage != null)
                        {
                            PreviewImage.Source = ImageSource.FromFile(localPath);
                        }

                        if (ImageFrame != null)
                        {
                            ImageFrame.IsVisible = true;
                        }

                        if (ImagePathLabel != null)
                        {
                            ImagePathLabel.Text = $"📁 {result.FileName}";
                        }

                        if (ImageDimensionsLabel != null)
                        {
                            ImageDimensionsLabel.Text = $"📐 Dimensions: {_imageWidth} × {_imageHeight} px";
                            ImageDimensionsLabel.IsVisible = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to pick image: {ex.Message}", "OK");
            }
        }

        private async Task GetImageDimensions(string imagePath)
        {
            try
            {
                using var stream = File.OpenRead(imagePath);
                var imageSource = ImageSource.FromStream(() => File.OpenRead(imagePath));

                if (imageSource is StreamImageSource streamImageSource)
                {
                    using var imageStream = await streamImageSource.Stream(CancellationToken.None);

#if ANDROID
                    var bitmap = await Android.Graphics.BitmapFactory.DecodeStreamAsync(imageStream);
                    _imageWidth = bitmap.Width;
                    _imageHeight = bitmap.Height;
                    bitmap.Dispose();
#elif IOS || MACCATALYST
                    var image = UIKit.UIImage.LoadFromData(Foundation.NSData.FromStream(imageStream));
                    _imageWidth = (int)image.Size.Width;
                    _imageHeight = (int)image.Size.Height;
                    image.Dispose();
#elif WINDOWS
                    using var memStream = new MemoryStream();
                    await imageStream.CopyToAsync(memStream);
                    memStream.Position = 0;
                    
                    var decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(memStream.AsRandomAccessStream());
                    _imageWidth = (int)decoder.PixelWidth;
                    _imageHeight = (int)decoder.PixelHeight;
#else
                    using var fileStream = File.OpenRead(imagePath);
                    var info = GetImageInfo(fileStream);
                    _imageWidth = info.Width;
                    _imageHeight = info.Height;
#endif
                }
                else
                {
                    using var fileStream = File.OpenRead(imagePath);
                    var info = GetImageInfo(fileStream);
                    _imageWidth = info.Width;
                    _imageHeight = info.Height;
                }

                Debug.WriteLine($"Image dimensions: {_imageWidth}x{_imageHeight}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting image dimensions: {ex.Message}");
                _imageWidth = 0;
                _imageHeight = 0;
            }
        }

        private (int Width, int Height) GetImageInfo(Stream stream)
        {
            try
            {
                byte[] buffer = new byte[24];
                stream.Read(buffer, 0, 24);
                stream.Position = 0;

                if (buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47)
                {
                    int width = (buffer[16] << 24) | (buffer[17] << 16) | (buffer[18] << 8) | buffer[19];
                    int height = (buffer[20] << 24) | (buffer[21] << 16) | (buffer[22] << 8) | buffer[23];
                    return (width, height);
                }
                else if (buffer[0] == 0xFF && buffer[1] == 0xD8)
                {
                    return ReadJpegDimensions(stream);
                }

                return (0, 0);
            }
            catch
            {
                return (0, 0);
            }
        }

        private (int Width, int Height) ReadJpegDimensions(Stream stream)
        {
            try
            {
                stream.Position = 2; 

                while (true)
                {
                    byte[] marker = new byte[2];
                    if (stream.Read(marker, 0, 2) != 2) break;

                    if (marker[0] != 0xFF) break;

                    if ((marker[1] >= 0xC0 && marker[1] <= 0xC3) ||
                        (marker[1] >= 0xC5 && marker[1] <= 0xC7) ||
                        (marker[1] >= 0xC9 && marker[1] <= 0xCB) ||
                        (marker[1] >= 0xCD && marker[1] <= 0xCF))
                    {
                        byte[] length = new byte[2];
                        stream.Read(length, 0, 2);

                        stream.ReadByte(); 

                        byte[] height = new byte[2];
                        byte[] width = new byte[2];
                        stream.Read(height, 0, 2);
                        stream.Read(width, 0, 2);

                        return ((width[0] << 8) | width[1], (height[0] << 8) | height[1]);
                    }
                    else
                    {
                        byte[] length = new byte[2];
                        if (stream.Read(length, 0, 2) != 2) break;
                        int segmentLength = (length[0] << 8) | length[1];
                        stream.Position += segmentLength - 2;
                    }
                }

                return (0, 0);
            }
            catch
            {
                return (0, 0);
            }
        }

        private async Task<string> SaveImageToLocalStorage(FileResult file)
        {
            try
            {
                var imageFolder = Path.Combine(FileSystem.AppDataDirectory, "CatImages");
                Directory.CreateDirectory(imageFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var localPath = Path.Combine(imageFolder, fileName);

                using (var stream = await file.OpenReadAsync())
                using (var fileStream = File.Create(localPath))
                {
                    await stream.CopyToAsync(fileStream);
                }

                return localPath;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to save image: {ex.Message}", "OK");
                return null;
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameEntry.Text))
            {
                await DisplayAlert("❌ Validation Error", "Please enter a name for your cat", "OK");
                return;
            }

            if (string.IsNullOrEmpty(_selectedImagePath))
            {
                await DisplayAlert("❌ Validation Error", "Please select an image for your cat", "OK");
                return;
            }

            var cat = new Cat
            {
                Id = Guid.NewGuid().ToString(),
                Name = NameEntry.Text.Trim(),
                Url = _selectedImagePath,
                Description = string.IsNullOrWhiteSpace(DescriptionEditor.Text)
                    ? "No description provided"
                    : DescriptionEditor.Text.Trim(),
                IsLocalOnly = true,
                CreatedAt = DateTime.Now,
                Width = _imageWidth,
                Height = _imageHeight,
                Origin = string.IsNullOrWhiteSpace(OriginEntry.Text)
                    ? "Local"
                    : OriginEntry.Text.Trim(),
                Temperament = string.IsNullOrWhiteSpace(TemperamentEntry.Text)
                    ? "Unknown"
                    : TemperamentEntry.Text.Trim(),
                LifeSpan = string.IsNullOrWhiteSpace(LifeSpanEntry.Text)
                    ? "Unknown"
                    : LifeSpanEntry.Text.Trim()
            };

            try
            {
                await _databaseService.SaveCatAsync(cat);
                await DisplayAlert("✅ Success", $"'{cat.Name}' has been added successfully!", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving cat: {ex.Message}");
                await DisplayAlert("❌ Error", $"Failed to save cat: {ex.Message}", "OK");
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            var confirm = await DisplayAlert(
                "Cancel",
                "Are you sure you want to cancel? All entered data will be lost.",
                "Yes",
                "No");

            if (confirm)
            {
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}