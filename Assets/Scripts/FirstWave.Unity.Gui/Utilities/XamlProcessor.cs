using FirstWave.Unity.Core.Utilities;
using FirstWave.Unity.Gui.Data;
using FirstWave.Unity.Gui.MarkupExtensions;
using FirstWave.Unity.Gui.Panels;
using FirstWave.Unity.Gui.TypeConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using UnityEngine;

namespace FirstWave.Unity.Gui.Utilities
{
	public static class XamlProcessor
	{
		private static IDictionary<Assembly, Type[]> loadedTypes;
		private static IDictionary<Type, IList<TypeConverter>> typeConverters;
		private static IDictionary<string, Type> markupExtensions;

		internal static IDictionary<string, object> resources;

		private static readonly Type STRING_TYPE = typeof(string);

		static XamlProcessor()
		{
			var currentAssembly = Assembly.GetExecutingAssembly();

			loadedTypes = new Dictionary<Assembly, Type[]>();
			loadedTypes.Add(currentAssembly, currentAssembly.GetTypes());

			LoadTypeConverters(currentAssembly);
			LoadMarkupExtensions(currentAssembly);
		}

		#region Xaml Parsing Methods

		public static void ParseXaml(IList<Panel> panels, string view, object viewModel)
		{
			resources = new Dictionary<string, object>();

			var viewText = Resources.Load(view) as TextAsset;

			// I prefer DOM parsing here becase I don't like the fact that I can't reference things in the XAML
			// before they are created, especially when editing longer style only XAML files. I don't think I've
			// ever written a XAML doc so long that the benefits of SAX parsing would have been felt
			var doc = new XmlDocument();
			doc.LoadXml(viewText.text);

			var panelNodes = doc.FirstChild.ChildNodes.OfType<XmlNode>();

			var currentAssembly = loadedTypes.Keys.FirstOrDefault();

			// The children of the view node must be panels (there can be multiple)
			// However everything underneath can continue to be a panel or a control
			foreach (var panelXml in panelNodes)
			{
				if (panelXml.LocalName == "Resources")
					LoadResources(panelXml);
				else
				{
					var panelType = loadedTypes[currentAssembly].FirstOrDefault(pt => pt.Name == panelXml.LocalName);
					if (panelType != null)
					{
						var panel = Activator.CreateInstance(panelType) as Panel;
						panels.Add(panel);

						LoadAttributes(panel, panelXml, viewModel);

						// The top-most panel/controls are going to get their DataContexts set to the passed in view model
						if (panel.DataContext == null)
							panel.DataContext = viewModel;

						LoadPanel(panel, panelXml, viewModel);
					}
					else
						Debug.LogError("Could not locate panel class for type: " + panelXml.LocalName);
				}
			}
		}

		private static void LoadResources(XmlNode resourceNode)
		{
			foreach (var rn in resourceNode.ChildNodes.OfType<XmlNode>())
			{
				var itemKey = rn.Attributes.GetNamedItem("Key");

				// Not going to support keyless styles and templates for now
				if (itemKey == null)
					continue;

				if (rn.LocalName == "Template")
					resources.Add(itemKey.Value, new Template(rn));
			}
		}

		private static void LoadPanel(Panel panel, XmlNode panelXml, object viewModel)
		{
			var currentAssembly = loadedTypes.Keys.FirstOrDefault();

			foreach (var childNode in panelXml.ChildNodes.OfType<XmlNode>())
			{
				var childType = loadedTypes[currentAssembly].FirstOrDefault(t => t.Name == childNode.LocalName);
				if (childType != null)
				{
					var child = Activator.CreateInstance(childType);
					panel.AddChild(child as Control);

					// Load attributes first, in case this is a panel
					LoadAttributes(child as Control, childNode, viewModel);

					if (child is Panel)
						LoadPanel(child as Panel, childNode, viewModel);
				}
				else
					Debug.LogError("Could not locate class for type: " + childNode.LocalName);
			}
		}

		private static void LoadAttributes(Control control, XmlNode controlXml, object viewModel)
		{
			var ct = control.GetType();

			foreach (var attr in controlXml.Attributes.OfType<XmlAttribute>())
			{
				var pi = ct.GetProperty(attr.LocalName);
				if (pi != null)
				{
					LoadProperty(control, attr, pi, viewModel);
					continue;
				}

				var ei = ct.GetEvent(attr.LocalName);
				if (ei != null)
				{
					LoadEvent(control, attr, ei, viewModel);
					continue;
				}
			}
		}

		private static void LoadProperty(Control control, XmlAttribute attr, PropertyInfo pi, object viewModel)
		{
			object value = attr.Value;

			if (pi.PropertyType.IsEnumOrNullableEnum())
				value = Enum.Parse(pi.PropertyType.GetUnderlyingType(), attr.Value, true);

			else
			{
				if (((string)value).StartsWith("{"))
					// If we start with the curly brace, then we're going to try and load a markup extension
					value = LoadMarkupExtension((string)value, control);
				else if (pi.PropertyType != STRING_TYPE)
				{
					// String values don't need to be converted
					var tc = typeConverters[STRING_TYPE].FirstOrDefault(t => t.CanConvert(STRING_TYPE, pi.PropertyType));
					if (tc != null)
						value = tc.ConvertTo(value);
				}
				else if (pi.PropertyType != STRING_TYPE)
					// This is probably just converting between primitive types (or we're missing a type converter)
					value = Convert.ChangeType(value, pi.PropertyType);
			}

			if (value is Binding)
				control.SetBinding(control.GetDependencyProperty(attr.LocalName), value as Binding);
			else
				pi.SetValue(control, value, null);
		}

		private static void LoadEvent(Control control, XmlAttribute attr, EventInfo ei, object viewModel)
		{
			/// TODO: maybe store this in the frame class in case theres a bunch of stuff and we don't want to keep doing reflection
			var vmType = viewModel.GetType();

			// Handlers can be declared as public or private
			var handlerMethod = vmType.GetMethod(attr.Value, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (handlerMethod == null)
				return;

			var handler = Delegate.CreateDelegate(ei.EventHandlerType, viewModel, handlerMethod);

			ei.AddEventHandler(control, handler);
		}

		private static object LoadMarkupExtension(string value, Control control)
		{
			var data = value.Substring(1, value.Length - 2);

			var meTypeIndex = data.IndexOf(' ');

			var meType = data.Substring(0, meTypeIndex);

			if (!markupExtensions.ContainsKey(meType))
				return null;

			var extension = Activator.CreateInstance(markupExtensions[meType]) as MarkupExtension;

			var parms = data.Substring(meTypeIndex + 1).Split(new char[] { ',' }).Select(s => s.Trim()).ToArray();

			extension.Load(control, parms);

			return extension.GetValue();
		}

		public static Control LoadDataTemplate(XmlNode dtNode, object dataContext)
		{
			foreach (var childNode in dtNode.ChildNodes.OfType<XmlNode>())
			{
				var childType = loadedTypes[Assembly.GetExecutingAssembly()].FirstOrDefault(t => t.Name == childNode.LocalName);
				if (childType != null)
				{
					var child = Activator.CreateInstance(childType) as Control;

					if (child is Panel)
						LoadPanel(child as Panel, childNode, dataContext);

					LoadAttributes(child, childNode, dataContext);

					return child;
				}
				else
					Debug.LogError("Could not locate class for type: " + childNode.LocalName);
			}

			throw new ArgumentException("Template node was empty");
		}

		#endregion

		#region Helper Methods

		private static void LoadTypeConverters(Assembly assem)
		{
			if (typeConverters == null)
				typeConverters = new Dictionary<Type, IList<TypeConverter>>();

			var tcType = typeof(TypeConverter);

			var types = loadedTypes[assem].Where(t => tcType.IsAssignableFrom(t) && t != tcType).ToList();

			foreach (var t in types)
			{
				var tc = Activator.CreateInstance(t) as TypeConverter;

				if (!typeConverters.ContainsKey(tc.FromType))
					typeConverters.Add(tc.FromType, new List<TypeConverter>());

				typeConverters[tc.FromType].Add(tc);
			}
		}

		private static void LoadMarkupExtensions(Assembly assem)
		{
			if (markupExtensions == null)
				markupExtensions = new Dictionary<string, Type>();

			var meType = typeof(MarkupExtension);

			var types = loadedTypes[assem].Where(t => meType.IsAssignableFrom(t) && t != meType).ToList();

			foreach (var t in types)
			{
				var me = Activator.CreateInstance(t) as MarkupExtension;

				markupExtensions[me.Key] = t;
			}
		}

		#endregion
	}
}
