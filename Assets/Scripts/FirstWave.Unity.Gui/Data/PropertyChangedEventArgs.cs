using System;

namespace FirstWave.Unity.Gui.Data
{
	public class PropertyChangedEventArgs : EventArgs
	{
		public string PropertyName { get; private set; }

		public PropertyChangedEventArgs(string propertyName)
		{
			PropertyName = propertyName;
		}
	}
}
