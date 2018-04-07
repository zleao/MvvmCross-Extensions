using MvvmCross.Core.ViewModels;
using MvvmCross.Localization;
using MvvmCross.Platform;
using MvvmCross.Platform.Platform;
using MvvmCross.Platform.WeakSubscription;
using MvxExtensions.Libraries.Portable.Core.Attributes;
using MvxExtensions.Libraries.Portable.Core.Extensions;
using MvxExtensions.Libraries.Portable.Core.Models;
using MvxExtensions.Libraries.Portable.Core.Services.Logger;
using MvxExtensions.Plugins.Notification;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace MvxExtensions.Libraries.Portable.Core.ViewModels
{
    /// <summary>
    /// Lightweight version of the <see cref="ViewModel"/>. 
    /// Inherits directly from <see cref="MvxNotifyPropertyChanged"/>, 
    /// ignoring all the lifecyle <see cref="MvxViewModel"/> and navigation <see cref="MvxNavigatingObject"/> capabilities provided by MvvmCross
    /// </summary>
    public abstract class LightViewModel : MvxNotifyPropertyChanged, IDisposable
    {
        #region Fields

        private volatile Dictionary<string, int> _dependsOnConditionalCount = new Dictionary<string, int>();

        private MvxNotifyPropertyChangedEventSubscription _propertyChangedSubscription = null;
        private volatile Dictionary<string, MvxNotifyCollectionChangedEventSubscription> _notifiableCollectionsChangedSubscription = new Dictionary<string, MvxNotifyCollectionChangedEventSubscription>();

        #endregion

        #region Properties

        /// <summary>
        /// Text source for text resources translation
        /// </summary>
        public IMvxLanguageBinder TextSource
        {
            get { return _textSource; }
        }
        private readonly IMvxLanguageBinder _textSource;

        /// <summary>
        /// Plugin for json manipulation
        /// </summary>
        protected IMvxJsonConverter JsonConverter
        {
            get { return _jsonConverter; }
        }
        private readonly IMvxJsonConverter _jsonConverter;

        /// <summary>
        /// Plugin used for notifications propagation.
        /// </summary>
        protected INotificationService NotificationManager
        {
            get { return _notificationManager; }
        }
        private readonly INotificationService _notificationManager;

        /// <summary>
        /// Manager for logs
        /// </summary>
        public abstract ILoggerManager LoggerManager { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LightViewModel" /> class.
        /// </summary>
        /// <param name="textSource">The text source.</param>
        /// <param name="jsonConverter">The json converter.</param>
        /// <param name="notificationManager">The notification manager.</param>
        /// <exception cref="System.NullReferenceException">IMvxJsonConverter
        /// or
        /// INotificationService
        /// or
        /// IMvxLanguageBinder</exception>
        internal LightViewModel(IMvxLanguageBinder textSource,
                                IMvxJsonConverter jsonConverter,
                                INotificationService notificationManager)
        {
            _textSource = textSource.ThrowIfIoComponentIsNull(nameof(textSource));
            _jsonConverter = jsonConverter.ThrowIfIoComponentIsNull(nameof(jsonConverter));
            _notificationManager = notificationManager.ThrowIfIoComponentIsNull(nameof(notificationManager));

            InitializePropertyDependencies(GetType());
            InitializeMethodDependencies(GetType());

            InitializePropertyChanged();
        }

        #endregion

        #region Dependency Management

        /// <summary>
        /// List of all the properties that have the DependsOn attribute configured
        /// </summary>
        private readonly Dictionary<string, IList<DependencyInfo>> _propertyDependencies = new Dictionary<string, IList<DependencyInfo>>();

        /// <summary>
        /// List of all the notifiable collections properties that have the PropagateCollectionChange attribute configured
        /// </summary>
        private readonly Dictionary<string, INotifyCollectionChanged> _notifiableCollectionsPropertyDependencies = new Dictionary<string, INotifyCollectionChanged>();

        /// <summary>
        /// List of all the methods that have the DependsOn attribute configured
        /// </summary>
        private readonly Dictionary<string, IList<MethodInfo>> _methodDependencies = new Dictionary<string, IList<MethodInfo>>();

        /// <summary>
        /// Gets a value indicating whether this instance should react to property changed events
        /// </summary>
        protected bool HasDependencies
        {
            get
            {
                return _propertyDependencies.Count > 0 || _notifiableCollectionsPropertyDependencies.Count > 0 || _methodDependencies.Count > 0;
            }
        }


        /// <summary>
        /// Maps all the properties that have de DependsOn and/or the PropagateCollectionChange attributes configured
        /// </summary>
        private void InitializePropertyDependencies(Type type)
        {
            foreach (var property in type.GetProperties(true))
            {
                var attributes = property.GetCustomAttributes<DependsOnAttribute>(true);
                if (attributes.SafeCount() > 0)
                {
                    lock (_propertyDependencies)
                    {
                        foreach (var attribute in attributes)
                        {
                            if (!_propertyDependencies.ContainsKey(attribute.Name))
                                _propertyDependencies.Add(attribute.Name, new List<DependencyInfo>());
                            _propertyDependencies[attribute.Name].Add(new DependencyInfo(property, attribute.IsConditional));
                        }
                    }
                }

                if (typeof(INotifyCollectionChanged).IsAssignableFrom(property.PropertyType))
                {
                    var attribute = property.GetCustomAttribute<PropagateCollectionChangeAttribute>(true);
                    if (attribute != null)
                    {
                        lock (_notifiableCollectionsPropertyDependencies)
                        {
                            var collection = property.GetValue(this, null) as INotifyCollectionChanged;
                            _notifiableCollectionsPropertyDependencies.Add(property.Name, collection);
                        }
                        lock (_notifiableCollectionsChangedSubscription)
                        {
                            _notifiableCollectionsChangedSubscription.Add(property.Name, null);
                        }
                    }
                }
            }

            if (type != typeof(ViewModel))
            {
                var newType = type.GetTypeInfo().BaseType;
                if (newType != null)
                    InitializePropertyDependencies(newType);
            }
        }

        /// <summary>
        /// Maps all the methods that have de DependsOn attribute configured
        /// </summary>
        private void InitializeMethodDependencies(Type type)
        {
            //foreach (var method in type.GetTypeInfo().DeclaredMethods.Where(m => m.ReturnType.Equals(typeof(void)) && m.GetParameters().Length == 0))
            foreach (var method in type.GetTypeInfo().DeclaredMethods.Where(m => m.GetParameters().Length == 0))
            {
                var attributes = method.GetCustomAttributes<DependsOnAttribute>(true);
                if (attributes.SafeCount() > 0)
                {
                    foreach (var attribute in attributes)
                    {
                        if (!_methodDependencies.ContainsKey(attribute.Name))
                            _methodDependencies.Add(attribute.Name, new List<MethodInfo>());
                        _methodDependencies[attribute.Name].Add(method);
                    }
                }
            }

            if (type != typeof(ViewModel))
            {
                var newType = type.GetTypeInfo().BaseType;
                if (newType != null)
                    InitializeMethodDependencies(newType);
            }
        }

        /// <summary>
        /// Initializes the listeners for property changed events
        /// </summary>
        private void InitializePropertyChanged()
        {
            if (HasDependencies)
            {
                _propertyChangedSubscription = (this as INotifyPropertyChanged).WeakSubscribe(OnPropertyChanged);

                foreach (var item in _notifiableCollectionsPropertyDependencies)
                {
                    if (item.Value != null)
                        _notifiableCollectionsChangedSubscription[item.Key] = item.Value.WeakSubscribe(OnCollectionChanged);
                }
            }
        }

        /// <summary>
        /// Called when a property raises the <see cref="PropertyChangedEventHandler"/>
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e == null)
                return;

            UpdateCollectionPropertyValue(e.PropertyName);
            RaiseDependenciesPropertyChanged(e.PropertyName);
        }

        /// <summary>
        /// Updates the collection property dependency subscription.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void UpdateCollectionPropertyValue(string propertyName)
        {
            INotifyCollectionChanged collection;
            if (_notifiableCollectionsPropertyDependencies.TryGetValue(propertyName, out collection))
            {
                var senderCollection = this.SafeGetPropertyValue(propertyName) as INotifyCollectionChanged;
                if (!object.ReferenceEquals(collection, senderCollection))
                {
                    //Remove previous subscription
                    if (_notifiableCollectionsChangedSubscription[propertyName] != null)
                    {
                        _notifiableCollectionsChangedSubscription[propertyName].Dispose();
                        _notifiableCollectionsChangedSubscription[propertyName] = null;
                    }

                    //Add new subscription
                    if (senderCollection != null)
                        _notifiableCollectionsChangedSubscription[propertyName] = senderCollection.WeakSubscribe(OnCollectionChanged);

                    _notifiableCollectionsPropertyDependencies[propertyName] = senderCollection;
                }
            }
        }

        /// <summary>
        /// Handles the collection changed events for the notifiable collections marked with the PropagateCollectionChange attribute
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var collection in _notifiableCollectionsPropertyDependencies.Where(nc => ReferenceEquals(sender, nc.Value)).OfType<KeyValuePair<string, INotifyCollectionChanged>?>())
            {
                RaiseDependenciesPropertyChanged(collection.Value.Key);
            }
        }

        /// <summary>
        /// Raises the dependencies property changed.
        /// </summary>
        /// <param name="dependencyName">Name of the dependency.</param>
        public void RaiseDependenciesPropertyChanged(string dependencyName)
        {
            //Prevents the conditional DependsOn from firing, if the execution was made
            //to prevent propagation (ExecuteWithoutConditionalDependsOn)
            if (_dependsOnConditionalCount.ContainsKey(dependencyName) &&
                _dependsOnConditionalCount[dependencyName] > 0)
            {
                return;
            }

            lock (_propertyDependencies)
            {
                IList<DependencyInfo> properties;
                if (_propertyDependencies.TryGetValue(dependencyName, out properties))
                {
                    foreach (var property in properties)
                    {
                        if (!typeof(ICommand).IsAssignableFrom(property.Info.PropertyType))
                        {
                            RaisePropertyChanged(property.Info.Name);
                        }
                        else
                        {
                            var command = property.Info.GetValue(this, null) as MvxCommand;
                            if (command != null)
                                command.RaiseCanExecuteChanged();
                        }
                    }
                }
            }

            lock (_methodDependencies)
            {
                IList<MethodInfo> methods;
                if (_methodDependencies.TryGetValue(dependencyName, out methods))
                {
                    foreach (var method in methods)
                    {
                        method.Invoke(this, null);
                    }
                }
            }
        }

        /// <summary>
        /// Removes the property changed handlers.
        /// </summary>
        private void RemovePropertyChangedHandlers()
        {
            if (_propertyChangedSubscription != null)
            {
                _propertyChangedSubscription.Dispose();
                _propertyChangedSubscription = null;
            }
        }

        /// <summary>
        /// Removes the collection changed handlers.
        /// </summary>
        private void RemoveCollectionChangedHandlers()
        {
            foreach (var item in _notifiableCollectionsChangedSubscription)
            {
                if (item.Value == null)
                    continue;
                try
                {
                    item.Value.Dispose();
                }
                catch (InvalidOperationException)
                {
                    // This error might occur during dispose.
                }
            }
            _notifiableCollectionsChangedSubscription.Clear();
        }

        /// <summary>
        /// Executes the action, preventing the propagation of DependsOn
        /// that are marked with the 'IsConditional' flag for the specified property
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="action">The action.</param>
        protected void ExecuteWithoutConditionalDependsOn(string propertyName, Action action)
        {
            try
            {
                if (!_dependsOnConditionalCount.ContainsKey(propertyName))
                    _dependsOnConditionalCount.Add(propertyName, 1);
                else
                    _dependsOnConditionalCount[propertyName]++;

                action.Invoke();

                _dependsOnConditionalCount[propertyName] = Math.Max(0, _dependsOnConditionalCount[propertyName] - 1);
            }
            catch
            {
                _dependsOnConditionalCount.Remove(propertyName);
            }
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
            RemovePropertyChangedHandlers();
            RemoveCollectionChangedHandlers();
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