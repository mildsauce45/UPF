using System.Collections.Generic;
using UnityEngine;

namespace FirstWave.Unity.Gui.Bridge
{
    public class TextureResourceLoader : IResourceLoader<Texture>
    {
        private static IDictionary<string, Texture> textureCache;

        public TextureResourceLoader()
        {
            textureCache = new Dictionary<string, Texture>();
        }

        public Texture LoadResource(string resourcePath)
        {
            if (textureCache.ContainsKey(resourcePath))
                return textureCache[resourcePath];

            var texture = Resources.Load(resourcePath) as Texture;

            if (texture != null)
                textureCache.Add(resourcePath, texture);

            return texture;
        }
    }
}
