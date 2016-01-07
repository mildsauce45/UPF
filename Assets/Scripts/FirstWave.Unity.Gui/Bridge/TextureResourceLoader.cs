using System.Collections.Generic;
using UnityEngine;

namespace FirstWave.Unity.Gui.Bridge
{
    public class TextureResourceLoader : IResourceLoader<Texture>
    {
        private static IDictionary<string, Texture> textureCache;

        public string ResourcePath { get; set; }

        public TextureResourceLoader()
        {
            textureCache = new Dictionary<string, Texture>();
        }

        public TextureResourceLoader(string resourcePath)
            : this()
        {
            ResourcePath = resourcePath;
        }

        public Texture LoadResource(string name)
        {
            var resourceKey = string.Format("{0}/{1}", ResourcePath, name);

            if (textureCache.ContainsKey(resourceKey))
                return textureCache[resourceKey];

            var texture = Resources.Load(resourceKey) as Texture;

            if (texture != null)
                textureCache.Add(resourceKey, texture);

            return texture;
        }
    }
}
