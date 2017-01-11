using UnityEngine;

namespace FirstWave.Unity.Gui.Transforms
{
    public class TranslateTransform : Transform
    {
        public float X { get; set; }
        public float Y { get; set; }

        private bool translated = false;

        public override void TransformElement(Control transformedElement)
        {
            if (transformedElement == null || transformedElement.Location == null || translated)
                return;

            var loc = transformedElement.Location.Value;

            transformedElement.Location = new Vector2(loc.x + X, loc.y + Y);
            translated = true;
        }
    }
}
