using MvvmCross.Core.ViewModels;
using MvvmCross.Localization;
using MvxExtensions.Libraries.Portable.Core.Extensions;
using System;

namespace MvxExtensions.Libraries.Portable.Core.Models
{
    /// <summary>
    /// Extends from <see cref="Model"/> and adds the ability to get localized text resources (<see cref="IMvxLanguageBinder"/>)
    /// </summary>
    public abstract class LocalizableModel : MvxNotifyPropertyChanged, IDisposable
    {
        #region Properties

        /// <summary>
        /// Text source for text resources translation
        /// </summary>
        public IMvxLanguageBinder TextSource
        {
            get { return _textSource; }
        }
        private readonly IMvxLanguageBinder _textSource;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizableModel"/> class.
        /// </summary>
        /// <param name="textSource">The text source.</param>
        public LocalizableModel(IMvxLanguageBinder textSource)
        {
            _textSource = textSource.ThrowIfIoComponentIsNull(nameof(textSource));
        }

        #endregion

        #region IDisposable Members

        private bool _disposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    DisposeManagedResources();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
                DisposeUnmanagedResources();

                // Note disposing has been done.
                _disposed = true;
            }
        }

        /// <summary>
        /// Disposes the managed resources.
        /// </summary>
        protected virtual void DisposeManagedResources()
        {
        }

        /// <summary>
        /// Disposes the unmanaged resources.
        /// </summary>
        protected virtual void DisposeUnmanagedResources()
        {
        }

        #endregion
    }
}
