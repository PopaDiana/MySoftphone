using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace MySoftphone.UI
{
    public class BuilderToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            StringBuilder stringBuilder = value as StringBuilder;

            if (stringBuilder == null)
                return string.Empty;

            return stringBuilder.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}