using System.Collections.Generic;

namespace FirstWave.Unity.Gui.MarkupExtensions
{

    public class ResourceMarkupExtension : MarkupExtension
    {
        public override string Key { get { return "Resource"; } }

        public string ResourceKey { get; private set; }

        public override void Load(object c, string[] parms)
        {
            if (parms.Length == 0)
                return;

            var parts = parms[0].Split(new char[] { '=' });

            if (parts.Length == 1)
                ResourceKey = parts[0];
            if (parts.Length == 2 && parts[0] == "ResourceKey")
                ResourceKey = parts[1];
        }

        public override object GetValue(IDictionary<string, object> resources)
        {
            if (string.IsNullOrEmpty(ResourceKey))
                return null;

            return resources.ContainsKey(ResourceKey) ? resources[ResourceKey] : null;
        }
    }
}
