using Android.Views;
using MvxExtensions.Libraries.Portable.Core.Messages.OneWay;
using MvxExtensions.Libraries.Portable.Core.Models;
using MvxExtensions.Libraries.Portable.Core.ViewModels.SimpleMenu;
using MvxExtensions.Libraries.Portable.Core.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Views.SimpleMenu
{
    public abstract class AppCompatSimpleMenuActivityBase<TViewModel> : AppCompatActivityBase<TViewModel>
        where TViewModel : SimpleMenuViewModel
    {
        #region Fields

        private volatile Dictionary<int, string> _contextOptionsMappings = new Dictionary<int, string>();

        #endregion

        #region Notification Management

        /// <summary>
        /// Indicates if the view should subscribe the <see cref="NotificationUpdateSimpleMenuMessage"/>.
        /// </summary>
        /// <value>
        /// <c>true</c> if view should subscribe <see cref="NotificationUpdateSimpleMenuMessage"/>; otherwise, <c>false</c>.
        /// </value>
        public virtual bool SubscribeUpdateSimpleMenuMessage { get { return true; } }

        protected override void SubscribeMessageEvents()
        {
            base.SubscribeMessageEvents();

            if (SubscribeUpdateSimpleMenuMessage)
                SubscribeEvent<NotificationUpdateSimpleMenuMessage>(OnNotificationUpdateSimpleMenuMessageAsync);
        }


        /// <summary>
        /// Called when notification update menu message.
        /// </summary>
        /// <param name="message">The object.</param>
        protected Task OnNotificationUpdateSimpleMenuMessageAsync(NotificationUpdateSimpleMenuMessage message)
        {
            InvalidateOptionsMenu();

            return Task.FromResult(true);
        }

        #endregion

        #region Simple Menu Handling

        /// <summary>
        /// Called when the options menu is created.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <returns></returns>
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            return InitializeMenuOptions(menu);
        }

        /// <summary>
        /// Prepare the Screen's standard options menu to be displayed.
        /// </summary>
        /// <param name="menu">The options menu as last shown or first initialized by
        /// onCreateOptionsMenu().</param>
        /// <returns>
        /// To be added.
        /// </returns>
        /// <since version="Added in API level 1" />
        ///   <altmember cref="M:Android.App.Activity.OnCreateOptionsMenu(Android.Views.IMenu)" />
        /// <remarks>
        ///   <para tool="javadoc-to-mdoc">Prepare the Screen's standard options menu to be displayed.  This is
        /// called right before the menu is shown, every time it is shown.  You can
        /// use this method to efficiently enable/disable items or otherwise
        /// dynamically modify the contents.
        ///   </para>
        ///   <para tool="javadoc-to-mdoc">The default implementation updates the system menu items based on the
        /// activity's state.  Deriving classes should always call through to the
        /// base class implementation.</para>
        ///   <para tool="javadoc-to-mdoc">
        ///   <format type="text/html">
        ///   <a href="http://developer.android.com/reference/android/app/Activity.html#onPrepareOptionsMenu(android.view.Menu)" target="_blank">[Android Documentation]</a>
        ///   </format>
        ///   </para>
        /// </remarks>
        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            menu.RemoveGroup(1);
            UpdateVariableMenuOptions(menu);

            return base.OnPrepareOptionsMenu(menu);
        }

        /// <summary>
        /// Initializes the menu items.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <returns></returns>
        protected virtual bool InitializeMenuOptions(IMenu menu)
        {
            UpdateVariableMenuOptions(menu);

            return true;
        }

        /// <summary>
        /// Updates the variable menu options.
        /// </summary>
        /// <param name="menu">The menu.</param>
        protected virtual void UpdateVariableMenuOptions(IMenu menu)
        {
            menu.Clear();

            _contextOptionsMappings = new Dictionary<int, string>();

            int menuItemId = 0;
            foreach (var option in TypedViewModel.ContextOptions)
            {
                if (!option.Value.IsActive || !ValidateContextOption(option.Value))
                    continue;

                AddMenuItem(menu, menuItemId, option.Value);

                _contextOptionsMappings.Add(menuItemId, option.Key);
                menuItemId++;
            }
        }

        /// <summary>
        /// Adds a menu item to a menu based on a <see cref="ContextOption"/>
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="menuItemId">The menu item identifier.</param>
        /// <param name="option">The option.</param>        
        protected virtual void AddMenuItem(IMenu menu, int menuItemId, ContextOption option)
        {
            if (option.VisibleIfPossible)
            {
                var visibleMenuItem = menu.Add(0, menuItemId, Menu.First, option.Text);
                visibleMenuItem.SetShowAsAction(ShowAsAction.IfRoom);
                visibleMenuItem.SetIcon(GetResourceIdFromImageId(option.ImageId));
                visibleMenuItem.SetEnabled(option.IsEnabled);                
            }
            else
            {                
                var hiddenMenuItem = menu.Add(1, menuItemId, Menu.First, option.Text);
                hiddenMenuItem.SetShowAsAction(ShowAsAction.Never);
                hiddenMenuItem.SetEnabled(option.IsEnabled);                                                               
            }            
        }

        /// <summary>
        /// Validates the context option.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <returns></returns>
        protected virtual bool ValidateContextOption(ContextOption option)
        {
            return true;
        }

        /// <summary>
        /// This hook is called whenever an item in your options menu is selected.
        /// </summary>
        /// <param name="item">The menu item that was selected.</param>
        /// <returns>
        /// To be added.
        /// </returns>
        /// <since version="Added in API level 1" />
        ///   <altmember cref="M:Android.App.Activity.OnCreateOptionsMenu(Android.Views.IMenu)" />
        /// <remarks>
        ///   <para tool="javadoc-to-mdoc">This hook is called whenever an item in your options menu is selected.
        /// The default implementation simply returns false to have the normal
        /// processing happen (calling the item's Runnable or sending a message to
        /// its Handler as appropriate).  You can use this method for any items
        /// for which you would like to do processing without those other
        /// facilities.
        ///   </para>
        ///   <para tool="javadoc-to-mdoc">Derived classes should call through to the base class for it to
        /// perform the default menu handling.</para>
        ///   <para tool="javadoc-to-mdoc">
        ///   <format type="text/html">
        ///   <a href="http://developer.android.com/reference/android/app/Activity.html#onOptionsItemSelected(android.view.MenuItem)" target="_blank">[Android Documentation]</a>
        ///   </format>
        ///   </para>
        /// </remarks>
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (!_contextOptionsMappings.IsNullOrEmpty() && _contextOptionsMappings.ContainsKey(item.ItemId))
            {
                var contextOptionId = _contextOptionsMappings[item.ItemId];
                TypedViewModel.ProcessContextOptionCommand.Execute(contextOptionId);
            }

            return true;
        }

        #endregion
    }
}