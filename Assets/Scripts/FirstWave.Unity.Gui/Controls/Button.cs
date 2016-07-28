using FirstWave.Unity.Gui;
using FirstWave.Unity.Gui.Bridge;
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

		public static readonly DependencyProperty BackgroundProperty =
			DependencyProperty.Register("Background", typeof(string), typeof(Button), new PropertyMetadata(null, OnBackgroundChanged));

		public static readonly DependencyProperty ImageProperty =
			DependencyProperty.Register("Image", typeof(string), typeof(Button), new PropertyMetadata(null, OnImageChanged));

		private static void OnBackgroundChanged(Control c, object oldValue, object newValue)
		{
			var b = c as Button;

			if (newValue == null)
				b.background = null;
			else
				b.background = new TextureResourceLoader().LoadResource((string)newValue);
		}

		private static void OnImageChanged(Control c, object oldValue, object newValue)
		{
			var b = c as Button;

			if (newValue == null)
				b.texture = null;
			else
				b.texture = new TextureResourceLoader().LoadResource((string)newValue);
		}

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public string Background
		{
			get { return (string)GetValue(BackgroundProperty); }
			set { SetValue(BackgroundProperty, value); }
		}

		public string Image
		{
			get { return (string)GetValue(ImageProperty); }
			set { SetValue(ImageProperty, value); }
		}

		#endregion

		private Texture background;
		private Texture texture;

		private GUIContent content;
		private GUIStyle style;

		#region Events

		public event EventHandler OnClick;

		#endregion

		public Button()
		{
		}

		#region UPF Methods

		public override Vector2 Measure()
		{
			content = new GUIContent(Text, texture);

			var bgContent = new GUIContent(background);

			style = GUIManager.Instance.GetButtonStyle(background as Texture2D);

			Size = style.CalcSize(bgContent);

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
			if (GUI.Button(new Rect(Location ?? Vector2.zero, Size ?? Vector2.zero), content, style))
			{
				if (OnClick != null)
					OnClick(this, EventArgs.Empty);
			}
		}

		#endregion
	}
}
