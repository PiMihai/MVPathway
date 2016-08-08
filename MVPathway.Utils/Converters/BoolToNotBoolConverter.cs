using System;
using Xamarin.Forms;

namespace MVPathway.Utils.Converters
{
  public class BoolToNotBoolConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return !(bool)value;
    }
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return !(bool)value;
    }
  }
}
