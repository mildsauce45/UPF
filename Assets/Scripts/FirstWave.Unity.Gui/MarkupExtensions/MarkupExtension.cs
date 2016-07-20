namespace FirstWave.Unity.Gui.MarkupExtensions
{
    public abstract class MarkupExtension
    {
        public abstract string Key { get; }

        public abstract object GetValue();
        public abstract void Load(Control c, string[] parms);
    }
}
