using MvvmCrossUtilities.Libraries.Droid.Bindings.Views.DragSortListView.Interfaces;
using System;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Views.DragSortListView
{
    /// <summary>
    /// Defines the scroll speed during a drag-scroll.
    /// this default is a simple linear profile
    /// where scroll speed increases linearly as the floating View
    /// nears the top/bottom of the ListView.
    /// </summary>
    class LinearDragScrollProfile : IDragScrollProfile
    {
        private readonly AbstractDragSortListView _dslv;

        public LinearDragScrollProfile(AbstractDragSortListView dslv)
        {
            if (dslv == null)
                throw new ArgumentNullException("dslv");

            _dslv = dslv;
        }

        public float GetSpeed(float w, long t)
        {
            return _dslv.MaxScrollSpeed * w;
        }
    }
}