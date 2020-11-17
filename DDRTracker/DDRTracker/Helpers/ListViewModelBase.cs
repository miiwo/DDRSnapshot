using DDRTracker.Helpers;

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

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
        public Command<Model> ItemTapped { get; }
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


        public ListViewModelBase()
        {
            ItemList = new ObservableCollection<Model>();
            LoadListCommand = new Command(async () => await ExecuteLoadItemsCommand());
            ItemTapped = new Command<Model>(OnItemSelected);
        }

        /// <summary>
        /// Loads the items into a viewable list. One must implement this class.
        /// </summary>
        /// <returns></returns>
        public abstract Task ExecuteLoadItemsCommand();

        /// <summary>
        /// When the page first appears, intialize some fields.
        /// </summary>
        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = default; // Make sure this works
        }

        /// <summary>
        /// If an item is selected, move user into a detail page with more info about said item.
        /// </summary>
        /// <param name="item">Selected item</param>
        public abstract void OnItemSelected(Model item);
    }
}
