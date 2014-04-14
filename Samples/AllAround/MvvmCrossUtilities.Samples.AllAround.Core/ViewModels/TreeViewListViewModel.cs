using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MvvmCrossUtilities.Libraries.Portable.Models;
using MvvmCrossUtilities.Samples.AllAround.Core.Models;
using MvvmCrossUtilities.Samples.AllAround.Core.ViewModels.Base;

namespace MvvmCrossUtilities.Samples.AllAround.Core.ViewModels
{
    public class TreeViewListViewModel : AllAroundViewModel
    {
        #region Fields

        private Random _random = new Random();

        #endregion

        #region Properties

        public ObservableCollection<ExpandableItem> Items
        {
            get { return _items; }
        }
        private ObservableCollection<ExpandableItem> _items = new ObservableCollection<ExpandableItem>();

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

        #endregion

        #region Constructor

        public TreeViewListViewModel()
        {
            CreateRootNodes();
        }

        #endregion

        #region Methods

        private void CreateRootNodes()
        {
            for (int i = 1; i <= 5; i++)
            {
                Items.Add(CreateNewItem("Item " + i));
            }
        }

        private ExpandableItem CreateNewItem(string nodeName)
        {
            var dotCount = 0;
            foreach (var item in nodeName)
            {
                if (item == '.')
                    dotCount++;
            }

            var hasChildren = (dotCount >= MAX_DEPTH || _random.NextDouble() < 0.5 ? false : true);
            var nickName = hasChildren ? "Group" : "Single";

            return new ExpandableItem(nodeName, nickName, GetChildren, hasChildren);
        }

        private readonly int MAX_DEPTH = 2;
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
                    childrenList.Add(CreateNewItem(nodeName + "." + i));
                }

                node.Children = childrenList;
            }
        }

        #endregion
    }
}
