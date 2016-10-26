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
		private static IDictionary<string, Type> markupExtensions;

		internal static IDictionary<string, object> resources;
		internal static IDictionary<Type, IList<TypeConverter>> TypeConverters { get; private set; }		

		static XamlProcessor()
		{
			var currentAssembly = Assembly.GetExecutingAssembly();

			loadedTypes = new Dictionary<Assembly, Type[]>();
			loadedTypes.Add(currentAssembly, currentAssembly.GetTypes());

			LoadTypeConverters(currentAssembly);
			LoadMarkupExtensions(currentAssembly);
		}

		#region Xaml Parsing Methods

		public static IList<Control> ParseXaml(string view, object viewModel)
		{
			resources = new Dictionary<string, object>();

			var controls = new List<Control>();

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
						var control = Activator.CreateInstance(panelType) as Control;
						controls.Add(control);

						LoadAttributes(control, panelXml, viewModel);

						// The top-most panel/controls are going to get their DataContexts set to the passed in view model
						if (control.DataContext == null)
                            control.DataContext = viewModel;

                        if (control is Panel)
						    LoadPanel(control as Panel, panelXml, viewModel);
					}
					else
						Debug.LogError("Could not locate panel class for type: " + panelXml.LocalName);
				}
			}

			return controls;
		}

		private static void LoadResources(XmlNode resourceNode)
		{
			foreach (var rn in resourceNode.ChildNodes.OfType<XmlNode>())
			{
				var itemKey = rn.Attributes.GetNamedItem("Key");

				// Not going to support keyless styles and templates for now
				if (itemKey == null)
				{
					Debug.LogWarning("Version 1.0 does not support keyless resources.");
					continue;
				}

                if (rn.LocalName == "Template")
                    resources.Add(itemKey.Value, new Template(rn));
				else if (rn.LocalName == "Style")
					resources.Add(itemKey.Value, new Style(rn));
                else if (!string.IsNullOrEmpty(rn.NamespaceURI))
                {
                    var typeName = string.Format("{0}.{1}", rn.NamespaceURI, rn.LocalName);

                    // I think we get away with this for right now, because importing UPF into your project doesn't
                    // produce multiple dlls, you still just get the Assembly-CSharp.dll
                    var assembly = Assembly.GetExecutingAssembly();

                    var matchingType = loadedTypes[assembly].FirstOrDefault(t => t.FullName == typeName);
                    if (matchingType != null)
                        resources.Add(itemKey.Value, Activator.CreateInstance(matchingType));
                }
			}
		}

		private static void LoadPanel(Panel panel, XmlNode panelXml, object viewModel)
		{
			var currentAssembly = loadedTypes.Keys.FirstOrDefault();

			foreach (var childNode in panelXml.ChildNodes.OfType<XmlNode>())
			{
                Type childType = null;

                if (string.IsNullOrEmpty(childNode.NamespaceURI))
                    childType = loadedTypes[currentAssembly].FirstOrDefault(t => t.Name == childNode.LocalName);
                else
                {
                    var typeName = string.Format("{0}.{1}", childNode.NamespaceURI, childNode.LocalName);

                    // I think we get away with this for right now, because importing UPF into your project doesn't
                    // produce multiple dlls, you still just get the Assembly-CSharp.dll
                    var assembly = Assembly.GetExecutingAssembly();

                    childType = loadedTypes[assembly].FirstOrDefault(t => t.FullName == typeName);
                }

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

			if (((string)value).StartsWith("{"))
				// If we start with the curly brace, then we're going to try and load a markup extension
				value = LoadMarkupExtension((string)value, control);
            else if (pi.PropertyType.IsEnumOrNullableEnum())
                value = Enum.Parse(pi.PropertyType.GetUnderlyingType(), attr.Value, true);
            else if (pi.PropertyType != Constants.STRING_TYPE)
			{
				// String values don't need to be converted
				var tc = TypeConverters[Constants.STRING_TYPE].FirstOrDefault(t => t.CanConvert(Constants.STRING_TYPE, pi.PropertyType));
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

            string meType = data;
            if (meTypeIndex >= 0)
			    meType = data.Substring(0, meTypeIndex);

			if (!markupExtensions.ContainsKey(meType))
				return null;

			var extension = Activator.CreateInstance(markupExtensions[meType]) as MarkupExtension;

            string[] parms = null;
            if (meTypeIndex >= 0)
			    parms = data.Substring(meTypeIndex + 1).Split(new char[] { ',' }).Select(s => s.Trim()).ToArray();

			extension.Load(control, parms);

			return extension.GetValue(resources);
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
			if (TypeConverters == null)
				TypeConverters = new Dictionary<Type, IList<TypeConverter>>();

			var tcType = typeof(TypeConverter);

			var types = loadedTypes[assem].Where(t => tcType.IsAssignableFrom(t) && t != tcType).ToList();

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
