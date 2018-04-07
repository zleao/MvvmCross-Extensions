using MvvmCross.Localization;
using MvvmCross.Platform.Platform;
using MvvmCross.Platform.WeakSubscription;
using MvxExtensions.Libraries.Portable.Core.Extensions;
using MvxExtensions.Libraries.Portable.Core.Models;
using MvxExtensions.Libraries.Portable.Core.Services.Logger;
using MvxExtensions.Plugins.Notification;
using MvxExtensions.Samples.AllAround.Core.Models;
using MvxExtensions.Samples.AllAround.Core.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MvxExtensions.Samples.AllAround.Core.ViewModels
{
    public class DragAndDropListViewModel : SimpleMenuBaseViewModel
    {
        #region Fields

        private IDisposable _subscription;
        private IDisposable _subscriptionSelectedItems;

        #endregion

        #region Properties

        public ObservableCollection<ListItem> Items
        {
            get { return _items; }
        }
        private ObservableCollection<ListItem> _items = new ObservableCollection<ListItem>();

        public ObservableCollection<ListItem> SelectedItems
        {
            get { return _selectedItems; }
        }
        private ObservableCollection<ListItem> _selectedItems = new ObservableCollection<ListItem>();

        public bool DragEnabled
        {
            get { return _dragEnabled; }
            set
            {
                if (_dragEnabled != value)
                {
                    _dragEnabled = value;
                    RaisePropertyChanged(() => DragEnabled);
                }
            }
        }
        private bool _dragEnabled;

        public bool SelectionEnabled
        {
            get { return _selectionEnabled; }
            set
            {
                if (_selectionEnabled != value)
                {
                    _selectionEnabled = value;
                    RaisePropertyChanged(() => SelectionEnabled);
                }
            }
        }
        private bool _selectionEnabled;

        public bool SingleSelection
        {
            get { return _singleSelection; }
            set
            {
                if (_singleSelection != value)
                {
                    _singleSelection = value;
                    RaisePropertyChanged(() => SingleSelection);
                }
            }
        }
        private bool _singleSelection;

        #endregion

        #region Constructor

        public DragAndDropListViewModel(IMvxLanguageBinder textSource,
                                        IMvxJsonConverter jsonConverter,
                                        INotificationService notificationManager,
                                        ILoggerManager loggerManager)
            : base(textSource, jsonConverter, notificationManager, loggerManager)
        {
            int order = 0;
            for (int i = 0; i < 40; i++)
            {
                Items.Add(new ListItem("Item " + i, "item " + i, Color.Aqua, order++));
            }

            _subscription = Items.WeakSubscribe(OnItemsSourceCollectionChanged);
            _subscriptionSelectedItems = SelectedItems.WeakSubscribe(OnSelectedItemsSourceCollectionChanged);
        }

        #endregion

        #region Methods

        private async void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            await NotificationManager.PublishInfoNotificationAsync("Items changed (Action: {0})".SafeFormatTemplate(e.Action));
        }

        private async void OnSelectedItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            await NotificationManager.PublishInfoNotificationAsync("SelectedItems changed (Action: {0})".SafeFormatTemplate(e.Action));
        }

        #endregion
    }
}
