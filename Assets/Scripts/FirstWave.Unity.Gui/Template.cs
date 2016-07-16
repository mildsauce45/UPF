using FirstWave.Unity.Gui.Utilities;
using System.Xml;

namespace FirstWave.Unity.Gui
{
    public sealed class Template
    {
        private XmlNode XmlTemplate { get; set; }

        internal Template(XmlNode xmlTemplate)
        {
            XmlTemplate = xmlTemplate;
        }

        internal Control GenerateItem(object item)
        {
            var control = XamlProcessor.LoadDataTemplate(XmlTemplate, item);
            control.DataContext = item;

            return control;
        }
    }
}
