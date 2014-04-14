#MvvmCross Utilities

Set of libraries to use with MvvmCross projects.
It contains base classes for views and viewmodels, specific platform controls, plugins and samples.
This is an ongoing project and and every help/sugestion is welcome.


**MvvmCross version: 3.1.1**

**PCL profile used: 78**

##Releases
- **2014/04/14**
	First version of the utitlities. Working on implementation for WP8

##Libraries
Includes base classes for views and view models, and some features and accelerators for more common usage in apps.
- **Portable**
	- **Attributes**
		- **DependsOnAttribute.cs**: Attribute used to allow configuration of properties and/or methods to react on propertychange of a specific property
	- **Collections**
		- **ObservableDictionary.cs**: Represents a dynamic data dictionary that provides notifications when items get added, removed, or when the whole list is refreshed.
	- **Extensions** Set of methods to accelerate the usage of common funcionalities
		- AssemblyExtensions.cs
		- CollectionExtensions.cs
		- DateTimeExtensions.cs
		- DictionaryExtensions.cs
		- ExceptionExtensions.cs
		- IEnumerabeExtensions.cs
		- KeyValuePairExtensions.cs
		- ListExtensions.cs
		- ObjectExtensions.cs
		- StringExtensions.cs
		- TypeExtensions.cs
	- **Messages**
		- **NotificationUpdateMenuMessage.cs**: Used to signal the view that the menu should be updated 
		- **TwoWay**
			- **NotificationQuestionWithCustomAnswerMessage.cs**: Used to send a request to view, to show a dialog with a question and some customized answers
			- **NotificationQuestionCustomAnswerResult.cs**: Result of the customized question
	- **Models**
		- **IExpandable.cs**: Interface used to represent classes for expandable controls (i.e. TreeView)
	- **Utilities**
		- **Color.cs**: Predefined colors
	- **ViewModels**
		- **ViewModel.cs**: Base class to be inherited by viewmodels. It contains several base implementations that allow to normalize behaviour across the app. It handles propertychanged notifications propagation (see: [PropertyChanged Event Propagation](https://github.com/zleao/MvvmCross-PropertyChangedEventPropagation)). It also gives entry points for view shown/hidden/destroyed as sugested by @slodge. It provides notification subscription and managment and also gives access to resource localization.
		- **IViewModelLifecycle.cs**: Interface used in ViewModel base class, to give entry points relative to view's lifecycle.
		- **ChildViewModel.cs**: Class inherited from ViewModel, that can be used to create viewModels that should be aware of a parent viewmodel.

- **Droid**
	- **Bindings**
		- **Adapters**
			- **TreeView**:
			- **BindableLinearLayoutAdapter.cs**:
			- **MvxBindablePagerAdapter.cs**
			- **SelectableListViewAdapter.cs**
		- **Targets**
			- **BindableViewPagerCurrentPageIndexTargetBinding.cs**
			- **EditTextLostFocusCommandTargetBinding.cs**
			- **EditTextSingleLineTargetBinding.cs**
			- **TextViewIsValidTargetBinding.cs**
			- **TextViewTextLabelTargetBinding.cs**
			- **ToggleButtonTextLabelOffTargetBinding.cs**
			- **ToggleButtonTextLabelOnTargetBinding.cs**
			- **ViewIsFocusedTargetBinding.cs**
		- **Views**
			- **TreeView**: implementation of the treeview with binding capability. Taken from an java to android implementation and adapted to work in Xamarin.Android. See sample to check binding options.
			- **BindableGalery.cs**: photo galery with itemssource binding.
			- **BindableLinearLayout.cs**: Extension of MvxLinearLayout, providing ItemClick command binding.
			- **BindableViewPager.cs**: Derivative of the [BindableViewPager](http://slodge.blogspot.pt/2013/02/binding-to-androids-horizontal-pager.html) with some more cool bindings and corrections. See sample for binding options.
			- **CheckableLinearLayout.cs**: Linearlayout with the ICheckable interface implementation.
			- **SelectableListView.cs**: Extension of the MvxListView, providing multiselection. See sample for binding options.
	- **Resources**: Contains several resources used in the controls
	- **Setup**
		- **AndroidSetup.cs**: Inherits from MvxAndroidSetup, and it registers the custom targets and controls.
	- **Views**
		- **FragmentActivity**: Base classe for activities. It provides out of the box behaviour control implementation for the common funcionalities provided in the viewmodel base class. this icludes handling of generic message subscription, and also promp messages. It provides notification managment.
		- **Extensions**: Extension methods for activities.
		- **ViewsContainer**: used to define some view behaviour. In this particular case, is used to define the ClearTop flag for new intents.

- **WP8**: Under construction (soon)...

- **Touch**: Not on the imediate plans, but is something to work on...

##Plugins
- **Device**: Provides several device information such as: Manufacturer, Model, Brand, Camera support, id and others. See sample for more information
	- **Droid**: Implemented 
	- **WP8 **: Soon...
	- **Touch** Not so soon...

- **Logger**: It provides simple file logging mechanism. It also provides methods to log execution times. This plugins is dependant of the storage plugin to save information to files.
 	- **Droid**: Implemented 
	- **WP8 **: Soon...
	- **Touch** Not so soon...

- **Notification**: Plugin inspired in the Messenger plugin of the MvvmCross framework. It provides OneWay and TwoWay message notification capabilities. Is also provides Background and UI message subscription.

- **Rest**: allows to consume Restfull webservices. Uses RestSharp. (Available in WP8, Droid and Touch)
	- **Droid**: Implemented 
	- **WP8 **: Implemented
	- **Touch** Implemented 

- **Storage**: Plugin inspired in the File plugin of MvvmCross framework. I provides writing and reading to files. It's possible to define the area where to store de file. The major upgrade of this plugin, is the ability to write/read encrypted files. It can encrypt files as one entire block, or it can add encrypted lines as needed. This second option is very usefull, for example, to create encrypted log files.
 	- **Droid**: Implemented 
	- **WP8 **: Soon...
	- **Touch** Not so soon...	
