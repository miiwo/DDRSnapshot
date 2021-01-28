using Xamarin.Forms;

namespace DDRTracker.Views
{
    /// <summary>
    /// Write Android/iOS specific implementation to have a renderable view of one.
    /// Hook this up to a DependencyService once finished implementing.
    /// </summary>
    public class CameraPreview : View
    {
        public static readonly BindableProperty CameraProperty = BindableProperty.Create(
        propertyName: "Camera",
        returnType: typeof(CameraOptions),
        declaringType: typeof(CameraPreview),
        defaultValue: CameraOptions.Rear);
    
        public CameraOptions Camera
        {
            get {return (CameraOptions)GetValue(CameraProperty); }
            set { SetValue(CameraProperty, value); }
        }
    }
    
}