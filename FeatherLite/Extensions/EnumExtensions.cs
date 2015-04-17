using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdysTech.FeatherLite.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets an attribute on an enum field value
        /// </summary>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="enumVal">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value</returns>
        public static T GetAttributeOfType<T>(this Enum enumVal) where T : System.Attribute
        {
            var type = enumVal.GetType ();
            var memInfo = type.GetMember (enumVal.ToString ());
            var attributes = memInfo[0].GetCustomAttributes (typeof (T), false);
            return ( attributes.Length > 0 ) ? (T) attributes[0] : null;
        }

        /// <summary>
        /// Gets the description of a specific enum value.
        /// </summary>
        public static string Description(this Enum eValue)
        {
            var nAttributes = eValue.GetType ().GetField (eValue.ToString ()).GetCustomAttributes (typeof (DescriptionAttribute), false);

            if ( !nAttributes.Any () )
            {
                return eValue.ToString ().Replace ("_", " ");
            }

            return ( nAttributes.First () as DescriptionAttribute ).Description;
        }

        /// <summary>
        /// Returns an enumerable collection of all values and descriptions for an enum type.
        /// </summary>
        public static IEnumerable<KeyValuePair<Enum, string>> GetValuesDescriptions<TEnum>() where TEnum : struct, IConvertible, IComparable, IFormattable
        {
            if ( !typeof (TEnum).IsEnum )
            {
                throw new ArgumentException ("TEnum must be an Enumeration type");
            }


            return Enum.GetValues (typeof(TEnum)).Cast<Enum> ().Select ((e) => new KeyValuePair<Enum, string> (e, e.Description ())).ToList ();
        }
    }
}
