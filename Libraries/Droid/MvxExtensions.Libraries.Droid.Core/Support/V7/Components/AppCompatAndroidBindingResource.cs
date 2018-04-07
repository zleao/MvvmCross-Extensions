using MvvmCross.Binding;
using MvvmCross.Platform;
using MvvmCross.Platform.Droid;
using MvvmCross.Platform.Platform;
using System;

namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components
{
    public class AppCompatAndroidBindingResource
    {
        public static readonly AppCompatAndroidBindingResource Instance = new AppCompatAndroidBindingResource();

        #region Properties
        public int[] ExpandableRecyclerViewStylableGroupId { get; private set; }

        public int ExpandableRecyclerViewChildItemTemplateId { get; private set; }

        #endregion

        #region Constructor

        private AppCompatAndroidBindingResource()
        {
            var setup = Mvx.Resolve<IMvxAndroidGlobals>();
            var resourceTypeName = setup.ExecutableNamespace + ".Resource";
            Type resourceType = setup.ExecutableAssembly.GetType(resourceTypeName);
            if (resourceType == null)
                throw new Exception("Unable to find resource type - " + resourceTypeName);
            try
            {
                var styleable = resourceType.GetNestedType("Styleable");

                ExpandableRecyclerViewStylableGroupId = (int[])SafeGetFieldValue(styleable, "ExpandableRecyclerView");

                ExpandableRecyclerViewChildItemTemplateId = (int)SafeGetFieldValue(styleable, "ExpandableRecyclerView_ChildItemTemplate");
            }
            catch (Exception)
            {
                throw new Exception(
                        "Error finding resource ids for MvvmCross.DeapBinding - please make sure ResourcesToCopy are linked into the executable");
            }
        }

        #endregion

        #region Methods

        private static object SafeGetFieldValue(Type styleable, string fieldName)
        {
            return SafeGetFieldValue(styleable, fieldName, 0);
        }

        private static object SafeGetFieldValue(Type styleable, string fieldName, object defaultValue)
        {
            var field = styleable.GetField(fieldName);
            if (field == null)
            {
                MvxBindingTrace.Trace(MvxTraceLevel.Error, "Missing stylable field {0}", fieldName);
                return defaultValue;
            }

            return field.GetValue(null);
        }

        #endregion
    }

}