using FirstWave.Unity.Gui.MarkupExtensions;
using FirstWave.Unity.Gui.Panels;
using FirstWave.Unity.Gui.TypeConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FirstWave.Unity.Gui.Utilities.Parsing
{
	public class ParseContext
	{
		internal static IDictionary<Assembly, Type[]> LoadedTypes { get; private set; }
		internal static IDictionary<string, Type> MarkupExtensions { get; private set; }
		internal static IDictionary<Type, IList<TypeConverter>> TypeConverters { get; private set; }

		public IDictionary<string, object> Resources { get; private set; }
		public object ViewModel { get; set; }

		public Panel CurrentPanel { get; set; }
				
		#region Static Initialization

		static ParseContext()
		{
			var currentAssembly = Assembly.GetExecutingAssembly();

			LoadedTypes = new Dictionary<Assembly, Type[]>();
			LoadedTypes.Add(currentAssembly, currentAssembly.GetTypes());

			LoadTypeConverters(currentAssembly);
			LoadMarkupExtensions(currentAssembly);
		}

		private static void LoadTypeConverters(Assembly assem)
		{
			if (TypeConverters == null)
				TypeConverters = new Dictionary<Type, IList<TypeConverter>>();

			var tcType = typeof(TypeConverter);

			var types = LoadedTypes[assem].Where(t => tcType.IsAssignableFrom(t) && t != tcType).ToList();

			foreach (var t in types)
			{
				var tc = Activator.CreateInstance(t) as TypeConverter;

				if (!TypeConverters.ContainsKey(tc.FromType))
					TypeConverters.Add(tc.FromType, new List<TypeConverter>());

				TypeConverters[tc.FromType].Add(tc);
			}
		}

		private static void LoadMarkupExtensions(Assembly assem)
		{
			if (MarkupExtensions == null)
				MarkupExtensions = new Dictionary<string, Type>();

			var meType = typeof(MarkupExtension);

			var types = LoadedTypes[assem].Where(t => meType.IsAssignableFrom(t) && t != meType).ToList();

			foreach (var t in types)
			{
				var me = Activator.CreateInstance(t) as MarkupExtension;

				MarkupExtensions[me.Key] = t;
			}
		}

		#endregion

		public IList<Control> Controls { get; private set; }

		public ParseContext(object viewModel)
		{
			Controls = new List<Control>();
			Resources = new Dictionary<string, object>();

			ViewModel = viewModel;
		}		

		public Type GetControlType(string name)
		{
			return GetControlType(name, GetName);
		}

		public Type GetCustomControlType(string localName)
		{
			return GetControlType(localName, GetFullName);
		}

		private Type GetControlType(string typeName, Func<Type, string> getter)
		{
			var assembly = Assembly.GetExecutingAssembly();

			var matchingType = LoadedTypes[assembly].FirstOrDefault(t => getter(t) == typeName);
			if (matchingType != null)
				return matchingType;

			return null;
		}

		private static readonly Func<Type, string> GetFullName = t => t.FullName;
		private static readonly Func<Type, string> GetName = t => t.Name;
	}
}
