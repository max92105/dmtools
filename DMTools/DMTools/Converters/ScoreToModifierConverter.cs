using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DMTools.Converters
{
    public class ZeroToEmptyStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try { return System.Convert.ToInt32(value) == 0 ? "" : value.ToString(); }
            catch { return ""; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s && !string.IsNullOrWhiteSpace(s) && short.TryParse(s, out short r))
                return r;
            return (short)0;
        }
    }

    public class ScoreToModifierConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                int score = System.Convert.ToInt32(value);
                int mod = (int)Math.Floor((score - 10) / 2.0);
                return mod >= 0 ? $"+{mod}" : mod.ToString();
            }
            catch { return "+0"; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class IsZeroToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try { return System.Convert.ToInt32(value) == 0 ? Visibility.Visible : Visibility.Collapsed; }
            catch { return Visibility.Collapsed; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    // Converts a CR decimal to the D&D 5e proficiency bonus string (e.g. 5m → "+3").
    // Used as a placeholder so the PB field shows the computed default when empty.
    public class CrToProficiencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                decimal cr = System.Convert.ToDecimal(value);
                int pb;
                if      (cr <= 4)  pb = 2;
                else if (cr <= 8)  pb = 3;
                else if (cr <= 12) pb = 4;
                else if (cr <= 16) pb = 5;
                else if (cr <= 20) pb = 6;
                else if (cr <= 24) pb = 7;
                else if (cr <= 28) pb = 8;
                else               pb = 9;
                return $"+{pb}";
            }
            catch { return "+2"; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
