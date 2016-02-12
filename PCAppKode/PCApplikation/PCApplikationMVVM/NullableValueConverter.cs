using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PCApplikationMVVM
{
	public class NullableValueConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (string.IsNullOrEmpty(value.ToString()))
				return null;
			return value;
		}
		#endregion
	}
}
