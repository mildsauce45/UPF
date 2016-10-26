using FirstWave.Unity.Core.Utilities;
using FirstWave.Unity.Gui.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

namespace FirstWave.Unity.Gui
{
	public sealed class Style
	{
		private const string MARKUP_EXT_ERROR = "Version 1.0 does not support markup extensions in Styles.";

		private readonly XmlNode xmlNode;

		internal IDictionary<DependencyProperty, object> Setters { get; private set; }

		internal Style(XmlNode xmlNode)
		{
			this.xmlNode = xmlNode;
			Setters = new Dictionary<DependencyProperty, object>();
		}

		internal void Initialize(Control control)
		{
			if (control == null)
				return;

			foreach (var setter in xmlNode.ChildNodes.OfType<XmlNode>().Where(x => x.LocalName == "Setter"))
			{
				var propertyName = setter.Attributes["Property"];
				if (propertyName != null && !string.IsNullOrEmpty(propertyName.Value))
				{
					var dp = control.GetDependencyProperty(propertyName.Value);
					if (dp == null)
						continue;

					var valueAttr = setter.Attributes["Value"];
					if (valueAttr == null || string.IsNullOrEmpty(valueAttr.Value))
						continue;

					var value = LoadValue(valueAttr.Value, dp.PropType);

					Setters.Add(dp, value);
				}
			}
		}

		private object LoadValue(string valueString, Type propertyType)
		{
			object value = valueString;

			if (((string)value).StartsWith("{"))
			{

				Debug.Log(MARKUP_EXT_ERROR);
				throw new Exception(MARKUP_EXT_ERROR);
			}
			else if (propertyType.IsEnumOrNullableEnum())
			{
				value = Enum.Parse(propertyType.GetUnderlyingType(), valueString, true);
			}
			else if (propertyType != Constants.STRING_TYPE)
			{
				// String values don't need to be converted
				var tc = XamlProcessor.TypeConverters[Constants.STRING_TYPE].FirstOrDefault(t => t.CanConvert(Constants.STRING_TYPE, propertyType));
				if (tc != null)
					value = tc.ConvertTo(value);
				else
				{
					// This is probably just converting between primitive types (or we're missing a type converter)

					// We need to handle nullable types as well
					var t = propertyType;
					t = Nullable.GetUnderlyingType(t) ?? t;

					// Coalese to set teh save value using the default of t or the safe type.
					value = value == null ? t.Default() : Convert.ChangeType(value, t);
				}
			}

			return value;
		}
	}
}
