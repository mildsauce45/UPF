using System;

namespace FirstWave.Unity.Core.Utilities
{
    public static class TypeExtensions
    {
        public static object Default(this Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);

            return null;
        }

		public static bool IsEnumOrNullableEnum(this Type t)
		{
			if (t.IsEnum)
				return true;

			return t.IsGenericType &&
				   t.GetGenericTypeDefinition() == typeof(Nullable<>) &&
				   t.GetGenericArguments()[0].IsEnum;
		}

		public static Type GetUnderlyingType(this Type t)
		{
			return !t.IsGenericType ? t : t.GetGenericArguments()[0];
		}
	}
}
