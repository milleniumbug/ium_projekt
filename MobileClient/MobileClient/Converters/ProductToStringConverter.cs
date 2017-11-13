using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Api.Models;
using Xamarin.Forms;

namespace MobileClient.Converters
{
    class ProductToStringConverter : IValueConverter
    {
		public static readonly ProductToStringConverter Default = new ProductToStringConverter();

	    /// <inheritdoc />
	    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	    {
			// xamarin weirdness?
		    if(value == null)
			    return "";
		    var p = (Product) value;
		    return $"{p.Amount}x {p.Name}, available in: {p.ShopName}. Price: {p.Price}";
	    }

	    /// <inheritdoc />
	    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    throw new NotImplementedException();
	    }
    }
}
