using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace MobileClient.Converters
{
    class BoolToLoggedInConverter : IValueConverter
    {
		public static BoolToLoggedInConverter Default = new BoolToLoggedInConverter();

	    /// <inheritdoc />
	    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    var v = (bool) value;
		    return v ? "Logout" : "Login";
	    }

	    /// <inheritdoc />
	    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    throw new NotImplementedException();
	    }
    }
}
