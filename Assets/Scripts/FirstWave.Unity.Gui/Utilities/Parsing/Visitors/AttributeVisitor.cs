using FirstWave.Unity.Core.Utilities;
using FirstWave.Unity.Gui.Data;
using FirstWave.Unity.Gui.MarkupExtensions;
using System;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace FirstWave.Unity.Gui.Utilities.Parsing.Visitors
{
    public class AttributeVisitor : IXamlNodeVisitor
	{
        protected readonly object obj;
		protected readonly Type controlType;

		public AttributeVisitor(Object obj)
        {
            this.obj = obj;
            controlType = obj.GetType();
        }

		public void Visit(XmlNode node, ParseContext context)
		{
			DoVisit(node, context);	
		}

		public Control VisitWithResult(XmlNode node, ParseContext context)
		{
			throw new NotImplementedException("This should only be called via data templating.");
		}

		protected virtual void DoVisit(XmlNode node, ParseContext context)
		{
			var attr = node as XmlAttribute;
			var pi = controlType.GetProperty(attr.LocalName);

			if (pi != null)
			{
				LoadProperty(attr, pi, context);
				return;
			}
			
			var ei = controlType.GetEvent(attr.LocalName);
			if (ei != null)
			{
				LoadEvent(attr, ei, context);
			}			
		}

		private void LoadProperty(XmlAttribute attr, PropertyInfo pi, ParseContext context)
		{
			object value = attr.Value;

			if (((string)value).StartsWith("{"))
				// If we start with the curly brace, then we're going to try and load a markup extension
				value = LoadMarkupExtension((string)value, context);
			else if (pi.PropertyType.IsEnumOrNullableEnum())
				value = Enum.Parse(pi.PropertyType.GetUnderlyingType(), attr.Value, true);
			else if (pi.PropertyType != Constants.STRING_TYPE)
			{
				// String values don't need to be converted
				var tc = ParseContext.TypeConverters[Constants.STRING_TYPE].FirstOrDefault(t => t.CanConvert(Constants.STRING_TYPE, pi.PropertyType));
				if (tc != null)
					value = tc.ConvertTo(value);
				else
				{
					// This is probably just converting between primitive types (or we're missing a type converter)

					// We need to handle nullable types as well
					var t = pi.PropertyType;
					t = Nullable.GetUnderlyingType(t) ?? t;

					// Coalesce to set the safe value using the default of t or the safe type.
					value = value == null ? t.Default() : Convert.ChangeType(value, t);
				}
			}

            if (value is Binding && obj is Control)
            {
                var control = obj as Control;
                control.SetBinding(control.GetDependencyProperty(attr.LocalName), value as Binding);
            }
            else
                pi.SetValue(obj, value, null);
		}

		private void LoadEvent(XmlAttribute attr, EventInfo ei, ParseContext context)
		{
			/// TODO: maybe store this in the frame class in case theres a bunch of stuff and we don't want to keep doing reflection
			var vmType = context.ViewModel.GetType();

			// Handlers can be declared as public or private
			var handlerMethod = vmType.GetMethod(attr.Value, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (handlerMethod == null)
				return;

			var handler = Delegate.CreateDelegate(ei.EventHandlerType, context.ViewModel, handlerMethod);

            ei.AddEventHandler(obj, handler);
		}

		private object LoadMarkupExtension(string value, ParseContext context)
		{
			var data = value.Substring(1, value.Length - 2);

			var meTypeIndex = data.IndexOf(' ');

			string meType = data;
			if (meTypeIndex >= 0)
				meType = data.Substring(0, meTypeIndex);

			if (!ParseContext.MarkupExtensions.ContainsKey(meType))
				return null;

			var extension = Activator.CreateInstance(ParseContext.MarkupExtensions[meType]) as MarkupExtension;

			string[] parms = null;
			if (meTypeIndex >= 0)
				parms = data.Substring(meTypeIndex + 1).Split(new char[] { ',' }).Select(s => s.Trim()).ToArray();

			extension.Load(obj, parms);

			return extension.GetValue(context.Resources);
		}		
	}
}
