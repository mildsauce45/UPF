using FirstWave.Unity.Gui.Bridge;
using FirstWave.Unity.Gui.Enums;
using System;
using UnityEngine;

namespace FirstWave.Unity.Gui.Primitives
{
    public class Image : Control
    {
        public Texture Texture;

        public IResourceLoader<Texture> ResourceLoader;

        #region Constructors

        public Image(Texture texture)
        {
            Texture = texture;
        }

        public Image(IResourceLoader<Texture> resourceLoader)
        {
            ResourceLoader = resourceLoader;
        }

        public Image(string resourcePath)
        {
            ResourceLoader = new TextureResourceLoader(resourcePath);
        }

        public Image(string resourcePath, string imageName)
            : this(resourcePath)
        {
            Texture = ResourceLoader.LoadResource(imageName);
        }

        #endregion

        public void LoadImage(string imageName)
        {
            if (ResourceLoader == null)
                throw new InvalidOperationException("Resource Loader is not set for this image control. Cannot load " + imageName);

            Texture = ResourceLoader.LoadResource(imageName);
        }

        #region UPF Layout Methods

        public override Vector2 Measure()
        {
            if (Size.HasValue)
                return Size.Value;

            if (Texture == null)
                throw new InvalidOperationException("Cannot measure image without a texture object");

            float height = Margin.Top + Margin.Bottom + Texture.height;
            float width = Margin.Left + Margin.Right + Texture.width;

            Size = new Vector2(width, height);

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
            var loc = Location ?? Vector2.zero;

            if (Visibility == Visibility.Visible)
                GUI.DrawTexture(new Rect(loc.x, loc.y, Texture.width, Texture.height), Texture);    
        }

        #endregion
    }
}
