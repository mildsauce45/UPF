using FirstWave.Unity.Gui.Enums;
using System.Linq;
using UnityEngine;

namespace FirstWave.Unity.Gui.Panels
{
    public class DockPanel : Panel
    {
        #region Private Variables

        private Control left;
        private Control right;
        private Control top;
        private Control bottom;
        private Control center;

        private Vector2 leftSize = Vector2.zero;
        private Vector2 rightSize = Vector2.zero;
        private Vector2 topSize = Vector2.zero;
        private Vector2 bottomSize = Vector2.zero;
        private Vector2 centerSize = Vector2.zero;

        #endregion

        /// <summary>
        /// Horizontal will make left and right controls stretch to the full height of the control.
        /// Vertical will make top and bottom stretch to the full width of the control.
        /// </summary>
        public Orientation Orientation { get; set; }

        public DockPanel()
        {
            Orientation = Orientation.Horizontal;
        }

        public override void AddChild(Control control)
        {
            AddChild(control, Dock.Center);
        }

        public void AddChild(Control control, Dock dock)
        {
            switch (dock)
            {
                case Dock.Left:
                    left = control;
                    break;
                case Dock.Right:
                    right = control;
                    break;
                case Dock.Top:
                    top = control;
                    break;
                case Dock.Bottom:
                    bottom = control;
                    break;
                default:
                    center = control;
                    break;
            }

            base.AddChild(control);
        }

        public override Vector2 Measure()
        {
            if (Size.HasValue)
                return Size.Value;

            if (left != null)
                leftSize = left.Measure();

            if (right != null)
                rightSize = right.Measure();

            if (top != null)
                topSize = top.Measure();

            if (bottom != null)
                bottomSize = bottom.Measure();

            if (center != null)
                centerSize = center.Measure();

            float height = 0;
            float width = 0;

            if (Orientation == Orientation.Horizontal)
            {
                height = (new[] { leftSize.y, rightSize.y, centerSize.y }).Max();
                width = leftSize.x + centerSize.x + rightSize.x;
            }
            else
            {
                height = topSize.y + centerSize.y + bottomSize.y;
                width = leftSize.x + centerSize.x + rightSize.x;
            }

            Size = new Vector2(width, height);

            return Size.Value;
        }

        public override void Layout(Rect r)
        {
            if (Location.HasValue)
                return;

            if (Orientation == Orientation.Horizontal)
                LayoutHorizontal(r);
            else
                LayoutVertical(r);

            Location = new Vector2(r.x, r.y);
        }

        private void LayoutHorizontal(Rect r)
        {
            if (left != null)
                left.Layout(new Rect(r.x, r.y, left.Size.Value.x, r.height));

            // These are X Coordinates
            float centerColumnStart = leftSize.x + r.x;
            float centerColumnEnd = r.x + r.width - rightSize.x;

            if (top != null)
                top.Layout(new Rect(centerColumnStart, r.y, centerColumnEnd - centerColumnStart, topSize.y));

            if (center != null)
                center.Layout(new Rect(centerColumnStart, topSize.y, centerColumnEnd - centerColumnStart, r.height - topSize.y - bottomSize.y));

            if (bottom != null)
                bottom.Layout(new Rect(centerColumnStart, r.y + r.height - bottomSize.y, centerColumnEnd - centerColumnStart, bottomSize.y));

            if (right != null)
                right.Layout(new Rect(centerColumnEnd, r.y, rightSize.x, r.height));
        }

        private void LayoutVertical(Rect r)
        {
            if (top != null)
                top.Layout(new Rect(r.x, r.y, r.width, topSize.y));

            // These are Y Coordinates
            float centerColumnStart = topSize.y + r.y;
            float centerColumnEnd = r.y + r.height - bottomSize.y;

            if (left != null)
                left.Layout(new Rect(r.x, centerColumnStart, leftSize.x, centerColumnEnd - centerColumnStart));

            if (center != null)
                center.Layout(new Rect(r.x + leftSize.x, centerColumnStart, r.width - leftSize.x - rightSize.x, centerColumnEnd - centerColumnStart));

            if (right != null)
                right.Layout(new Rect(r.x + r.width - rightSize.x, centerColumnStart, rightSize.x, centerColumnEnd - centerColumnStart));

            if (bottom != null)
                bottom.Layout(new Rect(r.x, centerColumnEnd, r.width, bottomSize.y));
        }
    }
}
