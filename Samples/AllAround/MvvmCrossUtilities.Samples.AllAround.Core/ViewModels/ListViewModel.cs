using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Cirrious.CrossCore.UI;
using Cirrious.MvvmCross.ViewModels;
using MvvmCrossUtilities.Libraries.Portable.Utilities;
using MvvmCrossUtilities.Samples.AllAround.Core.ViewModels.Base;

namespace MvvmCrossUtilities.Samples.AllAround.Core.ViewModels
{
    public class ListViewModel : AllAroundViewModel
    {
        #region Fields

        private Random _random = new Random();

        #endregion

        #region Properties

        public ObservableCollection<Item> Items
        {
            get { return _items; }
        }
        private ObservableCollection<Item> _items = new ObservableCollection<Item>();

        public ObservableCollection<Item> SelectedItems
        {
            get { return _selectedItems; }
        }
        private ObservableCollection<Item> _selectedItems = new ObservableCollection<Item>();

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

        public ListViewModel()
        {
            _selectionModeCommand = new MvxCommand(() => SingleSelection = !SingleSelection);
            _addItemCommand = new MvxCommand(() => Items.Add(new Item("Random" + _random.Next(5000, 10000), "Rand" + _random.Next(5000, 10000), Color.Aqua)));
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
                Items.Add(new Item("Random" + i, "Rand" + i, Color.Aqua));
            }
        }

        #endregion

        #region Methods
        #endregion


        public class Item : MvxNotifyPropertyChanged
        {
            public string Name
            {
                get { return _name; }
                set
                {
                    if (_name != value)
                    {
                        _name = value;
                        RaisePropertyChanged(() => Name);
                    }
                }
            }
            private string _name;

            public string NickName
            {
                get { return _nickName; }
                set
                {
                    if (_nickName != value)
                    {
                        _nickName = value;
                        RaisePropertyChanged(() => NickName);
                    }
                }
            }
            private string _nickName;


            public MvxColor BackgroundColor
            {
                get { return _backgroundColor; }
                set
                {
                    if (_backgroundColor != value)
                    {
                        _backgroundColor = value;
                        RaisePropertyChanged(() => BackgroundColor);
                    }
                }
            }
            private MvxColor _backgroundColor;

            public Item(string name, string nickname, MvxColor backgroundColor)
            {
                Name = name;
                NickName = nickname;
                BackgroundColor = backgroundColor;
            }
        }
    }
}
