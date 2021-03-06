﻿using FirstWave.Unity.Gui.Enums;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FirstWave.Unity.Gui.Panels
{
    public class UniformGrid : Panel
    {
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(UniformGrid), new PropertyMetadata(Orientation.Horizontal));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public UniformGrid()
        {
        }

        public override void Layout(Rect r)
        {
            if (Location.HasValue)
                return;

			// Wait for the next measure pass
			if (!Size.HasValue)
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

				// Now reset each childs size to the biggest both in heighth and width
				foreach(var child in Children)
                    child.Size = new Vector2(childMaxWidth, height);
            }
            else
            {
                float childMaxHeight = childSizes.Select(cs => cs.y).Max();
                height = childMaxHeight * Children.Count;

                width = childSizes.Select(cs => cs.x).Max();

				foreach (var child in Children)
					child.Size = new Vector2(width, childMaxHeight);
            }

            Size = new Vector2(width, height);

            return Size.Value;
        }
    }
}
