using System.Collections.Generic;

namespace FirstWave.Unity.Gui.Panels
{
    public abstract class Panel : Control
    {
        public IList<Control> Children { get; private set; }

        public Panel()
        {
            Children = new List<Control>();
        }

        public virtual void AddChild(Control control)
        {
            Children.Add(control);

            control.Parent = this;
        }

        public override void Draw()
        {
            foreach (var child in Children)
            {
                if (child.Location.HasValue && child.Size.HasValue)
                    child.Draw();
            }
        }

        internal override void InvalidateLayout(Control source)
        {
            base.InvalidateLayout(source);

            foreach (var c in Children)
            {
                if (c != source)
                    c.InvalidateLayout(source);
            }
        }
    }
}
