using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Media;
using Android.Net;
using Android.OS;
using Android.Provider;
using Android.Telephony;
using Android.Util;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Droid;
using Java.Lang;
using Java.Lang.Reflect;
using Java.Util;

namespace MvvmCrossUtilities.Plugins.Device.Droid
{
    public class Device : IDevice
    {
        #region Fields

        AudioManager _audioManager;
        SoundPool _soundPool;
        HashMap _soundPoolMap;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the application context.
        /// </summary>
        /// <value>
        /// The application context.
        /// </value>
        protected Context ApplicationContext
        {
            get
            {
                return Mvx.Resolve<IMvxAndroidGlobals>().ApplicationContext;
            }
        }

        /// <summary>
        /// Indicates if the color camera is supported
        /// </summary>
        public bool ColorCameraSupported
        {
            get
            {
                return ApplicationContext.PackageManager.HasSystemFeature(PackageManager.FeatureCamera);
            }
        }

        /// <summary>
        /// Indicates if the imager camera is supported
        /// </summary>
        public bool ImagerCameraSupported
        {
            get
            {
                return ApplicationContext.PackageManager.HasSystemFeature(PackageManager.FeatureCamera);
            }
        }

        /// <summary>
        /// Returns the serial number of the device
        /// </summary>
        public string ID
        {
            get
            {
                if (string.IsNullOrEmpty(_id))
                {
                    var telephonyManager = (TelephonyManager)ApplicationContext.GetSystemService(Context.TelephonyService);
                    if (telephonyManager == null || telephonyManager.DeviceId == null)
                        _id = Android.OS.Build.Serial;
                    else
                        _id = telephonyManager.DeviceId;
                }

                return _id;
            }
        }
        private string _id;

        /// <summary>
        /// Gets a value indicating whether this instance has wifi security mixed mode.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has wifi security mixed mode; otherwise, <c>false</c>.
        /// </value>
        public bool HasWifiSecurityMixedMode
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns the current battery power
        /// </summary>
        public BatteryLevelEnum BatteryPower
        {
            get
            {
                #region Get Battery Level

                var batLevel = 0f;

                Intent batteryIntent = ApplicationContext.RegisterReceiver(null, new IntentFilter(Intent.ActionBatteryChanged));
                int level = batteryIntent.GetIntExtra(BatteryManager.ExtraLevel, -1);
                int scale = batteryIntent.GetIntExtra(BatteryManager.ExtraScale, -1);

                // Error checking that probably isn't needed but I added just in case.
                if (level == -1 || scale == -1)
                {
                    batLevel = 50.0f;
                }

                batLevel = ((float)level / (float)scale) * 100.0f;

                #endregion

                if (batLevel < 21f)
                    return BatteryLevelEnum.VeryLow;
                else if (batLevel < 41f)
                    return BatteryLevelEnum.Low;
                else if (batLevel < 61f)
                    return BatteryLevelEnum.Medium;
                else if (batLevel < 81f)
                    return BatteryLevelEnum.High;
                else
                    return BatteryLevelEnum.VeryHigh;
            }
        }

        /// <summary>
        /// Returns the type of Network to which the device is currently connected
        /// </summary>
        public NetworkTypeEnum NetworkType
        {
            get
            {
                var telephonyManager = ApplicationContext.GetSystemService(Context.TelephonyService) as TelephonyManager;
                switch (telephonyManager.NetworkType)
                {
                    case Android.Telephony.NetworkType.Cdma:
                        return NetworkTypeEnum.Cdma;
                    case Android.Telephony.NetworkType.Edge:
                        return NetworkTypeEnum.Edge;
                    case Android.Telephony.NetworkType.Ehrpd:
                        return NetworkTypeEnum.Ehrpd;
                    case Android.Telephony.NetworkType.Evdo0:
                        return NetworkTypeEnum.Evdo0;
                    case Android.Telephony.NetworkType.EvdoA:
                        return NetworkTypeEnum.EvdoA;
                    case Android.Telephony.NetworkType.EvdoB:
                        return NetworkTypeEnum.EvdoB;
                    case Android.Telephony.NetworkType.Gprs:
                        return NetworkTypeEnum.Gprs;
                    case Android.Telephony.NetworkType.Hsdpa:
                        return NetworkTypeEnum.Hsdpa;
                    case Android.Telephony.NetworkType.Hspa:
                        return NetworkTypeEnum.Hspa;
                    case Android.Telephony.NetworkType.Hspap:
                        return NetworkTypeEnum.Hspap;
                    case Android.Telephony.NetworkType.Hsupa:
                        return NetworkTypeEnum.Hsupa;
                    case Android.Telephony.NetworkType.Iden:
                        return NetworkTypeEnum.Iden;
                    case Android.Telephony.NetworkType.Lte:
                        return NetworkTypeEnum.Lte;
                    case Android.Telephony.NetworkType.OneXrtt:
                        return NetworkTypeEnum.Xrtt;
                    case Android.Telephony.NetworkType.Umts:
                        return NetworkTypeEnum.Umts;
                    case Android.Telephony.NetworkType.Unknown:
                        return NetworkTypeEnum.Unknown;
                    default:
                        return NetworkTypeEnum.Unknown;
                }
            }
        }

        /// <summary>
        /// Returns if the wifi is connected
        /// </summary>
        public bool WifiConnected
        {
            get
            {

                var connectivityManager = ApplicationContext.GetSystemService(Context.ConnectivityService) as ConnectivityManager;
                return (connectivityManager.ActiveNetworkInfo.Type == ConnectivityType.Wifi);
            }
        }

        /// <summary>
        /// Returns the number of phone calls missed
        /// </summary>
        public int PhoneCallsMissed
        {
            get
            {
                string[] projection = { CallLog.Calls.CachedName, CallLog.Calls.CachedNumberLabel, CallLog.Calls.Type };
                string where = CallLog.Calls.Type + "=3";
                var result = ApplicationContext.ContentResolver.Query(CallLog.Calls.ContentUri, projection, where, null, null);
                result.MoveToFirst();

                return result.Count;
            }
        }

        /// <summary>
        /// Gets the time zone bias.
        /// </summary>
        /// <value>
        /// The time zone bias.
        /// </value>
        public int TimeZoneBias
        {
            get
            {
                var offset = System.TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
                return (int)offset.TotalMinutes;
            }
        }

        /// <summary>
        /// Gets the manufacturer.
        /// </summary>
        /// <value>
        /// The manufacturer.
        /// </value>
        public string Manufacturer
        {
            get
            {
                return Android.OS.Build.Manufacturer;
            }
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public string Model
        {
            get
            {
                return Android.OS.Build.Model;
            }
        }

        /// <summary>
        /// Gets the brand.
        /// </summary>
        /// <value>
        /// The brand.
        /// </value>
        public string Brand
        {
            get
            {
                return Android.OS.Build.Brand;
            }
        }

        /// <summary>
        /// Gets a value indicating whether has dedicated scanner.
        /// </summary>
        /// <value>
        /// <c>true</c> if has dedicated scanner; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool HasDedicatedScanner
        {
            get
            {
                return Helpers.HasDedicatedScanner(Model);
                //return Helpers.HasDedicatedScanner(Manufacturer, Model);
            }
        }

        public string OSVersion
        {
            get { return ((int)Android.OS.Build.VERSION.SdkInt).ToString(); }
        }

        #endregion

        #region Constructor

        public Device()
        {
            #region Sounds Loading

            //set up our audio player
            _audioManager = (AudioManager)this.ApplicationContext.GetSystemService(Context.AudioService);
            _soundPool = new SoundPool(6, Stream.Notification, 0);
            _soundPoolMap = new HashMap();

            //load fx
            _soundPoolMap.Put(SoundTypeEnum.OneBeep.ToString(), _soundPool.Load(ApplicationContext, Resource.Raw.OneBeep, 1));
            _soundPoolMap.Put(SoundTypeEnum.TwoBeeps.ToString(), _soundPool.Load(ApplicationContext, Resource.Raw.TwoBeeps, 1));
            _soundPoolMap.Put(SoundTypeEnum.FourBeeps.ToString(), _soundPool.Load(ApplicationContext, Resource.Raw.FourBeeps, 1));

            #endregion
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the available virtual memory in MB
        /// </summary>
        public uint GetAvailableVirtualMemory()
        {
            ActivityManager activityManager = (ActivityManager)ApplicationContext.GetSystemService(Context.ActivityService);
            Android.App.ActivityManager.MemoryInfo mi = new Android.App.ActivityManager.MemoryInfo();
            activityManager.GetMemoryInfo(mi);
            long availableMegs = mi.AvailMem / 1048576L;

            return Convert.ToUInt32(availableMegs);
        }

        /// <summary>
        /// Plays a sound
        /// </summary>
        /// <param name="sound">The type of sound to play</param>
        public void PlaySound(SoundTypeEnum sound)
        {
            var streamVolume = _audioManager.GetStreamVolume(Stream.Notification);

            _soundPool.Play((int)_soundPoolMap.Get(sound.ToString()), streamVolume, streamVolume, 1, 0, 1f);

        }

        /// <summary>
        /// Sets the current country on the device
        /// </summary>
        /// <param name="locale"></param>
        public void SetDefaultLocale(System.Globalization.CultureInfo locale)
        {
            try
            {
                Class amnClass = Class.ForName("android.app.ActivityManagerNative");
                Java.Lang.Object amn = null;
                Configuration config = null;

                // amn = ActivityManagerNative.getDefault();
                Method methodGetDefault = amnClass.GetMethod("getDefault");
                methodGetDefault.Accessible = true;
                amn = methodGetDefault.Invoke(amnClass);

                Method methodGetConfiguration = amnClass.GetMethod("getConfiguration");
                methodGetConfiguration.Accessible = true;
                config = (Configuration)methodGetConfiguration.Invoke(amn);

                Class configClass = config.Class;
                Field f = configClass.GetField("userSetLocale");
                f.SetBoolean(config, true);

                // set the locale to the new value
                var l = new Locale(locale.Name);
                config.Locale = l;

                // amn.updateConfiguration(config);
                Method methodUpdateConfiguration = amnClass.GetMethod("updateConfiguration", config.Class);
                methodUpdateConfiguration.Accessible = true;
                methodUpdateConfiguration.Invoke(amn, config);

            }
            catch (Java.Lang.Exception e)
            {
                // TODO: handle exception
                Log.Debug("error lang change-->", "" + e.Message);
            }
        }

        /// <summary>
        /// Returns the available free space in bytes for a given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ulong GetAvailableFreeSpace(string path)
        {
            StatFs statFs = new StatFs(path);
            long freeBytes = statFs.FreeBlocks * statFs.BlockSize;

            return Helpers.ConvertBytesToMegabytes(freeBytes);
        }

        #endregion        
    }
}