using System;

namespace FirstWave.Unity.Gui
{
    public class PropertyMetadata
    {
        public object DefaultValue { get; private set; }

        public bool AffectsLayout { get; private set; }

        public Action<Control, object, object> OnChangeHandler { get; private set; }

        public PropertyMetadata(object defaultValue, bool affectsLayout = true)
        {
            DefaultValue = defaultValue;
            AffectsLayout = affectsLayout;
        }

        public PropertyMetadata(object defaultValue, Action<Control, object, object> changeHandler, bool affectsLayout = true)
            : this(defaultValue, affectsLayout)
        {
            OnChangeHandler = changeHandler;
        }
    }
}
