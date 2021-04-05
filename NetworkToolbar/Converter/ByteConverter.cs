using System;
using System.Globalization;
using System.Windows.Data;

namespace NetworkToolbar.Converter
{
    public class ByteConverter : IValueConverter
    {
        const long KB = 1000;
        const long MB = KB * KB;
        const long GB = MB * MB;
        //const ulong TB = GB * GB;
        //const ulong PB = TB * TB;
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double count = value is double ? (double) value : 0;
            
            /*if (count >= PB) {
                double pb = count / PB;
                return pb > 10 ? $"{pb:N0} P" : $"{pb:N1} P"; 
            }
            if (count >= TB) {
                double tb = count / TB;
                return tb > 10 ? $"{tb:N0} T" : $"{tb:N1} T";
            }*/
            if (count >= GB) {
                double v = count / GB;
                return v > 10 ? $"{v:N0} G" : $"{v:N1} G";
            }
            if (count >= MB) {
                double v = count / MB;
                return v > 10 ? $"{v:N0} M" : $"{v:N1} M";
            }
            if (count >= KB) {
                double v = count / KB;
                return v > 10 ? $"{v:N0} K" : $"{v:N1} K";
            }

            return count > 10 ? $"{count:N0}" : $"{count:N1}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}