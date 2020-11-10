using DDRTracker.Views;
using Xamarin.Forms;

namespace DDRTracker
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(SongDetailPage), typeof(SongDetailPage));
        }
    }
}