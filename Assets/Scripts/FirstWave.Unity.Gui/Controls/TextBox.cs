using UnityEngine;

namespace FirstWave.Unity.Gui.Controls
{
    public class TextBox : Control
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TextBox), new PropertyMetadata(null, false));

        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register("MaxLength", typeof(int?), typeof(TextBox), new PropertyMetadata(null, false));

        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(float), typeof(TextBox), new PropertyMetadata(120f));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public int? MaxLength
        {
            get { return (int?)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        public float Width
        {
            get { return (float)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        #region UPF Methods

        public override void Draw()
        {
            var oldEnabled = GUI.enabled;

            string text;
            var style = GUIManager.Instance.GetTextBoxStyle(null);

            var rect = new Rect(Location ?? Vector2.zero, Size ?? Vector2.zero);

            if (MaxLength.HasValue)
                text = GUI.TextField(rect, Text ?? string.Empty, MaxLength.Value, style);
            else
                text = GUI.TextField(rect, Text ?? string.Empty, style);

            if (text != Text)
                FireTextChanged(Text, text);
        }

        public override void Layout(Rect r)
        {
            if (Location.HasValue)
                return;

            Location = new Vector2(GetStartingXCoordinate(r), GetStartingYCoordinate(r));
        }

        public override Vector2 Measure()
        {
            var style = GUIManager.Instance.GetTextBoxStyle(null);
            var size = style.CalcSize(new GUIContent(Text ?? string.Empty));

            Size = new Vector2(Width, size.y);

            return Size.Value;
        }

        #endregion

        private void FireTextChanged(string oldValue, string newValue)
        {
            Text = newValue;

            /// TODO: There will probably be some events here to fire
        }
    }
}
