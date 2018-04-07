using MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.DragSortListView.Interfaces;
using System;

namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.DragSortListView
{
    /// <summary>
    /// Defines the scroll speed during a drag-scroll.
    /// this default is a simple linear profile
    /// where scroll speed increases linearly as the floating View
    /// nears the top/bottom of the ListView.
    /// </summary>
    class AppCompatLinearDragScrollProfile : IDragScrollProfile
    {
        private readonly AppCompatBaseDragSortListView _dslv;

        public AppCompatLinearDragScrollProfile(AppCompatBaseDragSortListView dslv)
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