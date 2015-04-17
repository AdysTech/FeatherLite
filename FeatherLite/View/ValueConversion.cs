using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdysTech.FeatherLite.View
{
    [AttributeUsage (AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ValueConversionAttribute : Attribute
    {
        // Fields
        private Type _parameterType;
        private Type _sourceType;
        private Type _targetType;

        // Methods
        public ValueConversionAttribute(Type sourceType, Type targetType)
        {
            if ( sourceType == null )
            {
                throw new ArgumentNullException ("sourceType");
            }
            if ( targetType == null )
            {
                throw new ArgumentNullException ("targetType");
            }
            this._sourceType = sourceType;
            this._targetType = targetType;
        }

        public override int GetHashCode()
        {
            return ( this._sourceType.GetHashCode () + this._targetType.GetHashCode () );
        }

        // Properties
        public Type ParameterType
        {
            get
            {
                return this._parameterType;
            }
            set
            {
                this._parameterType = value;
            }
        }

        public Type SourceType
        {
            get
            {
                return this._sourceType;
            }
        }

        public Type TargetType
        {
            get
            {
                return this._targetType;
            }
        }
    }

}
