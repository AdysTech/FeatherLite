using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace AdysTech.FeatherLite.View
{
    [System.Windows.Markup.ContentProperty ("Converters")]
    public class ValueConverterChain : IValueConverter
    {
        // Fields
        private readonly ObservableCollection<IValueConverter> _converters =
            new ObservableCollection<IValueConverter> ();

        public ObservableCollection<IValueConverter> Converters
        {
            get { return _converters; }
        }


        private readonly Dictionary<IValueConverter, ValueConversionAttribute>
        cachedAttributes = new Dictionary<IValueConverter, ValueConversionAttribute> ();

        // Constructor
        public ValueConverterChain()
        {
            this._converters.CollectionChanged +=
             this.OnConvertersCollectionChanged;
        }

        private void OnConvertersCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            IList convertersToProcess = null;

            if ( e.Action == NotifyCollectionChangedAction.Add ||
                e.Action == NotifyCollectionChangedAction.Replace )
            {
                convertersToProcess = e.NewItems;
            }
            else if ( e.Action == NotifyCollectionChangedAction.Remove )
            {
                foreach ( IValueConverter converter in e.OldItems )
                    this.cachedAttributes.Remove (converter);
            }
            else if ( e.Action == NotifyCollectionChangedAction.Reset )
            {
                this.cachedAttributes.Clear ();
                convertersToProcess = this._converters;
            }
            if ( convertersToProcess != null && convertersToProcess.Count > 0 )
            {
                foreach ( IValueConverter converter in convertersToProcess )
                {
                    object[] attributes = converter.GetType ().GetCustomAttributes (
                     typeof (ValueConversionAttribute), false);

                    if ( attributes.Length != 1 )
                        throw new InvalidOperationException ("ValueConversionAttribute not specified for a Converter");

                    this.cachedAttributes.Add (
                     converter, attributes[0] as ValueConversionAttribute);
                }
            }
        }

        protected virtual Type GetTargetType(int converterIndex, Type finalTargetType, bool convert)
        {
            // If the current converter is not the last/first in the list, 
            // get a reference to the next/previous converter.
            IValueConverter nextConverter = null;
            if ( convert )
            {
                if ( converterIndex < this.Converters.Count - 1 )
                    nextConverter = this.Converters[converterIndex + 1];
            }
            else
            {
                if ( converterIndex > 0 )
                    nextConverter = this.Converters[converterIndex - 1];
            }

            if ( nextConverter != null )
            {
                ValueConversionAttribute attr = cachedAttributes[nextConverter];

                // If the Convert method is going to be called, 
                // we need to use the SourceType of the next 
                // converter in the list.  If ConvertBack is called, use the TargetType.
                return convert ? attr.SourceType : attr.TargetType;
            }
            // If the current converter is the last one to be executed return the target 
            // type passed into the conversion method.
            return finalTargetType;
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object output = value;
            for ( int i = 0; i < this.Converters.Count; ++i )
            {
                IValueConverter converter = this.Converters[i];
                Type currentTargetType = this.GetTargetType (i, targetType, true);
                output = converter.Convert (output, currentTargetType, parameter, culture);

                // If the converter returns 'DoNothing' 
                // then the binding operation should terminate.
                if ( output == DependencyProperty.UnsetValue )
                    break;
            }
            return output;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object output = value;
            for ( int i = 0; i < this.Converters.Count; ++i )
            {
                IValueConverter converter = this.Converters[i];
                Type currentTargetType = this.GetTargetType (i, targetType, false);
                output = converter.ConvertBack (output, currentTargetType, parameter, culture);

                // If the converter returns 'DoNothing' 
                // then the binding operation should terminate.
                if ( output == DependencyProperty.UnsetValue )
                    break;
            }
            return output;
        }

        #endregion
    }
}
