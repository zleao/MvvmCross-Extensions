﻿using Java.Lang;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Views.TreeView
{
    /**
     * Exception thrown when there is a problem with configuring tree.
     * 
     */
    //see: https://bugzilla.xamarin.com/show_bug.cgi?format=multiple&id=22959
    //public class TreeConfigurationException : RuntimeException
    public class TreeConfigurationException : Throwable
    {

        //private static long serialVersionUID = 1L;

        public TreeConfigurationException(string detailMessage)
            : base(detailMessage)
        {
        }
    }
}
