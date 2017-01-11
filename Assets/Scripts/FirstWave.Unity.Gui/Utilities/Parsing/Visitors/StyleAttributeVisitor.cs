using System.Xml;

namespace FirstWave.Unity.Gui.Utilities.Parsing.Visitors
{
	public class StyleAttributeVisitor : AttributeVisitor
	{
		public StyleAttributeVisitor(Control control)
			: base(control)
		{
		}

		protected override void DoVisit(XmlNode node, ParseContext context)
		{
			base.DoVisit(node, context);

            var control = obj as Control;

			control.Style.Initialize(control);
			control.Style.Apply(control);
		}
	}
}
