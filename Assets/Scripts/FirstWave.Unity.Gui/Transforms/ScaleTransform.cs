using UnityEngine;

namespace FirstWave.Unity.Gui.Transforms
{
    public class ScaleTransform : Transform
    {
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public float OriginX { get; set; }
        public float OriginY { get; set; }

        private Matrix4x4 transformMatrix;
        private Control control;
        
        public ScaleTransform()
        {
            ScaleX = 1;
            ScaleY = 1;
            OriginX = 0.5f;
            OriginY = 0.5f;
        }

        public override void TransformElement(Control transformedElement)
        {
            if (!CanTransformElement(transformedElement))
                return;

            transformMatrix = GUI.matrix;
            control = transformedElement;

            var loc = control.Location.Value;
            var size = control.Size.Value;

            var pivot = new Vector2(loc.x + (size.x / (1 / OriginX)), loc.y + (size.y / (1 / OriginY)));

            GUIUtility.ScaleAroundPivot(new Vector2(ScaleX, ScaleY), pivot);
        }

        public override void AfterTransform()
        {
            if (!CanTransformElement(control))
                return;

            base.AfterTransform();

            GUI.matrix = transformMatrix;
        }
    }
}
