using System;
using Xamarin.Forms;

namespace MVPathway.Utils.Converters
{
  public class StringToUrlWebViewSourceConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null || !(value is string))
      {
        return null;
      }
      return new UrlWebViewSource
      {
        Url = value as string
      };
    }
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null || !(value is UrlWebViewSource))
      {
        return null;
      }
      return (value as UrlWebViewSource).Url;
    }
  }
}
