using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
//using Windows.UI.Xaml.Data;

namespace AdysTech.FeatherLite.ViewModel
{
    /// <summary>
    /// Implementation of <see cref="INotifyPropertyChanged"/> to simplify models.
    /// </summary>
    public abstract class BindableObject : INotifyPropertyChanged, INotifyPropertyChanging, ICleanup
    {
        private static bool? _isInDesignMode;
        public bool IsInDesignMode
        {
            get
            {
#if DEBUG
                if ( !_isInDesignMode.HasValue )
                {
                    _isInDesignMode = DesignerProperties.IsInDesignTool;
                }

                return _isInDesignMode.Value;
#endif
                return false;
            }
        }

        public bool IsInDebugMode
        {
            get
            {
#if DEBUG
                return true;
#endif
                return false;
            }
        }

        //don't fire eventsif running in background, since if app is running in background all ViewModels will be runningin background, its a static backend property.
        private static bool _isRunningInBackground = false;
        // <summary>
        /// When set stops sending Property notifications. Useful when the app has a Background working model <see cref="RunningInBackground event"/> and you need to stop UI updates
        /// </summary>
        public bool IsRunningInBackground
        {
            get { return _isRunningInBackground; }
            set { SetProperty (ref _isRunningInBackground, value); }
        }

        //Indicates if any Async Operation is under progress. Can be used to bind to a progress bar
        private bool _isAsyncInProgress = false;
        public string PropertyIsAsyncInProgress { get { return GetPropertyName (() => this.IsAsyncInProgress); } }
        public bool IsAsyncInProgress
        {
            get { return _isAsyncInProgress; }
            set { SetProperty (ref _isAsyncInProgress, value); }
        }

        //Common Status text for Async operations, Can be used to bind to a progress bar
        private string _status;
        public string Status
        {
            get { return _status; }
            set { SetProperty (ref _status, value); }
        }


        public BindableObject()
        {
            _uiSynchronizationContext = TaskScheduler.FromCurrentSynchronizationContext ();
        }

        private TaskScheduler _uiSynchronizationContext;

        public TaskScheduler UiSynchronizationContext
        {
            get
            {
                return _uiSynchronizationContext;
            }
        }

        public Task DispatchToUIAsync(Action act)
        {
            return Task.Factory.StartNew (act,
                 CancellationToken.None,
                 TaskCreationOptions.None,
                 UiSynchronizationContext);
        }


        /// <summary>
        /// Multicast event for property change notifications.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Multicast event for property changing notifications.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        /// Checks if a property already matches a desired value.  Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (  EqualityComparer<T>.Default.Equals (storage, value) ) { return false; }

            this.RaisePropertyChanging (propertyName);
            storage = value;
            this.RaisePropertyChanged (propertyName);
            return true;
        }

        /// <summary>
        /// Notifies listeners that a property value is about to Change.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        protected void RaisePropertyChanging([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanging;
            if ( eventHandler != null )
            {
                //dispatcher or access violation NOT checked as data model should only be used by non UI thread.
                eventHandler (this, new PropertyChangingEventArgs (propertyName));
            }
        }

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if ( eventHandler != null && !_isRunningInBackground )
            {
                //dispatcher or access violation NOT checked as view model should only be used by UI thread.
                eventHandler (this, new PropertyChangedEventArgs (propertyName));
            }
        }

        #region ICleanup Members
        public virtual void Cleanup()
        {
            this.PropertyChanged = null;
        }
        #endregion

        protected string GetPropertyName<T>(Expression<Func<T>> expression)
        {
            var lambda = expression as LambdaExpression;
            MemberExpression memberExpression;
            if ( lambda.Body is UnaryExpression )
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambda.Body as MemberExpression;
            }

            if ( memberExpression != null )
            {
                return memberExpression.Member.Name;
            }

            return null;
        }
    }
}
