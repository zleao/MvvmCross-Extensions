using MvvmCross.Core.ViewModels;
using MvvmCross.Localization;
using MvvmCross.Platform.Platform;
using MvxExtensions.Libraries.Portable.Core.Models;
using MvxExtensions.Libraries.Portable.Core.Services.Logger;
using MvxExtensions.Plugins.Notification;
using MvxExtensions.Samples.AllAround.Core.Models;
using MvxExtensions.Samples.AllAround.Core.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MvxExtensions.Samples.AllAround.Core.ViewModels
{
    public class ListViewModel : SimpleMenuBaseViewModel
    {
        #region Fields

        private Random _random = new Random();

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

        #region Command

        public ICommand SelectionModeCommand
        {
            get { return _selectionModeCommand; }
        }
        private readonly ICommand _selectionModeCommand;

        public ICommand AddItemCommand
        {
            get { return _addItemCommand; }
        }
        private readonly ICommand _addItemCommand;

        public ICommand RemoveItemCommand
        {
            get { return _removeItemCommand; }
        }
        private readonly ICommand _removeItemCommand;

        public ICommand AddSelectedItemCommand
        {
            get { return _addSelectedItemCommand; }
        }
        private readonly ICommand _addSelectedItemCommand;

        public ICommand RemoveSelectedItemCommand
        {
            get { return _removeSelectedItemCommand; }
        }
        private readonly ICommand _removeSelectedItemCommand;

        #endregion

        #region Constructor

        public ListViewModel(IMvxLanguageBinder textSource,
                             IMvxJsonConverter jsonConverter,
                             INotificationService notificationManager,
                             ILoggerManager loggerManager)
            : base(textSource, jsonConverter, notificationManager, loggerManager)
        {
            _selectionModeCommand = new MvxCommand(() => SingleSelection = !SingleSelection);
            _addItemCommand = new MvxCommand(() => Items.Add(new ListItem("Random" + _random.Next(5000, 10000), "Rand" + _random.Next(5000, 10000), Color.Aqua)));
            _removeItemCommand = new MvxCommand(() => Items.RemoveAt(0));
            _removeSelectedItemCommand = new MvxCommand(() => SelectedItems.RemoveAt(0));
            _addSelectedItemCommand = new MvxCommand(() =>
            {
                foreach (var item in Items)
                {
                    if (!SelectedItems.Contains(item))
                    {
                        SelectedItems.Add(item);
                        break;
                    }
                }
            });

            for (int i = 0; i < 5; i++)
            {
                Items.Add(new ListItem("Random" + i, "Rand" + i, Color.Aqua));
            }
        }

        #endregion
    }
}
