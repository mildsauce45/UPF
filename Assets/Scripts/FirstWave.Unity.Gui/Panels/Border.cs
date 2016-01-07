using FirstWave.Unity.Gui.Drawing;
using System;
using UnityEngine;

namespace FirstWave.Unity.Gui.Panels
{
    public class Border : Panel
    {
        private Vector2 borderSize;

        private BorderTextures _textures;
        public BorderTextures Textures
        {
            get { return _textures != null ? _textures : GUIManager.Instance.borderTextures; }
            set { _textures = value; }
        }

        public Border()
        {
            HorizontalAlignment = Enums.HorizontalAlignment.Stretch;
            VerticalAlignment = Enums.VerticalAlignment.Stretch;
        }

        public Border(BorderTextures textures)
        {
            Textures = textures;
        }

        public override void AddChild(Control control)
        {
            if (Children.Count == 0)
                base.AddChild(control);
            else
                throw new InvalidOperationException("A Border can only have a single child element. Consider adding a panel as the child");
        }

        public override void Layout(Rect r)
        {
            if (Location.HasValue)
                return;

            float x = GetStartingXCoordinate(r);
            float y = GetStartingYCoordinate(r);

            Location = new Vector2(x, y);

            borderSize = new Vector2(HorizontalAlignment == Enums.HorizontalAlignment.Stretch ? r.width : Size.Value.x,
                                     VerticalAlignment == Enums.VerticalAlignment.Stretch ? r.height : Size.Value.y);

            float sizeX = borderSize.x - (2 * GetTextureWidth());
            float sizeY = borderSize.y - (2 * GetTextureHeight());

            foreach (var child in Children)
                child.Layout(new Rect(x + GetTextureWidth() + Padding.Left, y + GetTextureHeight() + Padding.Top, sizeX, sizeY));
        }

        public override Vector2 Measure()
        {
            if (Size.HasValue)
                return Size.Value;

            float height = Margin.Top + Margin.Bottom + Padding.Top + Padding.Bottom + (2 * GetTextureHeight());
            float width = Margin.Left + Margin.Right + Padding.Left + Padding.Right + (2 * GetTextureWidth());

            foreach (var child in Children)
            {
                var childSize = child.Measure();

                height += childSize.y;
                width += childSize.x;
            }

            Size = new Vector2(width, height);

            return Size.Value;
        }

        public override void Draw()
        {
            if (Size == null || Location == null)
                return;

            BorderBox.Draw(new Rect(Location.Value.x, Location.Value.y, borderSize.x, borderSize.y), Textures);

            base.Draw();
        }

        private float GetTextureHeight()
        {
            return Textures != null && Textures.BorderHorizontal != null ? Textures.BorderHorizontal.height : 0;
        }

        private float GetTextureWidth()
        {
            return Textures != null && Textures.BorderVertical != null ? Textures.BorderVertical.width : 0;
        }
    }
}
