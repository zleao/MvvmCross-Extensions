using System.Windows.Input;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using MvvmCrossUtilities.Plugins.Device;
using MvvmCrossUtilities.Plugins.Storage;
using MvvmCrossUtilities.Samples.AllAround.Core.ViewModels.Base;

namespace MvvmCrossUtilities.Samples.AllAround.Core.ViewModels
{
    public class DeviceViewModel : AllAroundViewModel
    {
        #region Properties

        public override string PageTitle
        {
            get { return "Device info"; }
        }

        protected IDevice Device
        {
            get { return _device ?? (_device = Mvx.Resolve<IDevice>()); }
        }
        private IDevice _device;

        protected IStorageManager StorageService
        {
            get { return _storageService ?? (_storageService = Mvx.Resolve<IStorageManager>()); }
        }
        private IStorageManager _storageService;

        public bool BackCameraSupported
        {
            get { return Device.BackCameraSupported; }
        }

        public bool FrontCameraSupported
        {
            get { return Device.FrontCameraSupported; }
        }

        public uint GetAvailableVirtualMemory
        {
            get { return Device.GetAvailableVirtualMemory(); }
        }

        public string ID
        {
            get { return Device.ID; }
        }

        public BatteryLevelEnum BatteryLevel
        {
            get { return Device.BatteryPower; }
        }

        public NetworkTypeEnum NetworkType
        {
            get { return Device.NetworkType; }
        }

        public bool WifiConnected
        {
            get { return Device.WifiConnected; }
        }

        public int PhoneCallsMissed
        {
            get { return Device.PhoneCallsMissed; }
        }

        public int TimeZoneBias
        {
            get { return Device.TimeZoneBias; }
        }

        public ulong AvailableFreeSpace
        {
            get { return Device.GetAvailableFreeSpace(StorageService.NativePath(StorageLocation.ExternalPublic, string.Empty)); }
        }

        /// <summary>
        /// Gets the manufacturer.
        /// </summary>
        /// <value>
        /// The manufacturer.
        /// </value>
        public string Manufacturer
        {
            get { return Device.Manufacturer; }
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public string Model
        {
            get { return Device.Model; }
        }

        /// <summary>
        /// Gets the brand.
        /// </summary>
        /// <value>
        /// The brand.
        /// </value>
        public string Brand
        {
            get { return Device.Brand; }
        }

        #endregion

        #region Command

        public ICommand RefreshCommand
        {
            get { return _refreshCommand; }
        }
        private readonly ICommand _refreshCommand;

        #endregion

        #region Constructor

        public DeviceViewModel()
        {
            _refreshCommand = new MvxCommand(OnRefresh);
        }

        #endregion

        #region Methods

        private void OnRefresh()
        {
            RaisePropertyChanged(() => BackCameraSupported);
            RaisePropertyChanged(() => FrontCameraSupported);
            RaisePropertyChanged(() => GetAvailableVirtualMemory);
            RaisePropertyChanged(() => ID);
            RaisePropertyChanged(() => BatteryLevel);
            RaisePropertyChanged(() => NetworkType);
            RaisePropertyChanged(() => WifiConnected);
            RaisePropertyChanged(() => PhoneCallsMissed);
            RaisePropertyChanged(() => TimeZoneBias);
            RaisePropertyChanged(() => AvailableFreeSpace);
        }

        #endregion
    }
}
