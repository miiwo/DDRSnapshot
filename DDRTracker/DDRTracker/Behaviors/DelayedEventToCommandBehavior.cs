using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace DDRTracker.InterfaceBases
{

    /// <summary>
    /// Behavior subclass that delays a command from being executed.
    /// </summary>
    public class DelayedEventToCommandBehavior : EventToCommandBehavior
    {
        #region Delay(ms)
        public const int DEFAULT_DELAY_INTERVAL_MS = 500;

        public int MinimumDelayIntervalMilliseconds
        {
            get { return (int)GetValue(MinimumDelayIntervalMillisecondsProperty); }
            set { SetValue(MinimumDelayIntervalMillisecondsProperty, value); }
        }

        public static readonly BindableProperty MinimumDelayIntervalMillisecondsProperty = BindableProperty.Create(nameof(MinimumDelayIntervalMilliseconds), typeof(int), typeof(DelayedEventToCommandBehavior), DEFAULT_DELAY_INTERVAL_MS);
        #endregion

        CancellationTokenSource throttleCts = new CancellationTokenSource();

        async protected override void ExecuteBehaviorCommand(object resolvedParameter)
        {
            try
            {
                Interlocked.Exchange(ref throttleCts, new CancellationTokenSource()).Cancel();
                await Task.Delay(MinimumDelayIntervalMilliseconds, throttleCts.Token).ContinueWith(delegate { Command.Execute(resolvedParameter); },
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnRanToCompletion,
                TaskScheduler.FromCurrentSynchronizationContext());
                
            } catch (OperationCanceledException)
            {
                Debug.WriteLine("DelayedEventToCommand Error");

            }
        }
    }
    
}