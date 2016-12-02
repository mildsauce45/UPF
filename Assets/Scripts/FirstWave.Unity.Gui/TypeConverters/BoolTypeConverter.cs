namespace FirstWave.Unity.Gui.TypeConverters
{
	public class BoolTypeConverter : TypeConverter
	{
		public BoolTypeConverter()
			: base(typeof(string), typeof(bool))
		{
		}

		public override object ConvertTo(object value)
		{
			if (value == null)
				return false;

			bool result = false;

			bool.TryParse(value.ToString(), out result);

			return result;
		}
	}
}
