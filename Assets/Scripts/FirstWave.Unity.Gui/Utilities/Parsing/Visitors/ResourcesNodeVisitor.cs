using System;
using System.Linq;
using System.Xml;
using UnityEngine;

namespace FirstWave.Unity.Gui.Utilities.Parsing.Visitors
{
	public class ResourcesNodeVisitor : IXamlNodeVisitor
	{
		public void Visit(XmlNode node, ParseContext context)
		{
			foreach (var rn in node.ChildNodes.OfType<XmlNode>())
			{
				var itemKey = rn.Attributes.GetNamedItem("Key");

				// Not going to support keyless styles and templates for now
				if (itemKey == null)
				{
					Debug.LogWarning("Version 1.0 does not support keyless resources.");
					continue;
				}

				var visitor = XamlProcessor.CreateVisitor(rn);
				visitor.Visit(rn, context);
			}
		}

		public Control VisitWithResult(XmlNode node, ParseContext context)
		{
			throw new NotImplementedException("This should only be called via data templating.");
		}
	}
}
