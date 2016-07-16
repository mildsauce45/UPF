using System;

namespace FirstWave.Unity.Gui.TypeConverters
{
	public abstract class TypeConverter
	{
		public Type FromType { get; private set; }
		public Type ToType { get; private set; }

		public TypeConverter(Type fromType, Type toType)
		{
			FromType = fromType;
			ToType = toType;
		}

		public virtual bool CanConvert(Type fromType, Type toType)
		{
			return FromType == fromType && ToType == toType;
		}

		public abstract object ConvertTo(object value);		
	}
}
