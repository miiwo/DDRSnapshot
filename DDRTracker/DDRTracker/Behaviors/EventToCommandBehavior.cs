using System;
using System.Reflection;
using System.Windows.Input;
using Xamarin.Forms;

namespace DDRTracker.InterfaceBases
{
    /// <summary>
    /// Behavior class that attaches commands to certain events. Adapter pattern.
    /// </summary>
    public class EventToCommandBehavior : BehaviorBase<VisualElement>
    {
        Delegate eventHandler;

        #region XAMLProperties
        public static readonly BindableProperty EventNameProperty = BindableProperty.Create("EventName", typeof(string), typeof(EventToCommandBehavior), null, propertyChanged: OnEventNameChanged);
        public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(ICommand), typeof(EventToCommandBehavior), null);
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create("CommandParameter", typeof(object), typeof(EventToCommandBehavior), null);
        public static readonly BindableProperty InputConverterProperty = BindableProperty.Create("Converter", typeof(IValueConverter), typeof(EventToCommandBehavior), null);
        #endregion

        #region Fields
        public string EventName
        {
            get { return (string)GetValue(EventNameProperty); }
            set { SetValue(EventNameProperty, value); }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public IValueConverter Converter
        {
            get { return (IValueConverter)GetValue(InputConverterProperty); }
            set { SetValue(InputConverterProperty, value); }
        }
        #endregion

        /// <summary>
        /// Register an event to a specified behavior.
        /// </summary>
        /// <param name="bindable">VisualElement</param>
        protected override void OnAttachedTo(VisualElement bindable)
        {
            base.OnAttachedTo(bindable);
            RegisterEvent(EventName);
        }

        /// <summary>
        /// Deregister an event to a specified behavior.
        /// </summary>
        /// <param name="bindable">VisualElement</param>
        protected override void OnDetachingFrom(VisualElement bindable)
        {
            DeregisterEvent(EventName);
            base.OnDetachingFrom(bindable);
        }

        /// <summary>
        /// Registers the OnEvent method to the specified event.
        /// </summary>
        /// <param name="name">The name of the event</param>
        void RegisterEvent(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            EventInfo eventInfo = AssociatedObject.GetType().GetRuntimeEvent(name);
            if (eventInfo == null)
            {
                throw new ArgumentException(string.Format("EventToCommandBehavior: Can't register the '{0}' event.", EventName));
            }

            MethodInfo methodInfo = typeof(EventToCommandBehavior).GetTypeInfo().GetDeclaredMethod("OnEvent");
            eventHandler = methodInfo.CreateDelegate(eventInfo.EventHandlerType, this);
            eventInfo.AddEventHandler(AssociatedObject, eventHandler);
        }

        /// <summary>
        /// De-registers the OnEvent method to the specified event.
        /// </summary>
        /// <param name="name">The name of the event</param>
        void DeregisterEvent(string name)
        {
            if (string.IsNullOrWhiteSpace(name) ||
                eventHandler == null)
            {
                return;
            }

            EventInfo eventInfo = AssociatedObject.GetType().GetRuntimeEvent(name);
            if (eventInfo == null)
            {
                throw new ArgumentException(string.Format("EventToCommandBehavior: Can't de-register the '{0}' event.", EventName));
            }

            eventInfo.RemoveEventHandler(AssociatedObject, eventHandler);
            eventHandler = null;
        }

        /// <summary>
        /// Callback function. Method to call on the command. This essentially holds the code to execute the associated command.
        /// </summary>
        /// <param name="sender">Object that generated the Event</param>
        /// <param name="eventArgs">Parameters to supply the command with</param>
        void OnEvent(object sender, object eventArgs)
        {
            if (Command == null)
            {
                return;
            }

            object resolvedParameter; // Possibly set this to null just in case. I think the else case settles it however. Doublecheck.
            if (CommandParameter != null)
            {
                resolvedParameter = CommandParameter;
            }
            else if (Converter != null)
            {
                resolvedParameter = Converter.Convert(eventArgs, typeof(object), null, null);
            }
            else {
                resolvedParameter = eventArgs;
            }

            ExecuteBehaviorCommand(resolvedParameter);
        }

        static void OnEventNameChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var behavior = (EventToCommandBehavior)bindable;
            if (behavior.AssociatedObject == null)
            {
                return;
            }

            string oldEventName = (string)oldValue;
            string newEventName = (string)newValue;

            behavior.DeregisterEvent(oldEventName);
            behavior.RegisterEvent(newEventName);
        }

        /// <summary>
        /// Executes the given command. For subclasses, provides flexibility in how the command is executed.
        /// </summary>
        /// <param name="resolvedParameter">Parameters to supply the command with</param>
        protected virtual void ExecuteBehaviorCommand(object resolvedParameter)
        {
            if (Command.CanExecute(resolvedParameter))
            {
                Command.Execute(resolvedParameter);
            }
        }
        
    }
}