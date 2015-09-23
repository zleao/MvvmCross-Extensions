namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Views.DragSortListView.Interfaces
{
    /// <summary>
    /// Interface for controlling
    /// scroll speed as a function of touch position and time. Use
    /// {@link DragSortListView#setDragScrollProfile(DragScrollProfile)} to
    /// set custom profile.
    /// </summary>
    public interface IDragScrollProfile
    {
        /// <summary>
        /// Return a scroll speed in pixels/millisecond. Always return a positive number.
        /// </summary>
        /// <param name="w">
        ///     Normalized position in scroll region (i.e. w \in [0,1]).
        ///     Small w typically means slow scrolling.
        /// </param>
        /// <param name="t">Time (in milliseconds) since start of scroll (handy if you want scroll acceleration).</param>
        /// <returns>Scroll speed at position w and time t in pixels/ms.</returns>
        float GetSpeed(float w, long t);
    }
}