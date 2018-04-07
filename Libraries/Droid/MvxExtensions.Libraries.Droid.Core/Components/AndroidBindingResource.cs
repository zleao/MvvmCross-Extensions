using MvvmCross.Binding;
using MvvmCross.Platform;
using MvvmCross.Platform.Droid;
using MvvmCross.Platform.Platform;
using System;

namespace MvxExtensions.Libraries.Droid.Core.Components
{
    public class AndroidBindingResource
    {
        public static readonly AndroidBindingResource Instance = new AndroidBindingResource();

        #region Properties

        public int BindableListGroupItemTemplateId { get; private set; }

        #endregion

        #region Constructor

        private AndroidBindingResource()
        {
            var setup = Mvx.Resolve<IMvxAndroidGlobals>();
            var resourceTypeName = setup.ExecutableNamespace + ".Resource";
            Type resourceType = setup.ExecutableAssembly.GetType(resourceTypeName);
            if (resourceType == null)
                throw new Exception("Unable to find resource type - " + resourceTypeName);
            try
            {
                var styleable = resourceType.GetNestedType("Styleable");

                BindableListGroupItemTemplateId = (int)SafeGetFieldValue(styleable, "MvxListView_GroupItemTemplate");
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