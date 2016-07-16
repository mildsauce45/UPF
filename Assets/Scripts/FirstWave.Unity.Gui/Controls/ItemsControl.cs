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

        public static readonly DependencyProperty ItemsPanelProperty =
            DependencyProperty.Register("ItemsPanel", typeof(Panel), typeof(ItemsControl), new PropertyMetadata(new StackPanel()));

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

        public Panel ItemsPanel
        {
            get { return (Panel)GetValue(ItemsPanelProperty); }
            set { SetValue(ItemsPanelProperty, value); }
        }

        #endregion

        #region UPF Engine

        public override Vector2 Measure()
        {
            if (Size.HasValue)
                return Size.Value;

            // Clear out any existing children and pseudo-invalidate the panel
            ItemsPanel.Children.Clear();
            ItemsPanel.Size = null;
            ItemsPanel.Location = null;

            if (ItemsSource == null || ItemsSource.OfType<object>().Count() == 0)
            {
                // If there are no items, then go ahead and return zero
                Size = Vector2.zero;
                return Vector2.zero;
            }

            // Now let's add back in any children items we need
            foreach (var item in ItemsSource)
                ItemsPanel.AddChild(ItemTemplate.GenerateItem(item));

            // Now let's measure the size of the panel
            var size = ItemsPanel.Measure();

            Size = size;

            return size;
        }

        public override void Layout(Rect r)
        {
            if (Location.HasValue)
                return;

            ItemsPanel.Layout(r);
        }

        public override void Draw()
        {
            ItemsPanel.Draw();
        }

        #endregion
    }
}
