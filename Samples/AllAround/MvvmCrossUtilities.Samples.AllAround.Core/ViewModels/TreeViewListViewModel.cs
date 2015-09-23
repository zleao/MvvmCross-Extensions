using Cirrious.MvvmCross.ViewModels;
using MvvmCrossUtilities.Libraries.Portable.Models;
using MvvmCrossUtilities.Libraries.Portable.Utilities;
using MvvmCrossUtilities.Samples.AllAround.Core.Models;
using MvvmCrossUtilities.Samples.AllAround.Core.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MvvmCrossUtilities.Samples.AllAround.Core.ViewModels
{
    public class TreeViewListViewModel : AllAroundViewModel
    {
        #region Constants

        private const int MAX_DEPTH = 4;

        #endregion

        #region Fields

        private Random _random = new Random();

        #endregion

        #region Properties

        public ObservableCollection<ExpandableItem> Items
        {
            get { return _items; }
            set
            {
                if (_items != value)
                {
                    _items = value;
                    RaisePropertyChanged(() => Items);
                }
            }
        }
        private ObservableCollection<ExpandableItem> _items;

        public ObservableCollection<ExpandableItem> SelectedItems
        {
            get { return _selectedItems; }
        }
        private ObservableCollection<ExpandableItem> _selectedItems = new ObservableCollection<ExpandableItem>();

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
        private bool _singleSelection = true;

        public bool PropagateSelection
        {
            get { return _propagateSelection; }
            set
            {
                if (_propagateSelection != value)
                {
                    _propagateSelection = value;
                    RaisePropertyChanged(() => PropagateSelection);
                }
            }
        }
        private bool _propagateSelection = true;

        public bool ExpandChildWhenSelected
        {
            get { return _expandChildWhenSelected; }
            set
            {
                if (_expandChildWhenSelected != value)
                {
                    _expandChildWhenSelected = value;
                    RaisePropertyChanged(() => ExpandChildWhenSelected);
                }
            }
        }
        private bool _expandChildWhenSelected = true;

        public bool AllowParentSelection
        {
            get { return _allowParentSelection; }
            set
            {
                if (_allowParentSelection != value)
                {
                    _allowParentSelection = value;
                    RaisePropertyChanged(() => AllowParentSelection);
                }
            }
        }
        private bool _allowParentSelection = false;

        public int NumberOfItems
        {
            get { return _numberOfItems; }
            set
            {
                if (_numberOfItems != value)
                {
                    _numberOfItems = value;
                    RaisePropertyChanged(() => NumberOfItems);
                }
            }
        }
        private int _numberOfItems;

        #endregion

        #region Commands

        public ICommand UpdateTreeViewCommand
        {
            get { return _updateTreeViewCommand; }
        }
        private readonly ICommand _updateTreeViewCommand;

        #endregion

        #region Constructor

        public TreeViewListViewModel()
        {
            _updateTreeViewCommand = new MvxCommand(UpdateTreeView);

            NumberOfItems = 5;
        }

        #endregion

        #region Methods

        private Task CreateRootNodesAsync()
        {
            var items = new List<ExpandableItem>();

            for (int i = 1; i <= NumberOfItems; i++)
            {
                items.Add(CreateNewItem("Item " + i, 0));
            }

            InvokeOnMainThread(() =>
            {
                Items = new ObservableCollection<ExpandableItem>(items);
            });

            return Task.FromResult(true);
        }

        private ExpandableItem CreateNewItem(string nodeName, int level)
        {
            var hasChildren = level < MAX_DEPTH ? (_random.NextDouble() < 0.5 ? false : true) : false;
            var nickName = hasChildren ? "Group" : "Single";

            return new ExpandableItem(nodeName, nickName, Color.Aqua, GetChildren, hasChildren, level);
        }

        private void GetChildren(IExpandable node)
        {
            if (node != null && node.HasChildren)
            {
                var ei = node as ExpandableItem;
                var nodeName = ei != null ? ei.Name : string.Empty;

                var numberOfChildren = _random.Next(1, 5);

                var childrenList = new List<IExpandable>();

                for (int i = 1; i <= numberOfChildren; i++)
                {
                    childrenList.Add(CreateNewItem(nodeName + "." + i, node.Level + 1));
                }

                node.Children = childrenList;
            }
        }

        private void UpdateTreeView()
        {
            DoWorkAsync(CreateRootNodesAsync, "Processing...");
        }

        #endregion
    }
}
