using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace SpeechRecg.Mobile.Converter
{
    public class StringNullOrEmptyBoolConverter : IValueConverter
    {
        /// <summary>Returns false if string is null or empty
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return false;
            if (value is string)
            {
                var s = value.ToString();
                return !string.IsNullOrWhiteSpace(s);
            }
            else if (value is bool)
            {
                return !(bool)value;
            }
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
