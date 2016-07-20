namespace FirstWave.Unity.Gui.Data
{
	public class Binding
	{
		private object source;
		private object target;

		public object DataContext { get; set; }
		public string Path { get; set; }
		public string ElementName { get; set; }

		public object Source
		{
			get
			{
				// In the future this will get to choose between DataContext and ElementName
				if (source == null)
					source = GetSource();

				return source;
			}
		}

		public Binding(object target)
		{
			this.target = target;
		}

		public object GetValue()
		{
			var src = Source;

			if (src == null)
				return null;

			if (string.IsNullOrEmpty(Path))
				return src;

			var localSrc = src;
			var pathParts = Path.Split(new char[] { '.' });
			for (int i = 0; i <= pathParts.Length - 1; i++)
			{
				localSrc = localSrc.GetType().GetProperty(pathParts[i]).GetValue(localSrc, null);

				// If we're at the end of the path
				if (i == pathParts.Length - 1)
					return localSrc;
			}

			// Shouldn't ever get here
			return null;
		}

		private object GetSource()
		{
			if (target is Control)
			{
				var c = target as Control;

				var dc = c.DataContext;
				while (dc == null && c != null)
				{
					c = c.Parent;
					if (c != null)
						dc = c.DataContext;
				}

				return dc;
			}

			return null;
		}
	}
}
