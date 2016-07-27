using System.Linq;
using UnityEngine;

namespace FirstWave.Unity.Gui.Panels
{
    /// <summary>
    /// This is like a super dumb grid, used primarily to lay stuff on top of each other. The last item added will be on top.
    /// </summary>
    public class OverlayPanel : Panel
    {
        public override void Layout(Rect r)
        {
            if (Location.HasValue)
                return;

            Location = new Vector2(GetStartingXCoordinate(r), GetStartingYCoordinate(r));

            foreach (var c in Children)
                c.Layout(new Rect(Location.Value, Size.Value));
        }

        public override Vector2 Measure()
        {
            foreach (var c in Children)
                c.Measure();

            var maxWidth = Children.Max(c => c.Size.Value.x);
            var maxHeight = Children.Max(c => c.Size.Value.y);

            Size = new Vector2(maxWidth, maxHeight);

            return Size.Value;
        }
    }
}
