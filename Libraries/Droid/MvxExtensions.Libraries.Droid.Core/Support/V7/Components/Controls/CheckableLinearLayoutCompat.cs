using Android.Content;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Widget;

namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls
{
    public class CheckableLinearLayoutCompat : LinearLayoutCompat, ICheckable
    {
        static readonly int[] CHECKED_STATE_SET = { Android.Resource.Attribute.StateChecked };

        bool mChecked = false;

        public CheckableLinearLayoutCompat(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        public bool Checked
        {
            get
            {
                return mChecked;
            }
            set
            {
                if (value != mChecked)
                {
                    mChecked = value;
                    RefreshDrawableState();
                }
            }
        }

        public void Toggle()
        {
            Checked = !mChecked;
        }

        protected override int[] OnCreateDrawableState(int extraSpace)
        {
            int[] drawableState = base.OnCreateDrawableState(extraSpace + 1);

            if (Checked)
                MergeDrawableStates(drawableState, CHECKED_STATE_SET);

            return drawableState;
        }
    }
}