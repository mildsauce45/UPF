using System.Collections.Generic;

namespace FirstWave.Unity.Gui.MarkupExtensions
{
    public abstract class MarkupExtension
    {
        public abstract string Key { get; }

        public abstract object GetValue(IDictionary<string, object> resources);
        public abstract void Load(object c, string[] parms);
    }
}
