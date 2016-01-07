using UnityEngine;

namespace FirstWave.Unity.Gui.Drawing
{
    internal class BorderBox
    {
        public static void Draw(Rect location, BorderTextures textures)
        {
            // First draw the background
            if (textures.Background != null)
                UnityEngine.GUI.DrawTexture(new Rect(location.x + textures.BorderVertical.width, location.y + textures.BorderHorizontal.height, location.width - (textures.BorderVertical.width * 2), location.height - (textures.BorderVertical.height * 2)), textures.Background);

            // Draw the upper left corner
            UnityEngine.GUI.DrawTexture(new Rect(location.x, location.y, textures.UpperLeft.width, textures.UpperLeft.height), textures.UpperLeft);

            // Draw upper horizontal texture
            UnityEngine.GUI.DrawTexture(new Rect(location.x + textures.UpperLeft.width, location.y, location.width - (textures.UpperLeft.width + textures.UpperRight.width), textures.BorderHorizontal.height), textures.BorderHorizontal);

            // Draw upper right corner
            UnityEngine.GUI.DrawTexture(new Rect(location.x + location.width - textures.UpperLeft.width, location.y, textures.UpperRight.width, textures.UpperRight.height), textures.UpperRight);

            // Draw right side
            UnityEngine.GUI.DrawTexture(new Rect(location.x + location.width - textures.BorderVertical.width, location.y + textures.UpperRight.height, textures.BorderVertical.width, location.height - (textures.UpperRight.height + textures.LowerRight.height)), textures.BorderVertical);

            // Draw lower right corner
            UnityEngine.GUI.DrawTexture(new Rect(location.x + location.width - textures.LowerRight.width, location.y + location.height - textures.LowerRight.height, textures.LowerRight.width, textures.LowerRight.height), textures.LowerRight);

            // Draw lower horizontal texture
            UnityEngine.GUI.DrawTexture(new Rect(location.x + textures.LowerLeft.width, location.y + location.height - textures.BorderHorizontal.height, location.width - (textures.LowerLeft.width + textures.LowerRight.width), textures.BorderHorizontal.height), textures.BorderHorizontal);

            // Draw lower left corner
            UnityEngine.GUI.DrawTexture(new Rect(location.x, location.y + location.height - textures.LowerLeft.height, textures.LowerLeft.width, textures.LowerLeft.height), textures.LowerLeft);

            // Draw left side
            UnityEngine.GUI.DrawTexture(new Rect(location.x, location.y + textures.UpperLeft.height, textures.BorderVertical.width, location.height - (textures.UpperLeft.height + textures.LowerLeft.height)), textures.BorderVertical);
        }
    }
}
