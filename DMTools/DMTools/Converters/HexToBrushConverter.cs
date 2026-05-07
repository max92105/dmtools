using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DMTools.Converters
{
    public class HexToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string hex = value as string ?? "#E69A28";
            string alphaHex = parameter as string ?? "FF";
            try
            {
                var color = (Color)ColorConverter.ConvertFromString(hex);
                byte alpha = System.Convert.ToByte(alphaHex, 16);
                return new SolidColorBrush(Color.FromArgb(alpha, color.R, color.G, color.B));
            }
            catch
            {
                return Brushes.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
