using DDRTracker.Services;
using Xamarin.Forms;

namespace DDRTracker.Helpers
{
    /// <summary>
    /// Abstract class that implements what a data-reliant ViewModel should behave like. This implementation is connected to a Data Store to access and display data associated with the business.
    /// </summary>
    /// <typeparam name="Model">Model of data store</typeparam>
    /// <typeparam name="Key">Type of UID</typeparam>
    abstract class DataStoreViewModelBase<Model, Key> : ObservableBase
    {
        public IDataSource<Model, Key> DataStore => DependencyService.Get<IDataSource<Model, Key>>();

        #region IsBusy
        bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetField(ref _isBusy, value); }
        }
        #endregion

        #region Title
        string _title = string.Empty;
        public string Title
        {
            get { return _title; }
            set { SetField(ref _title, value); }
        }
        #endregion
    }
}
