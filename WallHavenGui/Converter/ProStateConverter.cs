using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace WallEventGUI.Converter
{
    public class ProStateConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {            var result = System.Convert.ToBoolean(value);
            if (result == true)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var result = System.Convert.ToBoolean(value);
            if (result == true)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public class VisStateConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var result = System.Convert.ToBoolean(value);
            if (result == true)
            {
                return Visibility.Visible;
            }else
            {
                return Visibility.Collapsed;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var result = System.Convert.ToBoolean(value);
            if (result == true)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }
    }
}
