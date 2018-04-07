using MvxExtensions.Libraries.Portable.Core.Attributes;
using System.Reflection;

namespace MvxExtensions.Libraries.Portable.Core.Models
{
    /// <summary>
    /// Represents a dependency injected with the <see cref="DependsOnAttribute"/>
    /// </summary>
    public class DependencyInfo
    {
        #region Properties

        /// <summary>
        /// Proprett information using the <see cref="PropertyInfo"/> class
        /// </summary>
        public PropertyInfo Info
        {
            get { return _info; }
        }
        private readonly PropertyInfo _info;

        /// <summary>
        /// Indicates if the dependency is conditional, in order to be able to cancel propagations 
        /// </summary>
        public bool IsConditional
        {
            get { return _isConditional; }
        }
        private readonly bool _isConditional;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyInfo" /> class.
        /// </summary>
        /// <param name="info">The property information <see cref="PropertyInfo" /></param>
        /// <param name="isConditional">Indicates if the dependency is conditional, in order to be able to cancel propagations</param>
        public DependencyInfo(PropertyInfo info, bool isConditional = false)
        {
            _info = info;
            _isConditional = isConditional;
        }

        #endregion
    }
}
