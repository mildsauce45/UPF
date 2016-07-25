using FirstWave.Unity.Gui.Panels;
using System.Collections;
using UnityEngine;
using System.Linq;

namespace FirstWave.Unity.Gui.Controls
{
    public class ItemsControl : Panel
    {
        #region Dependency Properties

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ItemsControl));

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(Template), typeof(ItemsControl));

        public static readonly DependencyProperty ItemsPanelTemplateProperty =
            DependencyProperty.Register("ItemsPanelTemplate", typeof(Template), typeof(ItemsControl), new PropertyMetadata(null, OnItemsPanelTemplateChanged));

		private static void OnItemsPanelTemplateChanged(Control c, object oldValue, object newValue)
		{
			var ic = c as ItemsControl;
			ic.InvalidateLayout(ic);
		}

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public Template ItemTemplate
        {
            get { return (Template)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public Template ItemsPanelTemplate
        {
            get { return (Template)GetValue(ItemsPanelTemplateProperty); }
            set { SetValue(ItemsPanelTemplateProperty, value); }
        }

        #endregion

        private Panel generatedPanel;

        #region UPF Engine

        public override Vector2 Measure()
        {
            if (Size.HasValue)
                return Size.Value;

            // Clear out any existing children and pseudo-invalidate the panel
            generatedPanel = ItemsPanelTemplate == null ? new StackPanel() : ItemsPanelTemplate.GenerateItem(DataContext) as Panel;

			generatedPanel.Parent = this;

            if (ItemsSource == null || ItemsSource.OfType<object>().Count() == 0)
            {
                // If there are no items, then go ahead and return zero
                Size = Vector2.zero;
                return Vector2.zero;
            }

            // Now let's add back in any children items we need
            foreach (var item in ItemsSource)
                generatedPanel.AddChild(ItemTemplate.GenerateItem(item));

            // Now let's measure the size of the panel
            var size = generatedPanel.Measure();

            Size = size;

            return size;
        }

        public override void Layout(Rect r)
        {
            if (Location.HasValue || generatedPanel == null)
                return;

            generatedPanel.Layout(r);
        }

        public override void Draw()
        {
			if (generatedPanel != null)
				generatedPanel.Draw();
        }

		internal override void InvalidateLayout(Control source)
		{
			generatedPanel = null;

			base.InvalidateLayout(source);
		}

		#endregion
	}
}
