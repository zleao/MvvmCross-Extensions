using System.Globalization;
using Cirrious.CrossCore.UI;
using Cirrious.MvvmCross.Plugins.Color;

namespace MvvmCrossUtilities.Samples.AllAround.Core.Converters
{
    public class ContrastColorConverter : MvxColorValueConverter
    {
        protected override MvxColor Convert(object value, object parameter, CultureInfo culture)
        {
            var input = (MvxColor)value;
            var brightnessToUse = SimpleContrast(input.R, input.G, input.B);
            return new MvxColor(brightnessToUse, brightnessToUse, brightnessToUse);
        }

        private static int SimpleContrast(params int[] value)
        {
            // this is only a very simple contrast method
            // for more advanced methods you need to look at HSV-type approaches

            int max = 0;
            foreach (var v in value)
            {
                if (v > max)
                    max = v;
            }

            return 255 - max;
        }
    }
}
