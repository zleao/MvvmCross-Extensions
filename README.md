#MvvmCross Extensions

Set of libraries to use with MvvmCross projects.
It contains base classes for views and viewmodels, specific platform controls, plugins and samples.
This is an ongoing project and every help/sugestion is welcome :)

##Tools Used
- **MvvmCross Nuget version: 3.5.1**
- **PCL profile used: 259**
- **Android Min. API: 16**
- **Android Support V4: 22.2.0.0**

##Releases
- **2014/04/14** First version of the utilities.
- **2015/09/22** Major updates in all projects:
	- Update on all nugets packages
	- Added async support
	- Removed strong reference events from ViewModel
	- Added new features in all modules (see description for more details)

##Future Releases
- **2016/09/06** Plannig on a full revanp, that started with the change of the repository name. I´m planning on updating the existing functionalites, and add some more. Also want to create the nuget packages for each library.

##Libraries
Includes base classes for views and view models, and some features and accelerators for more common usage in apps.

- **Portable**
	- **Attributes**
		- **DependsOnAttribute.cs**: Allows the configuration of properties and/or methods to react on `PropertyChangeEvent` of a specific property
		- **PropagateCollectionChangeAttribute.cs**: Allows to configure a property (of type `ObservableCollection`) to fire the `PropertyChangeEvent` when a its `CollectionChangedEvent` occurs
		- **StringValueAttribute.cs**: Simple attribute class for storing `string` Values
	- **Collections**
		- **ObservableDictionary.cs**: Represents a dynamic data dictionary that provides notifications when items get added, removed, or when the whole list is refreshed.
	- **Extensions** Set of methods to accelerate the usage of common functionalities
	- **LanguageBinder** Provides a override for the `MvxLanguageBinder` 
	- **Messages**
		- **OneWay**
			- **NotificationDataUpdatedMessage.cs**: Generic message to signal that the 'the data' was updated 
			- **NotificationTerminateApplicationMessage.cs**: Generic message to signal the application should be terminated (In most cases this goes against store validations, but for apps that do not go to the store, it can be usefull)
			- **NotificationUpdateMenuMessage.cs**: Used to signal that the menu should be updated 
			- **NotificationViewModelCommunicationMessage.cs**: Used as a base for notifications exchanged between viewmodels 
		- **TwoWay**
			- **NotificationQuestionCustomAnswerResult.cs**: Result of the customized question
			- **NotificationQuestionWithCustomAnswerMessage.cs**: Used to send a request to view, to show a dialog with a question and some customized answers
	- **Models**
		- **ContextOption.cs**: Class tha represents a menu option in the context of a specific viewmodel/view
		- **IExpandable.cs**: Interface used to represent classes for expandable controls (i.e. TreeView)
		- **Model.cs**: Wrapper for the `MvxNotifyPropertyChanged` with the implementation of `IDisposable`
		- **TupleKeyValue.cs**: Wrapper of the Tupple class, configured to have a key and a value
		- **ViewModelSubscriptionToken.cs**: Subscription token for viewmodels communication notifications
	- **Utilities**
		- **Color.cs**: Predefined colors
	- **ViewModels**
		- **ViewModel.cs**: Base class to be inherited by viewmodels. It contains several base implementations that allow to normalize behaviour across the app. It handles propertychanged notifications propagation (see: [PropertyChanged Event Propagation](https://github.com/zleao/MvvmCross-PropertyChangedEventPropagation)). It also gives entry points for view shown/hidden/destroyed as sugested by @slodge. It provides notification subscription and managment and also gives access to resource localization. Added also the ability to manage the menu options for each viewmodel/view context.
		- **IViewModelLifecycle.cs**: Interface used in ViewModel base class, to give entry points relative to view's lifecycle.
		- **ChildViewModel.cs**: Class inherited from ViewModel, that can be used to create viewModels that should be aware of a parent viewmodel.
		- **IViewDispatcher.cs**: Extensions of the `IMvxViewDispatcher` to provide new ovewrrides of the `ShowViewModel` method.

- **Droid**
	- **Bindings**
		- **Adapters**: Adapters available to use with the custom controls
		- **Targets**: Custom targets to provide new bindings
		- **Views**
			- **DragSorListView**: Implementation of a DragAndSort kind of list with binding capabilities
			- **TreeView**: implementation of the treeview with binding capability. Taken from an java to android implementation and adapted to work in Xamarin.Android. See sample to check binding options.
			- **BindableLinearLayout.cs**: Extension of MvxLinearLayout, providing ItemClick command binding.
			- **BindableViewPager.cs**: Derivative of the [BindableViewPager](http://slodge.blogspot.pt/2013/02/binding-to-androids-horizontal-pager.html) with some more cool bindings and corrections. See sample for binding options.
			- **CheckableLinearLayout.cs**: Linearlayout with the ICheckable interface implementation.
			- **DecimalEditText.cs**: Wrapper for EditText with the ability to show `decimal` bindings, allowing to define the number of decimal places.
			- **NumericEditText.cs**: Wrapper for EditText with the ability to show `int` bindings.
			- **SelectableListView.cs**: Extension of the MvxListView, providing multiselection. See sample for binding options.
	- **Extensions**
		- **WeakSubscriptionExtensions**: Provides weak event subscription
	- **Resources**: Contains several resources used in the controls
	- **Setup**
		- **AndroidSetup.cs**: Inherits from MvxAndroidSetup, and it registers the custom targets and controls.
	- **Views**
		- **MvxOverrides**: Overrides of some Mvx classes/services, to provide custom navigation and behaviour
		- **ActivityBase**: Base classe for activities. It provides out of the box behaviour control implementation for the common funcionalities provided in the viewmodel base class. this icludes handling of generic message subscription, and also promp messages. It provides notification managment and context options management.
		- **FragmentBase**: Base classe for fragments. Provides the same kind of funcionalities present in the `ActivityBase`.

- **WP8**: Under construction (soon)...

- **W10**: Under construction...

##Plugins
- **Device**: Provides several device information such as: Manufacturer, Model, Brand, Camera support, id and others. See sample for more information
	- **Droid**: Implemented 
	- **WP8**: Soon...
	- **W10**: Soon...

- **Logger**: It provides simple file logging mechanism. It also provides methods to log execution times. This plugins is dependant of the storage plugin to save information to files.
	- **Droid**: Implemented
	- **WP8**: Soon...
	- **W10**: Soon...

- **Notification**: Plugin inspired in the Messenger plugin of the MvvmCross framework. It provides OneWay and TwoWay message notification capabilities. Is also provides Background and UI message subscription.
Also added support for delayed notifications. Usefull to be used in the resume of views... (See samples for more info)

- **Storage**: Plugin inspired in the File plugin of MvvmCross framework. I provides writing and reading to files. It's possible to define the area where to store de file. The major upgrade of this plugin, is the ability to write/read encrypted files. It can encrypt files as one entire block, or it can add encrypted lines as needed. This second option is very usefull, for example, to create encrypted log files.
	- **Droid**: Implemented 
	- **WP8**: Soon...
	- **W10**: Soon...	
