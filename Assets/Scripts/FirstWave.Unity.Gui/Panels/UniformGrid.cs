using FirstWave.Unity.Gui.Enums;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FirstWave.Unity.Gui.Panels
{
    public class UniformGrid : Panel
    {
        public Orientation Orientation { get; set; }

        public UniformGrid()
        {
            Orientation = Orientation.Horizontal;
        }

        public override void Layout(Rect r)
        {
            if (Location.HasValue)
                return;

            var x = GetStartingXCoordinate(r);
            var y = GetStartingYCoordinate(r);

            if (Orientation == Orientation.Horizontal)
            {
                var itemWidth = HorizontalAlignment == Enums.HorizontalAlignment.Stretch ? r.width / Children.Count : Children.First().Size.Value.x;
                var itemHeight = VerticalAlignment == Enums.VerticalAlignment.Stretch ? r.height : Children.First().Size.Value.y;

                foreach (var child in Children)
                {
                    child.Layout(new Rect(x, y, itemWidth, itemHeight));
                    x += itemWidth;
                }
            }
            else
            {
                var itemWidth = HorizontalAlignment == Enums.HorizontalAlignment.Stretch ? r.width : Children.First().Size.Value.x;
                var itemHeight = VerticalAlignment == Enums.VerticalAlignment.Stretch ? r.height / Children.Count : Children.First().Size.Value.y;

                foreach (var child in Children)
                {
                    child.Layout(new Rect(x, y, itemWidth, itemHeight));
                    y += itemHeight;
                }
            }

            Location = new Vector2(x, y);
        }

        public override Vector2 Measure()
        {
            if (Size.HasValue)
                return Size.Value;

            var childSizes = new List<Vector2>();

            foreach (var child in Children)
                childSizes.Add(child.Measure());

            float height = 0;
            float width = 0;

            if (Orientation == Orientation.Horizontal)
            {
                float childMaxWidth = childSizes.Select(cs => cs.x).Max();
                width = childMaxWidth * Children.Count;

                height = childSizes.Select(cs => cs.y).Max();
            }
            else
            {
                float childMaxHeight = childSizes.Select(cs => cs.y).Max();
                height = childMaxHeight * Children.Count;

                width = childSizes.Select(cs => cs.x).Max();
            }

            Size = new Vector2(width, height);

            return Size.Value;
        }
    }
}
