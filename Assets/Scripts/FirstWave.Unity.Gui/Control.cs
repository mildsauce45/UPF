using FirstWave.Unity.Gui.Enums;
using UnityEngine;

namespace FirstWave.Unity.Gui
{
    // Base class for all controls, contains the common properties such as location, size, alignments, etc
    public abstract class Control
    {
        public HorizontalAlignment? HorizontalAlignment { get; set; }
        public VerticalAlignment? VerticalAlignment { get; set; }

        public Vector2? Location { get; set; }

        /// <summary>
        /// If set, Measure() is not called
        /// </summary>
        public Vector2? Size;

        private Thickness _padding;
        public Thickness Padding
        {
            get { return _padding ?? Thickness.ZERO; }
            set { _padding = value; }
        }

        private Thickness _margin;
        public Thickness Margin
        {
            get { return _margin ?? Thickness.ZERO; }
            set { _margin = value; }
        }

        public Visibility Visibility { get; set; }

        public abstract Vector2 Measure();
        public abstract void Layout(Rect r);
        public abstract void Draw();

        protected float GetStartingXCoordinate(Rect r)
        {
            float x = r.x;

            if (!HorizontalAlignment.HasValue || HorizontalAlignment == Enums.HorizontalAlignment.Left || HorizontalAlignment == Enums.HorizontalAlignment.Stretch)
            {
                x = r.x + Margin.Left;
            }
            else if (HorizontalAlignment == Enums.HorizontalAlignment.Right)
            {
                x = r.x + r.width - Margin.Right - (Size.HasValue ? Size.Value.x : 0);
            }
            else if (HorizontalAlignment == Enums.HorizontalAlignment.Center)
            {
                x = r.x + r.width / 2 - (Size.HasValue ? Size.Value.x : 0) / 2;
            }

            return x;
        }

        protected float GetStartingYCoordinate(Rect r)
        {
            float y = r.y;

            if (!VerticalAlignment.HasValue || VerticalAlignment == Enums.VerticalAlignment.Top || VerticalAlignment == Enums.VerticalAlignment.Stretch)
            {
                y = r.y + Margin.Top;
            }
            else if (VerticalAlignment == Enums.VerticalAlignment.Bottom)
            {
                y = r.y + r.height - (Size.HasValue ? Size.Value.y : 0) - Margin.Bottom;
            }
            else if (VerticalAlignment == Enums.VerticalAlignment.Center)
            {
                y = r.y + r.height / 2 - (Size.HasValue ? Size.Value.y : 0) / 2;
            }

            return y;
        }

        protected virtual void OnKeyDown(string key)
        {
        }

        protected virtual void OnKeyPressed(string key)
        {
        }

        protected virtual void OnKeyReleased(string key)
        {
        }
    }
}
