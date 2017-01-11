using FirstWave.Unity.Gui.Data;
using System;
using System.Collections.Generic;

namespace FirstWave.Unity.Gui.MarkupExtensions
{
    public class BindingMarkupExtension : MarkupExtension
    {
        private object target;

        public override string Key { get { return "Binding"; } }

        public string Path { get; private set; }
        public BindingMode Mode { get; private set; }
        public string ElementName { get; private set; }
        public string Converter { get; private set; }
        public string ConverterParameter { get; private set; }

        public BindingMarkupExtension()
        {
            Mode = BindingMode.OneWay;
        }

        public override void Load(object c, string[] parms)
        {
            target = c;

            if (parms == null || parms.Length == 0)
				return;

			foreach (var parm in parms)
			{
				var parts = parm.Split(new char[] { '=' });

				if (parts.Length == 1)
					Path = parts[0];
				else if (parts.Length == 2)
				{
                    if (parts[0] == "Path")
                        Path = parts[1];
                    else if (parts[0] == "Mode")
                        Mode = (BindingMode)Enum.Parse(typeof(BindingMode), parts[1]);
                    else if (parts[0] == "ElementName")
                        ElementName = parts[1];
                    else if (parts[0] == "Converter")
                        Converter = parts[1];
                    else if (parts[0] == "ConverterParameter")
                        ConverterParameter = parts[1];
				}
			}
		}

        public override object GetValue(IDictionary<string, object> resources)
        {
            var binding = new Binding(target);
            binding.Path = Path;
            binding.Mode = Mode;
            binding.ElementName = ElementName;

            if (!string.IsNullOrEmpty(Converter) && resources.ContainsKey(Converter))
                binding.Converter = resources[Converter] as IValueConverter;

            binding.ConverterParameter = ConverterParameter;

            return binding;
        }
	}
}
