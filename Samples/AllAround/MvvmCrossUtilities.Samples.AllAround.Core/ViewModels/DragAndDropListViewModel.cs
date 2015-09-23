using Cirrious.CrossCore.WeakSubscription;
using MvvmCrossUtilities.Libraries.Portable.Extensions;
using MvvmCrossUtilities.Libraries.Portable.Utilities;
using MvvmCrossUtilities.Samples.AllAround.Core.Models;
using MvvmCrossUtilities.Samples.AllAround.Core.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MvvmCrossUtilities.Samples.AllAround.Core.ViewModels
{
    public class DragAndDropListViewModel : AllAroundViewModel
    {
        #region Fields

        private IDisposable _subscription;
        private IDisposable _subscriptionSelectedItems;

        #endregion

        #region Properties

        public override string PageTitle
        {
            get { return "DragAndDrop List"; }
        }

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

        public DragAndDropListViewModel()
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
            await PublishInfoNotificationAsync("Items changed (Action: {0})".SafeFormatTemplate(e.Action));
        }

        private async void OnSelectedItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            await PublishInfoNotificationAsync("SelectedItems changed (Action: {0})".SafeFormatTemplate(e.Action));
        }

        #endregion
    }
}
