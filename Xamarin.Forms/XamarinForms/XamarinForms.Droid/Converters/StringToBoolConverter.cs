using System;
using Xamarin.Forms;
using System.Globalization;

namespace XamarinForms.Droid.Models
{
    public class StringToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string valueAsString = value.ToString();
            switch (valueAsString)
            {
                case ("true"):
                    {
                        return true;
                    }
                case ("false"):
                    {
                        return false;
                    }
                default:
                    {
                        return false;
                    }
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}