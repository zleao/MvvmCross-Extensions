using Android.App;
using Android.OS;
using Android.Widget;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Droid.Views;
using Cirrious.MvvmCross.Plugins.File;
using NLog;
using NLog.Config;
using NLog.Targets;
using System.IO;

namespace MvvmCrossUtilities.Samples.AllAround.Droid.Views
{
    [Activity(Label = "View for FirstViewModel")]
    public class FirstView : MvxActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.FirstView);

            Button buttonNLog = FindViewById<Button>(Resource.Id.buttonNLog);
            buttonNLog.Click += buttonNLog_Click;

        }

       

        void buttonNLog_Click(object sender, System.EventArgs e)
        {
            // Step 1. Create configuration object 
            LoggingConfiguration config = new LoggingConfiguration();

            // Step 2. Create targets and add them to the configuration 
            //MessageBoxTarget mbtarget = new MessageBoxTarget();
            //config.AddTarget("messageBox", mbtarget);

            FileTarget fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            // Step 3. Set target properties 
            //mbtarget.Layout = "${message}";
            fileTarget.FileName = Path.Combine(Mvx.Resolve<IMvxFileStore>().NativePath(string.Empty), "file.txt");
            fileTarget.Layout = "${message}";

            // Step 4. Define rules
            //LoggingRule rule1 = new LoggingRule("*", LogLevel.Debug, mbtarget);
            //config.LoggingRules.Add(rule1);

            LoggingRule rule2 = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule2);

            // Step 5. Activate the configuration
            LogManager.Configuration = config;

            Logger logger = LogManager.GetCurrentClassLogger();

            //logger.Trace("Sample trace message");
            logger.Debug("Sample debug message");
            //logger.Info("Sample informational message");
            //logger.Warn("Sample warning message");
            //logger.Error("Sample error message");
            //logger.Fatal("Sample fatal error message");

            // alternatively you can call the Log() method 
            // and pass log level as the parameter.
            //logger.Log(LogLevel.Info, "Sample fatal error message");
        }
    }
}