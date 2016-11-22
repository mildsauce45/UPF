using FirstWave.Unity.Gui.Bridge;
using System;
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

		public static readonly DependencyProperty HoverBackgroundProperty =
			DependencyProperty.Register("HoverBackground", typeof(string), typeof(Button), new PropertyMetadata(null, OnHoverBackgroundChanged));

		public static readonly DependencyProperty PressedBackgroundProperty =
			DependencyProperty.Register("PressedBackground", typeof(string), typeof(Button), new PropertyMetadata(null, OnPressedBackgroundChanged));

		public static readonly DependencyProperty ImageProperty =
			DependencyProperty.Register("Image", typeof(string), typeof(Button), new PropertyMetadata(null, OnImageChanged));

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(Button), new PropertyMetadata(null, false));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(Button), new PropertyMetadata(null, false));

		private static void OnBackgroundChanged(Control c, object oldValue, object newValue)
		{
			var b = c as Button;

			if (newValue == null)
				b.background = null;
			else
				b.background = new TextureResourceLoader().LoadResource((string)newValue);
		}

		private static void OnHoverBackgroundChanged(Control c, object oldValue, object newValue)
		{
			var b = c as Button;

			if (newValue == null)
				b.hoverBackground = null;
			else
				b.hoverBackground = new TextureResourceLoader().LoadResource((string)newValue);
		}

		private static void OnPressedBackgroundChanged(Control c, object oldValue, object newValue)
		{
			var b = c as Button;

			if (newValue == null)
				b.pressedBackground = null;
			else
				b.pressedBackground = new TextureResourceLoader().LoadResource((string)newValue);
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

		public string HoverBackground
		{
			get { return (string)GetValue(HoverBackgroundProperty); }
			set { SetValue(HoverBackgroundProperty, value); }
		}

		public string Image
		{
			get { return (string)GetValue(ImageProperty); }
			set { SetValue(ImageProperty, value); }
		}

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        #endregion

        #region Private Variables

        private Texture background;
		private Texture hoverBackground;
		private Texture pressedBackground;
		private Texture texture;

		private GUIContent content;
		private GUIStyle style;

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
            // One last ditch attempt at loading the texture (in case of a binding)
            if (!string.IsNullOrEmpty(Image))
                texture = new TextureResourceLoader().LoadResource(Image);

            content = new GUIContent(Text, texture);

			var bgContent = new GUIContent(background);

			style = GUIManager.Instance.GetButtonStyle(ConvertToStyle());

			Size = style.CalcSize(background != null ? bgContent : content);

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

                var command = Command;
                if (command != null && command.CanExecute(CommandParameter))
                    command.Execute(CommandParameter);
			}
		}

		#endregion

		private ButtonStyle ConvertToStyle()
		{
			return new ButtonStyle
			{
				Background = background as Texture2D,
				HoverBackground = hoverBackground as Texture2D,
				PressedBackground = pressedBackground as Texture2D
			};
		}
	}
}
