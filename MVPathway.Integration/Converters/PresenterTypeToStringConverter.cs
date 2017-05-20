using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace MVPathway.Integration.Converters
{
    public class PresenterTypeListToStringConverter : IValueConverter
    {
        public PresenterTypeToStringConverter ForItem { get; private set; }

        public PresenterTypeListToStringConverter()
        {
            ForItem = new PresenterTypeToStringConverter(this);
        }

        public ObservableCollection<Type> SupportedPresenters { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is IEnumerable<Type> types))
            {
                return null;
            }
            return new ObservableCollection<string>(types.Select(
                t => ForItem.Convert(t, typeof(string), null, null) as string));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is IEnumerable<string> types))
            {
                return null;
            }
            return new ObservableCollection<Type>(types.Select(
                t => ForItem.ConvertBack(t, typeof(Type), null, null) as Type));
        }
    }

    public class PresenterTypeToStringConverter : IValueConverter
    {
        private readonly PresenterTypeListToStringConverter _parent;

        public PresenterTypeToStringConverter(PresenterTypeListToStringConverter parent)
        {
            _parent = parent;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Type t))
            {
                return string.Empty;
            }
            return t.Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string s) || string.IsNullOrWhiteSpace(s))
            {
                return null;
            }
            return _parent.SupportedPresenters.FirstOrDefault(p => p.Name == s);
        }
    }
}
