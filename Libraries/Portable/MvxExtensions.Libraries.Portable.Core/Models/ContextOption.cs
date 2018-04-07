using MvvmCross.Core.ViewModels;
using System.Windows.Input;

namespace MvxExtensions.Libraries.Portable.Core.Models
{
    /// <summary>
    /// ContextOption
    /// </summary>
    public class ContextOption : Model
    {
        #region Properties

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>    
        /// <remarks>
        /// Has to be integer because the menu items on Android have an integer ID.
        /// </remarks>
        public string Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    RaisePropertyChanged(() => Id);
                }
            }
        }
        private string _id;

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    RaisePropertyChanged(() => Text);
                }
            }
        }
        private string _text;

        /// <summary>
        /// Gets or sets the image identifier.
        /// </summary>
        /// <value>
        /// The image identifier.
        /// </value>
        public string ImageId
        {
            get { return _imageId; }
            set
            {
                if (_imageId != value)
                {
                    _imageId = value;
                    RaisePropertyChanged(() => ImageId);
                }
            }
        }
        private string _imageId;

        /// <summary>
        /// Gets or sets a value indicating whether [visible if possible].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [visible if possible]; otherwise, <c>false</c>.
        /// </value>
        public bool VisibleIfPossible
        {
            get { return _visibleIfPossible; }
            set
            {
                if (_visibleIfPossible != value)
                {
                    _visibleIfPossible = value;
                    RaisePropertyChanged(() => VisibleIfPossible);
                }
            }
        }
        private bool _visibleIfPossible;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    RaisePropertyChanged(() => IsActive);
                }
            }
        }
        private bool _isActive;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    RaisePropertyChanged(() => IsEnabled);
                }
            }
        }
        private bool _isEnabled;

        /// <summary>
        /// Gets the process command.
        /// </summary>
        /// <value>
        /// The process command.
        /// </value>
        public ICommand ProcessCommand
        {
            get { return _processCommand; }
        }
        private readonly ICommand _processCommand;
        private readonly ICommand _internalProcessCommand;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextOption"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="text">The text.</param>
        /// <param name="isActive">if set to <c>true</c> [is active].</param>
        /// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
        /// <param name="processCommand">The process command.</param>
        /// <param name="imageId">The image identifier.</param>
        /// <param name="visibleIfPossible">if set to <c>true</c> [visible if possible].</param>
        public ContextOption(string id,
                             string text, 
                             bool isActive, 
                             bool isEnabled, 
                             ICommand processCommand, 
                             string imageId = null, 
                             bool visibleIfPossible = true)
        {
            Id = id;
            Text = text;
            IsActive = isActive;
            IsEnabled = isEnabled;
            _internalProcessCommand = processCommand;
            _processCommand = new MvxCommand(OnProcessCommand);
            ImageId = imageId;
            VisibleIfPossible = visibleIfPossible;
        }

        private void OnProcessCommand()
        {
            if (_internalProcessCommand != null && _internalProcessCommand.CanExecute(Id))
                _internalProcessCommand.Execute(Id);
        }

        #endregion
    }
}
