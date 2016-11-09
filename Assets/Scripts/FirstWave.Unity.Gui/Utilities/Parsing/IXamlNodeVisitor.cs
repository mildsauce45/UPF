using System.Xml;

namespace FirstWave.Unity.Gui.Utilities.Parsing
{
	public interface IXamlNodeVisitor
	{
		void Visit(XmlNode node, ParseContext context);

		/// <summary>
		/// Not super happy with this, but for now this is used only for data templating
		/// </summary>
		Control VisitWithResult(XmlNode node, ParseContext context);
	}
}
