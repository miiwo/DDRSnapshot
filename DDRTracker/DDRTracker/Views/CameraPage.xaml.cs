using DDRTracker.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DDRTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CameraPage : ContentPage
    {

        public CameraPage()
        {
            InitializeComponent();
            BindingContext = new SongCameraViewModel(AmazonPhotoProcessor.Instance);
        }


    }
}