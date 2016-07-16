namespace FirstWave.Unity.Core.Utilities
{
	public static class StringExtensions
	{
		public static float ToFloat(this string s)
		{
			if (string.IsNullOrEmpty(s))
				return 0;

			float res = 0;
			if (float.TryParse(s, out res))
				return res;

			return 0;
		}
	}
}
