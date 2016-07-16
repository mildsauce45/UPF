using FirstWave.Unity.Core.Utilities;
using System;
using System.Linq;

namespace FirstWave.Unity.Gui.TypeConverters
{
	public class ThicknessTypeConverter : TypeConverter
	{
		public ThicknessTypeConverter()
			: base(typeof(string), typeof(Thickness))
		{
		}

		public override object ConvertTo(object value)
		{
			var parts = ((string)value).Split(new char[] { ',' }).Select(s => s.Trim().ToFloat()).ToArray();

			if (parts.Length == 1)
				return new Thickness(parts[0]);

			if (parts.Length == 2)
				return new Thickness(parts[0], parts[1]);

			if (parts.Length == 4)
				return new Thickness(parts[0], parts[1], parts[2], parts[3]);

			throw new ArgumentException(string.Format("{0} is not a valid Thickness format", value));
		}
	}
}
