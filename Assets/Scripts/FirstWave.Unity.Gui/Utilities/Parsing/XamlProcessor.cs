using FirstWave.Unity.Gui.Panels;
using FirstWave.Unity.Gui.Utilities.Parsing.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

namespace FirstWave.Unity.Gui.Utilities.Parsing
{
	public static class XamlProcessor
	{
		#region Xaml Parsing Methods

		public static IList<Control> ParseXaml(string view, object viewModel)
		{
			var context = new ParseContext(viewModel);

			var viewText = Resources.Load(view) as TextAsset;

			// I prefer DOM parsing here becase I don't like the fact that I can't reference things in the XAML
			// before they are created, especially when editing longer style only XAML files. I don't think I've
			// ever written a XAML doc so long that the benefits of SAX parsing would have been felt
			var doc = new XmlDocument();
			doc.LoadXml(viewText.text);

			var panelNodes = doc.FirstChild.ChildNodes.OfType<XmlNode>().ToList();

			// The children of the view node must be panels (there can be multiple)
			// However everything underneath can continue to be a panel or a control
			foreach (var panelXml in panelNodes)
			{
				var visitor = CreateVisitor(panelXml);
				visitor.Visit(panelXml, context);
			}

			return context.Controls;
		}

		public static IXamlNodeVisitor CreateVisitor(XmlNode node)
		{
			switch (node.LocalName)
			{
				case "Resources":
					return new ResourcesNodeVisitor();
				case "Template":
					return new TemplateNodeVisitor();
				case "Style":
					return new StyleNodeVisitor();
			}

			if (!string.IsNullOrEmpty(node.NamespaceURI) && node.ParentNode.LocalName == "Resources")
				return new NonControlResourceNodeVisitor();
			else if (!string.IsNullOrEmpty(node.NamespaceURI))
				return new CustomControlNodeVisitor();
			else
				return new ControlNodeVisitor();
		}

		internal static void LoadPanel(Panel panel, XmlNode panelXml, ParseContext context)
		{
			var oldPanel = context.CurrentPanel;

			context.CurrentPanel = panel;

			foreach (var childNode in panelXml.ChildNodes.OfType<XmlNode>())
				CreateVisitor(childNode).Visit(childNode, context);

			context.CurrentPanel = oldPanel;
		}

		internal static void LoadAttributes(Control control, XmlNode controlXml, ParseContext context)
		{
			var allAttributes = controlXml.Attributes.OfType<XmlAttribute>();

			// Style should be applied first (so local values take more precedent)
			var styleAttribute = allAttributes.FirstOrDefault(x => x.LocalName == "Style");
			if (styleAttribute != null)
				new StyleAttributeVisitor(control).Visit(styleAttribute, context);

			var otherAttributes = allAttributes.Where(x => x.LocalName != "Style");

			foreach (var attr in otherAttributes)
				new AttributeVisitor(control).Visit(attr, context);
		}

		public static Control LoadDataTemplate(XmlNode dtNode, ParseContext context, object itemViewModel)
		{
			var oldVM = context.ViewModel;

			context.ViewModel = itemViewModel;

			foreach (var childNode in dtNode.ChildNodes.OfType<XmlNode>())
			{				
				var control = CreateVisitor(childNode).VisitWithResult(childNode, context);

				context.ViewModel = oldVM;

				return control;
			}

			throw new ArgumentException("Template node was empty");						
		}

		#endregion
	}
}
