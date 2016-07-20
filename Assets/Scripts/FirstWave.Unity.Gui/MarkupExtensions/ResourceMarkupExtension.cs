using FirstWave.Unity.Gui.Utilities;

namespace FirstWave.Unity.Gui.MarkupExtensions
{

    public class ResourceMarkupExtension : MarkupExtension
    {
        public override string Key { get { return "Resource"; } }

        public string ResourceKey { get; private set; }

        public override void Load(Control c, string[] parms)
        {
            if (parms.Length == 0)
                return;

            var parts = parms[0].Split(new char[] { '=' });

            if (parts.Length == 1)
                ResourceKey = parts[0];
            if (parts.Length == 2 && parts[0] == "ResourceKey")
                ResourceKey = parts[1];
        }

        public override object GetValue()
        {
            if (string.IsNullOrEmpty(ResourceKey))
                return null;

            return XamlProcessor.resources.ContainsKey(ResourceKey) ? XamlProcessor.resources[ResourceKey] : null;
        }
    }
}
