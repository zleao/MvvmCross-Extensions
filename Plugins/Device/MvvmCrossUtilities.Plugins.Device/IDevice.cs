using System;

namespace MvvmCrossUtilities.Plugins.Device
{
    public interface IDevice
    {
        #region Properties

        /// <summary>
        /// Indicates if the back camera is supported
        /// </summary>
        bool BackCameraSupported { get; }

        /// <summary>
        /// Indicates if the front camera is supported
        /// </summary>
        bool FrontCameraSupported { get; }

        /// <summary>
        /// Returns the serial number of the device
        /// </summary>
        string ID { get; }

        /// <summary>
        /// Returns the current battery power
        /// </summary>
        BatteryLevelEnum BatteryPower { get; }

        /// <summary>
        /// Returns the type of Network to which the device is currently connected
        /// </summary>
        NetworkTypeEnum NetworkType { get; }

        /// <summary>
        /// Returns if the wifi is connected
        /// </summary>
        bool WifiConnected { get; }

        /// <summary>
        /// Returns the number of phone calls missed
        /// </summary>
        int PhoneCallsMissed { get; }

        /// <summary>
        /// Returns the current bias, in minutes, for local time translation on this device
        /// </summary>
        int TimeZoneBias { get; }

        /// <summary>
        /// Gets the manufacturer.
        /// </summary>
        /// <value>
        /// The manufacturer.
        /// </value>
        string Manufacturer { get; }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        string Model { get; }

        /// <summary>
        /// Gets the brand.
        /// </summary>
        /// <value>
        /// The brand.
        /// </value>
        string Brand { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the available virtual memory in MB
        /// </summary>
        /// <returns></returns>
        uint GetAvailableVirtualMemory();

        /// <summary>
        /// Returns the available free space in bytes for a given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        ulong GetAvailableFreeSpace(string path);

        /// <summary>
        /// Sets the current country on the device
        /// </summary>
        /// <param name="locale"></param>
        void SetDefaultLocale(System.Globalization.CultureInfo locale);

        #endregion
    }
}
