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
    }
}
