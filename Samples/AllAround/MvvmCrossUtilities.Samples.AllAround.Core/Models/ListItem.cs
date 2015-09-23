using Cirrious.CrossCore.UI;
using MvvmCrossUtilities.Libraries.Portable.Models;

namespace MvvmCrossUtilities.Samples.AllAround.Core.Models
{
    public class ListItem : Model
    {
        #region Properties

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

        public int Order
        {
            get { return _order; }
            set
            {
                if (_order != value)
                {
                    _order = value;
                    RaisePropertyChanged(() => Order);
                }
            }
        }
        private int _order;

        #endregion

        #region Constructor

        public ListItem(string name, string nickname, MvxColor backgroundColor, int order = 0)
        {
            Name = name;
            NickName = nickname;
            BackgroundColor = backgroundColor;
            Order = order;
        }

        #endregion
    }
}
