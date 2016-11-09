using FirstWave.Unity.Gui.Utilities.Parsing;
using System.Xml;

namespace FirstWave.Unity.Gui
{
	public sealed class Template
	{
		private readonly XmlNode xmlTemplate;
		private readonly ParseContext context;

		internal Template(XmlNode xmlTemplate, ParseContext context)
		{
			this.xmlTemplate = xmlTemplate;
			this.context = context;
		}

		internal Control GenerateItem(object item)
		{
			var control = XamlProcessor.LoadDataTemplate(xmlTemplate, context, item);
			control.DataContext = item;

			return control;
		}
	}
}
