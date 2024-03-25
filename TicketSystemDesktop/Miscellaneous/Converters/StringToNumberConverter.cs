using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace TicketSystemDesktop.Miscellaneous.Converters
{
    public class StringToNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string stringValue = (string)value;

            if (stringValue == null || stringValue == "")
                return null;

            var parsable = long.TryParse(stringValue, out long numberValue);

            if (!parsable)
            {
                return value;
            }

            return numberValue;
        }
    }
}
