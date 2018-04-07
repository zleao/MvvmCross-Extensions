using Android.Content;
using Android.Util;
using Android.Views;
using MvvmCross.Binding.Droid.Views;

namespace MvxExtensions.Libraries.Droid.Core.Components.Controls
{
    public class ExpandedHeightGridView : MvxGridView
    {
        public ExpandedHeightGridView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        public ExpandedHeightGridView(Context context, IAttributeSet attrs, IMvxAdapter adapter)
            : base(context, attrs, adapter)
        {
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            // Calculate entire height by providing a very large height hint.
            // View.MEASURED_SIZE_MASK represents the largest height possible.
            int expandSpec = MeasureSpec.MakeMeasureSpec(View.MeasuredSizeMask, MeasureSpecMode.AtMost);
            base.OnMeasure(widthMeasureSpec, expandSpec);

            LayoutParameters.Height = MeasuredHeight;
        }
    }
}