using System;
using System.Collections.Generic;
using Cirrious.MvvmCross.ViewModels;
using MvvmCrossUtilities.Libraries.Portable.Extensions;
using MvvmCrossUtilities.Libraries.Portable.Models;

namespace MvvmCrossUtilities.Samples.AllAround.Core.Models
{
    public class ExpandableItem : MvxNotifyPropertyChanged, IExpandable
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

        public IList<IExpandable> Children
        {
            get { return _children; }
            set
            {
                if (_children != value)
                {
                    _children = value;
                    RaisePropertyChanged(() => Children);
                }
            }
        }
        private IList<IExpandable> _children;

        public bool HasChildren { get; private set; }

        private Action<IExpandable> _getChildrenFunc;

        public void GetChildren()
        {
            if (Children.IsNullOrEmpty())
                _getChildrenFunc.Invoke(this);
        }

        public ExpandableItem(string name, string nickname, Action<IExpandable> getChildrenFunc, bool hasChildren)
        {
            Name = name;
            NickName = nickname;
            _getChildrenFunc = getChildrenFunc;
            HasChildren = hasChildren;
        }
    }
}
