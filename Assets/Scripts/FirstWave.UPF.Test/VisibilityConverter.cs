using FirstWave.Unity.Gui.Data;
using FirstWave.Unity.Gui.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FirstWave.UPF.Test
{
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object obj, object parameter)
        {
            if (obj == null)
                return Visibility.Collapsed;
            else if (obj.GetType() == typeof(bool))
                return (bool)obj ? Visibility.Visible : Visibility.Collapsed;

            return Visibility.Visible;
        }
    }
}
