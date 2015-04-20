using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
//using Windows.UI.Xaml.Data;

namespace AdysTech.FeatherLite.Model
{
    /// <summary>
    /// Implementation of <see cref="INotifyPropertyChanged"/> to simplify models.
    /// </summary>
    // [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class ObservableObject : INotifyPropertyChanged, INotifyPropertyChanging
    {

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
            if ( EqualityComparer<T>.Default.Equals (storage, value)  ) { return false; } 


            this.NotifyPropertyChanging (propertyName);
            storage = value;
            this.NotifyPropertyChanged (propertyName);
            return true;
        }

        /// <summary>
        /// Notifies listeners that a property value is about to Change.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        protected void NotifyPropertyChanging([CallerMemberName] string propertyName = null)
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
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if ( eventHandler != null )
            {
                //dispatcher or access violation NOT checked as data model should only be used by non UI thread.
                eventHandler (this, new PropertyChangedEventArgs (propertyName));
            }
        }

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
