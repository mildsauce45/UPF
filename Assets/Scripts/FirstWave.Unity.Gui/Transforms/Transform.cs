using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FirstWave.Unity.Gui.Transforms
{
    public abstract class Transform
    {
        public abstract void TransformElement(Control transformedElement);

        /// <summary>
        /// Use this method to clean up anything that may have been needed to be restored after the render pass.
        /// </summary>
        public virtual void AfterTransform()
        {
        }
    }
}
