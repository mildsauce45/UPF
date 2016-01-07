namespace FirstWave.Unity.Gui
{
    public class Thickness
    {
        public static readonly Thickness ZERO = new Thickness(0);

        public float Left { get; private set; }
        public float Right { get; private set; }
        public float Top { get; private set; }
        public float Bottom { get; private set; }

        public Thickness(float allMargins)
        {
            Left = allMargins;
            Right = allMargins;
            Top = allMargins;
            Bottom = allMargins;
        }

        public Thickness(float lr, float tb)
        {
            Left = lr;
            Right = lr;

            Top = tb;
            Bottom = tb;
        }

        public Thickness(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }
}
