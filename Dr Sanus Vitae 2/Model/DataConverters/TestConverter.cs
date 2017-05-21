using System;
using System.ComponentModel;
using System.Windows.Data;

namespace SanusVitae
{
    public class TestConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is MedicalTest))
                return "";
            else return ((DisplayNameAttribute)value.GetType().GetCustomAttributes(typeof(DisplayNameAttribute), true)[0]).DisplayName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return "";
        }
    }
}
