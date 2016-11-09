using System;
using System.Xml;

namespace FirstWave.Unity.Gui.Utilities.Parsing.Visitors
{
	public class NonControlResourceNodeVisitor : IXamlNodeVisitor
	{
		public void Visit(XmlNode node, ParseContext context)
		{
			var itemKey = node.Attributes.GetNamedItem("Key").Value;

			var typeName = string.Format("{0}.{1}", node.NamespaceURI, node.LocalName);

			// I think we get away with this for right now, because importing UPF into your project doesn't
			// produce multiple dlls, you still just get the Assembly-CSharp.dll
			var matchingType = context.GetCustomControlType(typeName);
			if (matchingType != null)
				context.Resources.Add(itemKey, Activator.CreateInstance(matchingType));
		}

		public Control VisitWithResult(XmlNode node, ParseContext context)
		{
			throw new NotImplementedException("This should only be called via data templating.");
		}
	}
}
