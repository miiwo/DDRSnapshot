using DDRTracker.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DDRTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SongDetailPage : ContentPage
    {

        public SongDetailPage()
        {
            InitializeComponent();
            BindingContext = new SongDetailViewModel();
        }
    }
}