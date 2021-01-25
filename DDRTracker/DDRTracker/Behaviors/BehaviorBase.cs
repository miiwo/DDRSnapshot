using System;
using Xamarin.Forms;

namespace DDRTracker.InterfacesBases
{
    /// <summary>
    /// A base for behaviors in Xamarin Forms for BindableObjects. All BindableObjects have a
    /// BindingContext, thus they use Events.
    /// </summary>
    /// <typeparam name="T">The object to put a behavior on. Must be a BindableObject.</typeparam>
    public abstract class BehaviorBase<T> : Behavior<T> where T : BindableObject
    {
        public T AssociatedObject {get; private set; } // BindableObject to Attach Behavior To

        /// <summary>
        /// Changes the binding context of this class to the BindableObject's, and adds the intended behavior of this class
        /// to the BindableObject.
        /// </summary>
        /// <param name="bindable">the object to attach this class on</param>
        protected override OnAttachedTo(T bindable)
        {
            base.OnAttachedTo(bindable);
            AssociatedObject = bindable;

            if (bindable.BindingContext != null)
            {
                BindingContext = bindable.BindingContext;
            }

            bindable.BindingContextChanged += OnBindingContextChanged;
        }

        /// <summary>
        /// Removes the binding context in preparation for the next BindableObject to be attached to this class.
        /// </summary>
        /// <param name="bindable">the object to dettach from</param>
        protected override void OnDetachingFrom(T bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.BindingContextChanged -= OnBindingContextChanged;
            AssociatedObject = null;
        }

        /// <summary>
        /// BindingContextChanged method to attach to events. This will fit event method signatures.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnBindingContextChanged(object sender, EventArgs e)
        {
            OnBindingContextChanged();
        }

        /// <summary>
        /// Perform the intended behavior.
        /// Consider putting this into the same method.
        /// </summary>
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            BindingContext = AssociatedObject.BindingContext;
        }
    }
}