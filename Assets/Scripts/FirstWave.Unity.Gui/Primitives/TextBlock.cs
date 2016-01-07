using UnityEngine;
using FirstWave.Unity.Gui.Enums;

namespace FirstWave.Unity.Gui.Primitives
{
    public class TextBlock : Control
    {
        public string Text;

        public TextBlock()
        {
        }

        public TextBlock(string text)
        {
            Text = text;
        }

        public override string ToString()
        {
            return string.Format("TextBlock: {0}", Text);
        }

        public override void Draw()
        {
            if (string.IsNullOrEmpty(Text) || Visibility != Visibility.Visible)
                return;

            var labelContent = new GUIContent(Text);
            var labelStyle = GUIManager.Instance.GetMessageBoxStyle(GUIManager.Instance.fontProperties);

            var loc = Location ?? Vector2.zero;
            var s = Size ?? Vector2.zero;

            GUI.Label(new Rect(loc.x, loc.y, s.x, s.y), labelContent, labelStyle);
        }

        public override void Layout(Rect r)
        {
            if (Location.HasValue)
                return;

            Location = new Vector2(GetStartingXCoordinate(r), GetStartingYCoordinate(r));
        }

        public override Vector2 Measure()
        {
            var labelContent = new GUIContent(Text);
            var labelStyle = GUIManager.Instance.GetMessageBoxStyle(GUIManager.Instance.fontProperties);

            var actualSize = labelStyle.CalcSize(labelContent);

            // We have the text size now, but we need to take into account margins now
            actualSize = new Vector2(actualSize.x + Margin.Left + Margin.Right, actualSize.y + Margin.Top + Margin.Bottom);

            Size = actualSize;

            return actualSize;
        }
    }
}
