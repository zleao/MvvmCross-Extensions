using Android.Content;
using Android.Views;
using Android.Widget;
using Cirrious.MvvmCross.Binding.Droid.Views;
using Cirrious.MvvmCross.Binding.BindingContext;
namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Views.DragSortListView
{
    /// <summary>
    /// Lightweight ViewGroup that wraps list items obtained from user's
    /// ListAdapter. ItemView expects a single child that has a definite
    /// height (i.e. the child's layout height is not MATCH_PARENT).
    /// The width of
    /// ItemView will always match the width of its child (that is,
    /// the width MeasureSpec given to ItemView is passed directly
    /// to the child, and the ItemView measured width is set to the
    /// child's measured width). The height of ItemView can be anything;
    /// The purpose of this class is to optimize slide
    /// shuffle animations.
    /// </summary>
    public class DragSortItemView : MvxListItemView
    {
        public GravityFlags Gravity
        {
            get { return _gravity; }
            set { _gravity = value; }
        }
        private GravityFlags _gravity = GravityFlags.Top;

        public DragSortItemView(Context context, IMvxLayoutInflater layoutInflater, object dataContext, int templateId)
            : base(context, layoutInflater, dataContext, templateId)
        {
            // always init with standard ListView layout params
            LayoutParameters = new AbsListView.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            View child = GetChildAt(0);

            if (child == null)
            {
                return;
            }

            if (_gravity == GravityFlags.Top)
            {
                child.Layout(0, 0, MeasuredWidth, child.MeasuredHeight);
            }
            else
            {
                child.Layout(0, MeasuredHeight - child.MeasuredHeight, MeasuredWidth, MeasuredHeight);
            }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            int height = MeasureSpec.GetSize(heightMeasureSpec);
            int width = MeasureSpec.GetSize(widthMeasureSpec);

            MeasureSpecMode heightMode = MeasureSpec.GetMode(heightMeasureSpec);

            View child = GetChildAt(0);
            if (child == null)
            {
                SetMeasuredDimension(0, width);
                return;
            }

            if (child.IsLayoutRequested)
            {
                // Always let child be as tall as it wants.
                MeasureChild(child, widthMeasureSpec, MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified));
            }

            if (heightMode == MeasureSpecMode.Unspecified)
            {
                ViewGroup.LayoutParams lp = LayoutParameters;

                if (lp.Height > 0)
                {
                    height = lp.Height;
                }
                else
                {
                    height = child.MeasuredHeight;
                }
            }

            SetMeasuredDimension(width, height);
        }

        protected override void OnDetachedFromWindow()
        {
            this.ClearAllBindings();
            base.OnDetachedFromWindow();
        }
    }
}