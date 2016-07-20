using FirstWave.Unity.Gui.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FirstWave.Unity.Gui.MarkupExtensions
{
	public class BindingMarkupExtension : MarkupExtension
	{
		private Control target;

		public override string Key { get { return "Binding"; } }

		public string Path { get; private set; }		

		// Unused right now, but I see an eventual need for it
		public string ElementName { get; private set; }

		public override void Load(Control c, string[] parms)
		{
			if (parms.Length == 0)
				return;

			this.target = c;

			foreach (var parm in parms)
			{
				var parts = parm.Split(new char[] { '=' });

				if (parts.Length == 1)
					Path = parts[0];
				else if (parts.Length == 2 && parts[0] == "Path")
					Path = parts[1];
			}
		}

		public override object GetValue()
		{
			return new Binding(target) { Path = this.Path };
		}
	}
}
