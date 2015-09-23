using System;
using System.Collections.Generic;
using Cirrious.MvvmCross.ViewModels;
using MvvmCrossUtilities.Libraries.Portable.Extensions;
using MvvmCrossUtilities.Libraries.Portable.Models;
using Cirrious.CrossCore.UI;

namespace MvvmCrossUtilities.Samples.AllAround.Core.Models
{
    public class ExpandableItem : Model, IExpandable
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

        public int Level { get; private set; }

        private Action<IExpandable> _getChildrenFunc;

        public void GetChildren()
        {
            if (_getChildrenFunc != null && Children.IsNullOrEmpty())
                _getChildrenFunc.Invoke(this);
        }

        public ExpandableItem(string name, string nickname, MvxColor backgroundColor, Action<IExpandable> getChildrenFunc, bool hasChildren, int level, bool getChildrenInConstructor = false)
        {
            Name = name;
            NickName = nickname;
            BackgroundColor = backgroundColor;
            _getChildrenFunc = getChildrenFunc;
            HasChildren = hasChildren;
            Level = level;

            if (getChildrenInConstructor)
                GetChildren();
        }
    }
}
