using System.Collections.Generic;

namespace FirstWave.Unity.Gui.Transforms
{
    public class TransformGroup : Transform
    {
        public IList<Transform> Children { get; private set; }

        public TransformGroup()
        {
            Children = new List<Transform>();
        }

        public override void TransformElement(Control transformedElement)
        {
            for (int i = 0; i < Children.Count; i++)
                Children[i].TransformElement(transformedElement);
        }

        public override void AfterTransform()
        {
            base.AfterTransform();

            // Do this backwards in case multiple transforms altered the GUI.matrix property.
            // This let's us properly unwind the property
            for (int i = Children.Count - 1; i >= 0; i--)
                Children[i].AfterTransform();
        }
    }
}
