using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace WallHavenGui.Converter
{
    internal class ViewConvert: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                try
                {
                    //此错误时下面的ToInt32转换的错误，GridView自带的虚拟化会再次启动转换器，这时的转换器值为32W+，这只是一个例子，只要出错的话就会返回原本的值。
                    //意思是说，只要转换一次就够了。
                    int Value = System.Convert.ToInt32(value);
                    if (Value > 10000)
                    {
                        string stringvaue = (Value / 10000).ToString();
                        return stringvaue + "W+";
                    }
                    return Value.ToString();
                }
                catch (Exception)
                {
                    return value.ToString();
                }
                
            }
            else
            {
                return "0";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            string v = value as string;
            return v;
        }
    }

    internal class FactorConvert:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            float f = System.Convert.ToSingle(value);
            return Math.Round((f * 100)).ToString()+"%";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
