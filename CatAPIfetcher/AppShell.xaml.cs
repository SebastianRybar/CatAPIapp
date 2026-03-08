using CatAPIfetcher.Views;

namespace CatAPIfetcher
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(CatDetailPage), typeof(CatDetailPage));
            Routing.RegisterRoute(nameof(AddCatPage), typeof(AddCatPage));

            if (this.Items.Count > 0 && this.Items[0] is TabBar tabBar)
            {
                Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific.TabbedPage.SetToolbarPlacement(
                    this,
                    Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific.ToolbarPlacement.Bottom);

                var border = new BoxView
                {
                    HeightRequest = 2,
                    BackgroundColor = Color.FromArgb("#4682B4"),
                    VerticalOptions = LayoutOptions.Start
                };
            }
        }
    }
}