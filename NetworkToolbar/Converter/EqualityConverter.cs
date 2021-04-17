using System;
using System.CodeDom;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Navigation;

namespace NetworkToolbar.Converter
{
    public class EqualityConverter : MarkupExtension, IValueConverter
    {
        public object ExpectedObject { get; set; }
        
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(targetType == typeof(bool) || targetType == typeof(Boolean))
            {
                if(ExpectedObject is Enum)
                {
                    int expectedObject = (int) ExpectedObject; 
                    return (int)value == expectedObject;
                }
                
                return value == ExpectedObject;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}