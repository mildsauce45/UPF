using FirstWave.Unity.Gui;
using FirstWave.Unity.Gui.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FirstWave.Unity.Gui.Controls
{
	public class Button : Control
	{
		#region Dependency Properties

		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(Button), new PropertyMetadata(null));		

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		#endregion

		#region Events

		public event EventHandler OnClick;

		#endregion

		public Button()
		{
		}

		#region UPF Methods

		public override Vector2 Measure()
		{
			Size = new Vector2(50, 25);

			return Size.Value;
		}

		public override void Layout(Rect r)
		{
			if (Location.HasValue)
				return;

			Location = new Vector2(GetStartingXCoordinate(r), GetStartingYCoordinate(r));
		}

		public override void Draw()
		{
			if (GUI.Button(new Rect(Location ?? Vector2.zero, Size ?? Vector2.zero), Text))
			{
				if (OnClick != null)
					OnClick(this, EventArgs.Empty);
			}
		}

		#endregion
	}
}
