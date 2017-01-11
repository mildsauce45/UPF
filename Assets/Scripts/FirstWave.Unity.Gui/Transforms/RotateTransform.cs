using UnityEngine;

namespace FirstWave.Unity.Gui.Transforms
{
    public class RotateTransform : Transform
    {
        public float Angle { get; set; }
        public float OriginX { get; set; }
        public float OriginY { get; set; }

        private Matrix4x4 transformMatrix;
        private Control control;

        public RotateTransform()
        {
            OriginX = 0.5f;
            OriginY = 0.5f;
        }

        public override void TransformElement(Control transformedElement)
        {
            if (transformedElement == null || transformedElement.Location == null || transformedElement.Size == null)
                return;

            transformMatrix = GUI.matrix;
            control = transformedElement;

            var loc = control.Location.Value;
            var size = control.Size.Value;

            var pivot = new Vector2(loc.x + (size.x / (1 / OriginX)), loc.y + (size.y / (1 / OriginY)));

            GUIUtility.RotateAroundPivot(Angle, pivot);
        }

        public override void AfterTransform()
        {
            if (control == null || control.Location == null || control.Size == null)
                return;

            base.AfterTransform();

            GUI.matrix = transformMatrix;
        }
    }
}
