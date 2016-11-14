using FirstWave.Unity.Gui.Panels;
using System;
using System.Xml;
using UnityEngine;

namespace FirstWave.Unity.Gui.Utilities.Parsing.Visitors
{
	public class ControlNodeVisitor : IXamlNodeVisitor
	{
		public void Visit(XmlNode node, ParseContext context)
		{
			var control = VisitWithResult(node, context);

			if (control != null)
			{
				if (context.CurrentPanel != null)
					context.CurrentPanel.AddChild(control);
				else
					context.Controls.Add(control);
			}
		}

		public Control VisitWithResult(XmlNode node, ParseContext context)
		{
			var controlType = context.GetControlType(node.LocalName);
			if (controlType != null)
			{
				var control = Activator.CreateInstance(controlType) as Control;

				XamlProcessor.LoadAttributes(control, node, context);

				if (control is Panel)
					XamlProcessor.LoadPanel(control as Panel, node, context);

				return control;
			}

			Debug.LogError("Could not locate panel class for type: " + node.LocalName);

			return null;
		}
	}
}
