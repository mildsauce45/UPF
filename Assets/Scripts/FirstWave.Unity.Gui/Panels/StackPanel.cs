using FirstWave.Unity.Gui.Enums;
using System.Linq;
using UnityEngine;

namespace FirstWave.Unity.Gui.Panels
{
    public class StackPanel : Panel
    {
        public Orientation Orientation;

        public StackPanel()
        {
            Orientation = Orientation.Vertical;
        }

        #region Measure

        public override Vector2 Measure()
        {
            if (Size.HasValue)
                return Size.Value;

            if (Orientation == Orientation.Vertical)
                return MeasureVertical();

            return MeasureHorizontal();
        }

        private Vector2 MeasureVertical()
        {
            float height = this.Margin.Top + this.Margin.Bottom + this.Padding.Top + this.Padding.Bottom;
            float width = this.Margin.Left + this.Margin.Right + this.Padding.Left + this.Padding.Right;

            foreach (var child in Children)
            {
                height += child.Measure().y;
            }

            width += Children.Select(c => c.Size.Value.x).Max();

            var actualSize = new Vector2(width, height);

            Size = actualSize;

            return actualSize;
        }

        private Vector2 MeasureHorizontal()
        {
            float height = this.Margin.Top + this.Margin.Bottom + this.Padding.Top + this.Padding.Bottom;
            float width = this.Margin.Left + this.Margin.Right + this.Padding.Left + this.Padding.Right;

            foreach (var child in Children)
            {
                width += child.Measure().x;
            }

            height += Children.Select(c => c.Measure().y).Max();

            var actualSize = new Vector2(width, height);

            Size = actualSize;

            return actualSize;
        }

        #endregion

        #region Layout

        public override void Layout(Rect r)
        {
            if (Location.HasValue)
                return;

            float x = GetStartingXCoordinate(r);
            float y = GetStartingYCoordinate(r);

            Location = new Vector2(x, y);

            if (Orientation == Orientation.Vertical)
            {
                foreach (var child in Children)
                {
                    var childSize = child.Size ?? Vector2.zero;

                    child.Layout(new Rect(x, y, Size.Value.x, childSize.y));

                    y += childSize.y;
                }
            }
            else
            {
                foreach (var child in Children)
                {
                    var childSize = child.Size ?? Vector2.zero;

                    child.Layout(new Rect(x, y, childSize.y, Size.Value.y));

                    x += childSize.x;
                }
            }
        }

        #endregion
    }
}
