using DDRTracker.Helpers;

using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace DDRTracker.ViewModels
{
    /// <summary>
    /// Provides a general class to use on any page that requires listing out data.
    /// </summary>
    /// <typeparam name="Model">Model for each item</typeparam>
    /// <typeparam name="Key">Key to datastore</typeparam>
    public abstract class ListViewModelBase<Model, Key> : ObservableBase
    {
        public ObservableCollection<Model> ItemList { get; }

        #region Commands
        public ICommand LoadListCommand { get; }
        public ICommand ItemTapped { get; }
        public ICommand PerformSearchCommand { get; }
        #endregion

        #region SelectedItem
        protected Model _selectedItem;
        public Model SelectedItem
        {
            get { return _selectedItem; }
            set { SetField(ref _selectedItem, value, null, OnItemSelected); }
        }
        #endregion

        #region IsBusy
        protected bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetField(ref _isBusy, value); }
        }
        #endregion

        #region Query
        protected string _query;
        public string Query
        {
            get { return _query; }
            set { SetField(ref _query, value); }
        }
        #endregion


        public ListViewModelBase()
        {
            ItemList = new ObservableCollection<Model>();

            LoadListCommand = new Command(async () => await LoadItems());
            ItemTapped = new Command<Model>(OnItemSelected);
            PerformSearchCommand = new Command<string>(PerformSearch);
        }

        /// <summary>
        /// Loads the items into a viewable list. Async method.
        /// </summary>
        /// <returns></returns>
        public abstract Task LoadItems();

        /// <summary>
        /// If an item is selected, move user into a detail page with more info about said item.
        /// </summary>
        /// <param name="item">Selected item</param>
        public abstract void OnItemSelected(Model item);

        /// <summary>
        /// Filter/search the list based on the query the user provides.
        /// <summary>
        /// <param name="query">query user provides</param>
        public abstract void PerformSearch(string query);


        /// <summary>
        /// When the page first appears, initialize some fields.
        /// </summary>
        public void OnAppearing()
        {
            IsBusy = true; // Triggers the refresh.
            SelectedItem = default; // Make sure this works
        }
    }
}
