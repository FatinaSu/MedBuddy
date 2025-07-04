using System;
using System.Globalization;
using System.Windows.Data;

namespace MedBuddy.Converter
{
    public class StatusToSymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                if (status.ToLower() == "genommen")
                    return "✔";
                if (status.ToLower() == "vergessen")
                    return "✖";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 