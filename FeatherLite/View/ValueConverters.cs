using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections;
using System.Data;
using System.ComponentModel;
using System.Collections.Specialized;


namespace AdysTech.FeatherLite.View
{

    public class DebugBindingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException ();
        }
    }

    [ValueConversion (typeof (bool), typeof (string))]
    public class BooleanStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var values = ( parameter as string ).Split (':');
            var s = "";
            if ( value is bool )
                s = ( (bool) value ) ? values[0] : values[1];
            return s;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var values = ( parameter as string ).Split (':');
            return ( (String) value == values[0] );
        }
    }

    [ValueConversion (typeof (object), typeof (Visibility))]
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool visible = (bool) new BooleanConverter ().Convert (value, targetType, parameter, culture);
            return visible ? Visibility.Visible : Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (Visibility) value == Visibility.Visible ? true : false;
        }
    }

    [ValueConversion (typeof (object), typeof (bool))]
    public class BooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool boolean = true;
            if ( value is bool )
            {
                boolean = (bool) value;
            }
            else if ( value is int || value is short || value is long )
            {
                boolean = 0 != (int) value;
            }
            else if ( value is float || value is double || value is decimal )
            {
                boolean = 0.0 != System.Convert.ToDouble (value);
            }
            else if ( value is IEnumerable )
            {
                IEnumerable list = value as IEnumerable;
                boolean = list.GetEnumerator ().MoveNext ();
            }
            else if ( value is string )
            {
                boolean = value.ToString () != "";
            }
            else if ( value is DateTime )
            {
                boolean = System.Convert.ToDateTime (value) != DateTime.MinValue;
            }
            else if ( value == null )
            {
                boolean = false;
            }
            if ( (string) parameter == "!" )
            {
                boolean = !boolean;
            }
            return boolean;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return false;
        }
    }

    [ValueConversion (typeof (int), typeof (string))]
    public class PositionConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if ( value != null )
            {

                int number = (int) value;
                string numberText = number.ToString ();
                string positionText;

                if ( number >= 10 && numberText[numberText.Length - 2] == '1' )
                {
                    // teen numbers always end in 'th'
                    positionText = "th";
                }
                else
                {
                    switch ( numberText[numberText.Length - 1] )
                    {
                        case '1':
                            positionText = "st";
                            break;
                        case '2':
                            positionText = "nd";
                            break;
                        case '3':
                            positionText = "rd";
                            break;
                        default:
                            positionText = "th";
                            break;
                    }
                }
                return ( numberText + positionText );
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string s = value as string;
            int i = 0;
            int.TryParse (s.Substring (0, s.Length - 2), out i);
            return i;
        }
    }

    //<Summary>Converts ENum to boolean and back
    // Convert: uses parameter passed in, returns true if current value of the Enum matches parameter
    //ConvertBack: if value is true, sets the value of the ENum to parameter passed in
    //</summary>
    [ValueConversion (typeof (Enum), typeof (Boolean))]
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string parameterString = parameter as string;
            if ( parameterString == null || Enum.IsDefined (value.GetType (), value) == false )
                return DependencyProperty.UnsetValue;

            return Enum.Parse (value.GetType (), parameterString).Equals (value);

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals (true) ? Enum.Parse (targetType, parameter as String) : DependencyProperty.UnsetValue;
        }
    }

    //<Summary>Converts ENum to strings, uses Description as string, Parameter can be used to pick specific enum, current value will be used otherwise</summary>
    [ValueConversion (typeof (Enum), typeof (String))]
    public class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ( DesignerProperties.IsInDesignTool ) return "NotSet";
            string parameterString = parameter as string;
            //if ( Enum.IsDefined (value.GetType (), value) == false )
            //    return DependencyProperty.UnsetValue;

            var desc = ( value.GetType ().GetField (parameterString == null ? value.ToString () : parameterString).GetCustomAttributes (typeof (DescriptionAttribute), false).FirstOrDefault () as DescriptionAttribute );
            if ( desc != null )
                return desc.Description;
            else
                return parameterString == null ? value.ToString () : parameterString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException ();
        }
    }
}
