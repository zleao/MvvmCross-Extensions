using Android.Util;
using System.Collections.Generic;
using System.Linq;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Views.DragSortListView
{
    class HeightCache
    {
        #region Fields

        private SparseIntArray _map;
        private List<int> _order;
        private int _maxSize;

        #endregion

        #region Constructor

        public HeightCache(int size)
        {
            _map = new SparseIntArray(size);
            _order = new List<int>(size);
            _maxSize = size;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add item height at position if doesn't already exist.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="height"></param>
        public void Add(int position, int height)
        {
            int currHeight = _map.Get(position, -1);
            if (currHeight != height)
            {
                if (currHeight == -1)
                {
                    if (_map.Size() == _maxSize)
                    {
                        // remove oldest entry
                        _map.Delete(_order.ElementAt(0));
                        _order.Remove(0);
                    }
                }
                else
                {
                    // move position to newest slot
                    _order.Remove((int)position);
                }
                _map.Put(position, height);
                _order.Add(position);
            }
        }

        public int Get(int position)
        {
            return _map.Get(position, -1);
        }

        public void Clear()
        {
            _map.Clear();
            _order.Clear();
        }

        #endregion
    }
}