using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ApprenticeManager.UI.App.Converters;

/// <summary>Converts a decimal grade value to a colour brush (green ≥ 4.0, red &lt; 4.0).</summary>
public class GradeColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var grade = value switch
        {
            decimal d => (double)d,
            double dbl => dbl,
            _ => -1.0
        };

        if (grade < 0) return Brushes.Gray;
        return grade >= 4.0 ? new SolidColorBrush(Color.FromRgb(27, 128, 53)) : new SolidColorBrush(Color.FromRgb(196, 36, 36));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Returns true when the grade is a passing grade (≥ 4.0); used for row background triggers.</summary>
public class PassingGradeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is double d && d >= 4.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
