using Android.Content;
using Android.Util;
using Android.Widget;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Views
{
    public class CheckableLinearLayout : LinearLayout, ICheckable
    {
        static readonly int[] CHECKED_STATE_SET = { Android.Resource.Attribute.StateChecked };

        bool mChecked = false;

        public CheckableLinearLayout(Context context, IAttributeSet attrs)
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