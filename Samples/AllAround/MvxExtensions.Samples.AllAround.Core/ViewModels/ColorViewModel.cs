using MvvmCross.Localization;
using MvvmCross.Platform.Platform;
using MvxExtensions.Libraries.Portable.Core.Models;
using MvxExtensions.Libraries.Portable.Core.Services.Logger;
using MvxExtensions.Plugins.Notification;
using MvxExtensions.Samples.AllAround.Core.Models;
using MvxExtensions.Samples.AllAround.Core.ViewModels.Base;
using System.Collections.ObjectModel;

namespace MvxExtensions.Samples.AllAround.Core.ViewModels
{
    public class ColorViewModel : SimpleMenuBaseViewModel
    {
        #region Properties

        public ObservableCollection<ColorItem> ColorList
        {
            get { return _colorList; }
        }
        private readonly ObservableCollection<ColorItem> _colorList = new ObservableCollection<ColorItem>();

        #endregion

        #region Constructor

        public ColorViewModel(IMvxLanguageBinder textSource,
                              IMvxJsonConverter jsonConverter,
                              INotificationService notificationManager,
                              ILoggerManager loggerManager)
            : base(textSource, jsonConverter, notificationManager, loggerManager)
        {
            ColorList.Add(new ColorItem() { Value = Color.Black, Name = "Black" });
            ColorList.Add(new ColorItem() { Value = Color.White, Name = "White" });
            ColorList.Add(new ColorItem() { Value = Color.Red, Name = "Red" });
            ColorList.Add(new ColorItem() { Value = Color.Lime, Name = "Lime" });
            ColorList.Add(new ColorItem() { Value = Color.Blue, Name = "Blue" });
            ColorList.Add(new ColorItem() { Value = Color.Yellow, Name = "Yellow" });
            ColorList.Add(new ColorItem() { Value = Color.Cyan, Name = "Cyan" });
            ColorList.Add(new ColorItem() { Value = Color.Magenta, Name = "Magenta" });
            ColorList.Add(new ColorItem() { Value = Color.Silver, Name = "Silver" });
            ColorList.Add(new ColorItem() { Value = Color.Gray, Name = "Gray" });
            ColorList.Add(new ColorItem() { Value = Color.Maroon, Name = "Maroon" });
            ColorList.Add(new ColorItem() { Value = Color.Olive, Name = "Olive" });
            ColorList.Add(new ColorItem() { Value = Color.Green, Name = "Green" });
            ColorList.Add(new ColorItem() { Value = Color.Purple, Name = "Purple" });
            ColorList.Add(new ColorItem() { Value = Color.Teal, Name = "Teal" });
            ColorList.Add(new ColorItem() { Value = Color.Navy, Name = "Navy" });
        }

        #endregion
    }
}
