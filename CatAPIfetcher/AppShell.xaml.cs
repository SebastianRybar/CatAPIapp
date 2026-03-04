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
        }
    }
}