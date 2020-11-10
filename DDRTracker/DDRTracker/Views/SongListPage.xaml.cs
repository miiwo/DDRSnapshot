using DDRTracker.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DDRTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SongListPage : ContentPage
    {
        readonly SongListViewModel _viewModel;
        public SongListPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new SongListViewModel();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}