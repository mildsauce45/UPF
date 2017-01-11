using System.Collections.Generic;
using FirstWave.Unity.Gui.Enums;

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
            if (Visibility != Visibility.Visible)
                return;

            foreach (var child in Children)
            {
                if (child.Location.HasValue && child.Size.HasValue)
                    child.DoDraw();
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

		internal override void ResolveDataContext(object viewModel)
		{
			base.ResolveDataContext(viewModel);

			foreach (var c in Children)
				c.ResolveDataContext(viewModel);
		}
	}
}
