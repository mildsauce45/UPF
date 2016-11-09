using System;
using System.Xml;

namespace FirstWave.Unity.Gui.Utilities.Parsing.Visitors
{
	public class TemplateNodeVisitor : IXamlNodeVisitor
	{		
		public void Visit(XmlNode node, ParseContext context)
		{
			context.Resources.Add(node.Attributes.GetNamedItem("Key").Value, new Template(node, context));
		}

		public Control VisitWithResult(XmlNode node, ParseContext context)
		{
			throw new NotImplementedException("This should only be called via data templating.");
		}
	}
}
