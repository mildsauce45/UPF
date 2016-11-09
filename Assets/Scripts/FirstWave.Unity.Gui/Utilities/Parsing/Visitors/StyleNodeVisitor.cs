using System;
using System.Xml;
using UnityEngine;

namespace FirstWave.Unity.Gui.Utilities.Parsing.Visitors
{
	public class StyleNodeVisitor : IXamlNodeVisitor
	{
		public void Visit(XmlNode node, ParseContext context)
		{
			var itemKey = node.Attributes.GetNamedItem("Key").Value;

			var tt = node.Attributes.GetNamedItem("TargetType");

			if (tt == null || string.IsNullOrEmpty(tt.Value))
			{
				Debug.LogWarning("TargetType is required for styles. Skipping entry.");
				return;
			}

			var matchingType = context.GetControlType(tt.Value);
			if (matchingType == null)
			{
				Debug.LogWarning("TargetType not found for style with key: " + itemKey);
				return;
			}

			context.Resources.Add(itemKey, new Style(node, matchingType));
		}

		public Control VisitWithResult(XmlNode node, ParseContext context)
		{
			throw new NotImplementedException("This should only be called via data templating.");
		}
	}
}
