namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.DragSortListView
{
    class DragSortTracker
    {
        //StringBuilder mBuilder = new StringBuilder();

        //File mFile;

        //private int mNumInBuffer = 0;
        //private int mNumFlushes = 0;

        //private boolean mTracking = false;

        public DragSortTracker()
        {
            //File root = Environment.getExternalStorageDirectory();
            //mFile = new File(root, "dslv_state.txt");

            //if (!mFile.exists())
            //{
            //    try
            //    {
            //        mFile.createNewFile();
            //        Log.d("mobeta", "file created");
            //    }
            //    catch (IOException e)
            //    {
            //        Log.w("mobeta", "Could not create dslv_state.txt");
            //        Log.d("mobeta", e.getMessage());
            //    }
            //}
        }

        public void startTracking()
        {
            //mBuilder.append("<DSLVStates>\n");
            //mNumFlushes = 0;
            //mTracking = true;
        }

        public void appendState()
        {
            //if (!mTracking) {
            //    return;
            //}

            //mBuilder.append("<DSLVState>\n");
            //final int children = getChildCount();
            //final int first = FirstVisiblePosition;
            //mBuilder.append("    <Positions>");
            //for (int i = 0; i < children; ++i) {
            //    mBuilder.append(first + i).append(",");
            //}
            //mBuilder.append("</Positions>\n");

            //mBuilder.append("    <Tops>");
            //for (int i = 0; i < children; ++i) {
            //    mBuilder.append(getChildAt(i).getTop()).append(",");
            //}
            //mBuilder.append("</Tops>\n");
            //mBuilder.append("    <Bottoms>");
            //for (int i = 0; i < children; ++i) {
            //    mBuilder.append(getChildAt(i).getBottom()).append(",");
            //}
            //mBuilder.append("</Bottoms>\n");

            //mBuilder.append("    <FirstExpPos>").append(_firstExpPos).append("</FirstExpPos>\n");
            //mBuilder.append("    <FirstExpBlankHeight>")
            //        .append(getItemHeight(_firstExpPos) - getChildHeight(_firstExpPos))
            //        .append("</FirstExpBlankHeight>\n");
            //mBuilder.append("    <SecondExpPos>").append(_secondExpPos).append("</SecondExpPos>\n");
            //mBuilder.append("    <SecondExpBlankHeight>")
            //        .append(getItemHeight(_secondExpPos) - getChildHeight(_secondExpPos))
            //        .append("</SecondExpBlankHeight>\n");
            //mBuilder.append("    <SrcPos>").append(_srcPos).append("</SrcPos>\n");
            //mBuilder.append("    <SrcHeight>").append(_floatViewHeight + DividerHeight)
            //        .append("</SrcHeight>\n");
            //mBuilder.append("    <ViewHeight>").append(getHeight()).append("</ViewHeight>\n");
            //mBuilder.append("    <LastY>").append(mLastY).append("</LastY>\n");
            //mBuilder.append("    <FloatY>").append(_floatViewMid).append("</FloatY>\n");
            //mBuilder.append("    <ShuffleEdges>");
            //for (int i = 0; i < children; ++i) {
            //    mBuilder.append(getShuffleEdge(first + i, getChildAt(i).getTop())).append(",");
            //}
            //mBuilder.append("</ShuffleEdges>\n");

            //mBuilder.append("</DSLVState>\n");
            //mNumInBuffer++;

            //if (mNumInBuffer > 1000) {
            //    flush();
            //    mNumInBuffer = 0;
            //}
        }

        public void flush()
        {
            //if (!mTracking)
            //{
            //    return;
            //}

            //// save to file on sdcard
            //try
            //{
            //    boolean append = true;
            //    if (mNumFlushes == 0)
            //    {
            //        append = false;
            //    }
            //    FileWriter writer = new FileWriter(mFile, append);

            //    writer.write(mBuilder.toString());
            //    mBuilder.delete(0, mBuilder.length());

            //    writer.flush();
            //    writer.close();

            //    mNumFlushes++;
            //}
            //catch (IOException e)
            //{
            //    // do nothing
            //}
        }

        public void stopTracking()
        {
            //if (mTracking)
            //{
            //    mBuilder.append("</DSLVStates>\n");
            //    flush();
            //    mTracking = false;
            //}
        }
    }
}
