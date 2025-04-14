using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace PortfolioSimulationWpf
{
    internal class PnLToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal unRealisedPnl = (decimal)value;
            if(unRealisedPnl > 0)
            {
                return Brushes.Green;
            }else if(unRealisedPnl < 0)
            {
                return Brushes.Red;
            }
            else
            {
                return Brushes.Black;
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
