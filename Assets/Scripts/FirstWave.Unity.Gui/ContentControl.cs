using UnityEngine;

namespace FirstWave.Unity.Gui
{
    /// <summary>
    /// Base class for anything that wants to have a single child as content
    /// </summary>
    public class ContentControl : Control
    {
        public Control Child { get; set; }

        public ContentControl()
        {
        }

        public ContentControl(Control child)
        {
            Child = child;
        }

        public void AddChild(Control child)
        {
            Child = child;
        }

        public override void Draw()
        {
            if (Child != null)
                Child.Draw();
        }

        public override void Layout(Rect r)
        {
            if (Location.HasValue)
                return;

            float x = GetStartingXCoordinate(r);
            float y = GetStartingYCoordinate(r);

            Location = new Vector2(x, y);

            if (Child == null)
                return;

            x += Padding.Left;
            y += Padding.Top;

            var size = Size ?? Vector2.zero;

            Child.Layout(new Rect(x, y, size.x - Padding.Left - Padding.Right, size.y - Padding.Top - Padding.Bottom));
        }

        public override Vector2 Measure()
        {
            if (Size.HasValue)
                return Size.Value;

            var width = Margin.Left + Margin.Right + Padding.Left + Padding.Right;
            var height = Margin.Top + Margin.Bottom + Padding.Top + Padding.Bottom;

            if (Child != null)
            {
                var childSize = Child.Measure();
                width += childSize.x;
                height += childSize.y;
            }

            Size = new Vector2(width, height);

            return Size.Value;
        }

        internal override void InvalidateLayout()
        {
            base.InvalidateLayout();

            Child.InvalidateLayout();
        }
    }
}
