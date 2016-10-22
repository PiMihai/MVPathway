using System;
using System.Globalization;
using Xamarin.Forms;

namespace MVPathway.Utils.Converters
{
  public class BoolToStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var boolean = (bool)value;
      var options = (parameter as string)?.Split(',');
      if(options == null || string.IsNullOrEmpty(options[0]) || string.IsNullOrEmpty(options[1]))
      {
        return "INVALID_CONVERTER_PARAM";
      }
      return boolean ? options[0] : options[1];
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var stringValue = value as string;
      var options = (parameter as string)?.Split(',');
      if (options == null || string.IsNullOrEmpty(options[0]) || string.IsNullOrEmpty(options[1]))
      {
        return "INVALID_CONVERTER_PARAM";
      }
      return stringValue == options[0];
    }
  }
}
