using FirstWave.Unity.Gui.Bridge;
using FirstWave.Unity.Gui.Enums;
using System;
using UnityEngine;

namespace FirstWave.Unity.Gui.Primitives
{
    public class Image : Control
    {
        #region Dependency Properties

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(Image), new PropertyMetadata(null, OnSourceChanged));

        public static readonly DependencyProperty ResourceLoaderProperty =
            DependencyProperty.Register("ResourceLoader", typeof(IResourceLoader<Texture>), typeof(Image), new PropertyMetadata(new TextureResourceLoader(), OnResourceLoaderChanged));

        // Height and Width here fairly specialized. My use case is primarily for stretching a simple texture to fill an area.
        // Therefore, I'm going to ignore them unless the Fill property is set to Stretch. This may change in the future.
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(float?), typeof(Image));

        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(float?), typeof(Image));

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(ImageFill), typeof(Image), new PropertyMetadata(ImageFill.NoFill));

        private static void OnSourceChanged(Control source, object oldValue, object newValue)
        {
            var image = (Image)source;
            var newSource = (string)newValue;

            if (newSource == null)
                image.Texture = null;
            else if (image.ResourceLoader != null)
                image.Texture = image.ResourceLoader.LoadResource(newSource);
        }

        private static void OnResourceLoaderChanged(Control source, object oldValue, object newValue)
        {
            var image = (Image)source;
            var loader = (IResourceLoader<Texture>)oldValue;

            if (loader != null && image.Source != null)
                image.Texture = loader.LoadResource(image.Source);
        }

        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public IResourceLoader<Texture> ResourceLoader
        {
            get { return (IResourceLoader<Texture>)GetValue(ResourceLoaderProperty); }
            set { SetValue(ResourceLoaderProperty, value); }
        }

        public float? Width
        {
            get { return (float?)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        public float? Height
        {
            get { return (float?)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        public ImageFill Fill
        {
            get { return (ImageFill)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        #endregion

        private Texture Texture;

        #region Constructors

        public Image()
        {
        }

        public Image(Texture texture)
        {
            Texture = texture;
        }

        public Image(IResourceLoader<Texture> resourceLoader)
        {
            ResourceLoader = resourceLoader;
        }

        public Image(string source)
            : this(new TextureResourceLoader())
        {
            Source = source;
        }

        #endregion

        #region UPF Layout Methods

        public override Vector2 Measure()
        {
            if (Size.HasValue)
                return Size.Value;

			// One last ditch attempt at loading the texture (in case of a binding)
			if (ResourceLoader != null)
				Texture = ResourceLoader.LoadResource(Source);

            if (Texture == null)
                throw new InvalidOperationException("Cannot measure image without a texture object");

            if (UseStretch())
                Size = new Vector2(Width.Value, Height.Value);
            else
            {
                float height = Margin.Top + Margin.Bottom + Texture.height;
                float width = Margin.Left + Margin.Right + Texture.width;

                Size = new Vector2(width, height);
            }
            
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
            {
                var width = UseStretch() ? Width.Value : Texture.width;
                var height = UseStretch() ? Height.Value : Texture.height;

                GUI.DrawTexture(new Rect(loc.x, loc.y, width, height), Texture);
            }
        }

        #endregion

        private bool UseStretch()
        {
            return Fill == ImageFill.Stretch && Height.HasValue && Width.HasValue;
        }
    }
}
