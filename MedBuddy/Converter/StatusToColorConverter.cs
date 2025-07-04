using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MedBuddy.Converter
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                if (status.ToLower() == "genommen")
                    return new SolidColorBrush(Color.FromRgb(46, 204, 113)); // Grün
                if (status.ToLower() == "vergessen")
                    return new SolidColorBrush(Color.FromRgb(231, 76, 60)); // Rot
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 