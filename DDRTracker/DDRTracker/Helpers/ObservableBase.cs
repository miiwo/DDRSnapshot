using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DDRTracker.Helpers
{
    /// <summary>
    /// Abstract class that contains the necessary functions for an Observable Object.
    /// </summary>
    public abstract class ObservableBase : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected ObservableBase()
        {
        }

        #endregion
        #region INotifyPropertyChanged
        /// <summary>
        /// Event that is raised after a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        #region INotifyPropertyChanging
        /// <summary>
        /// Event that is raised before the property is changed.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        protected virtual void OnPropertyChanging(string propertyName)
        {
            var handler = PropertyChanging;
            if (handler != null)
            {
                handler(this, new PropertyChangingEventArgs(propertyName));
            }
        }
        #endregion

        /// <summary>
        /// Sets the value of the <paramref name="field"/> to the new value and raises the appropriate
        /// property change notifications.
        /// </summary>
        /// <typeparam name="T">The type of the field to be set.</typeparam>
        /// <param name="field">The field to set, passed by reference.</param>
        /// <param name="value">The new value to set on the field.</param>
        /// <param name="onChanging">The action to perform before changing the field using the value.</param>
        /// <param name="onChanged">The action to be performed after changing the field using the value.</param>
        /// <param name="propertyName">The name of the property that will be communicated in the property change event. If not given an argument, uses whatever called it as a string.</param>
        /// <returns>True if the field was changed, false if the new value was the same as the old value.</returns>
        protected bool SetField<T>(ref T field, T value, Action<T> onChanging=null, Action<T> onChanged=null, [CallerMemberName] string propertyName = "")
        {

            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            OnPropertyChanging(propertyName);
            onChanging?.Invoke(value);
            field = value;
            onChanged?.Invoke(value);
            OnPropertyChanged(propertyName);

            return true;

        }
    }
}
