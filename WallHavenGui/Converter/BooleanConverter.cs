using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace WallEventGUI.Converter
{
    public class BooleanConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool bo1 = System.Convert.ToBoolean(value);
            if (bo1 == true)
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            bool bo1 =  System.Convert.ToBoolean( value);
            if (bo1 == true)
                return true;
            else
                return false;
        }
    }
}
                                             