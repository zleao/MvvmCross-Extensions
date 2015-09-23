using System.Collections.Generic;
using System.Linq;

#if WINDOWS_PHONE
namespace MvvmCrossUtilities.Plugins.Device.WindowsPhone
#elif WINDOWS_PHONE_APP
namespace MvvmCrossUtilities.Plugins.Device.WindowsPhoneStore
#elif WINDOWS_APP
namespace MvvmCrossUtilities.Plugins.Device.WindowsStore
#elif MONODROID
namespace MvvmCrossUtilities.Plugins.Device.Droid
#else
namespace MvvmCrossUtilities.Plugins.Device
#endif
{
    public static class Helpers
    {
        #region Constants

        private static readonly long MEGA_BYTE = 1048576;

        private static readonly IList<KeyValuePair<string, string>> DevicesWithEmbededScanner;

        #endregion

        #region Properties

        private static System.StringComparison ComparisonType 
        { 
            get
            {
#if WINDOWS_PHONE_APP || WINDOWS_APP
                return System.StringComparison.OrdinalIgnoreCase;
#else
                return System.StringComparison.InvariantCultureIgnoreCase;
#endif
            } 
        }

        #endregion

        #region Static Constructor

        static Helpers()
        {
            DevicesWithEmbededScanner = new List<KeyValuePair<string, string>>();

            DevicesWithEmbededScanner.Add(new KeyValuePair<string, string>("motorola", "tc55"));
            DevicesWithEmbededScanner.Add(new KeyValuePair<string, string>("motorola", "et1"));
            //This device was needed because the Android.OS.Build.Manufacturer on the Motorola TC55 sometimes returns "motorola solutions" instead of "motorola"
            DevicesWithEmbededScanner.Add(new KeyValuePair<string, string>("motorola solutions", "tc55"));
        }

        #endregion

        #region Methods

        public static bool HasDedicatedScanner(string manufacturer, string model)
        {
            if (!string.IsNullOrEmpty(manufacturer) && !string.IsNullOrEmpty(model))
            {
                var key = manufacturer.ToLower();

                var manufacts = DevicesWithEmbededScanner.Where((d => d.Key.Equals(manufacturer, ComparisonType)));
                if(manufacts != null && manufacts.Count() > 0)
                {
                    return manufacts.Any(m => m.Value.Equals(model, ComparisonType));
                }
            }

            return false;
        }

        public static bool HasDedicatedScanner(string model)
        {
            if (!string.IsNullOrEmpty(model))
            {
                return DevicesWithEmbededScanner.Any(d => d.Value.Equals(model, ComparisonType));
            }

            return false;
        }

        public static ulong ConvertBytesToMegabytes(long bytesToConvert)
        {
            return (ulong)(bytesToConvert / MEGA_BYTE);
        }

        #endregion
    }
}