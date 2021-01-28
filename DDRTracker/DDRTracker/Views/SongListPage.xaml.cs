using DDRTracker.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DDRTracker.Views
{
    /// <summary>
    /// Concrete page to display the songs in.
    /// TODO: Add a Label to indicate there are no songs in the database to show. "List is empty :("
    /// </summary>
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