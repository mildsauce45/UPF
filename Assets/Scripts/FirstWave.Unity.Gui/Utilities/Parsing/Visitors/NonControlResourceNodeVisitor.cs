using System;
using System.Xml;
using System.Linq;

namespace FirstWave.Unity.Gui.Utilities.Parsing.Visitors
{
	public class NonControlResourceNodeVisitor : IXamlNodeVisitor
	{
		public void Visit(XmlNode node, ParseContext context)
		{
			var itemKey = node.Attributes.GetNamedItem("Key").Value;

            Type matchingType = null;

            if (!string.IsNullOrEmpty(node.NamespaceURI))
            {
                var typeName = string.Format("{0}.{1}", node.NamespaceURI, node.LocalName);

                // I think we get away with this for right now, because importing UPF into your project doesn't
                // produce multiple dlls, you still just get the Assembly-CSharp.dll
                matchingType = context.GetCustomControlType(typeName);
            }
            else
                matchingType = context.GetControlType(node.LocalName);


            if (matchingType != null)
            {
                var resource = Activator.CreateInstance(matchingType);

                var attributes = node.Attributes.OfType<XmlAttribute>();

                foreach (var attr in attributes)
                    new AttributeVisitor(resource).Visit(attr, context);

                context.Resources.Add(itemKey, resource);
            }
		}

		public Control VisitWithResult(XmlNode node, ParseContext context)
		{
			throw new NotImplementedException("This should only be called via data templating.");
		}
	}
}
