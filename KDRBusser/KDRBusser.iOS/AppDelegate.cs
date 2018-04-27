using Foundation;
using Plugin.Toasts;
using StaffBusser.iOS.FCM;
using System;
using System.Threading.Tasks;
using UIKit;
using UserNotifications;
using Xamarin.Forms;
using Google.SignIn;
using System.Threading;
using AudioToolbox;
using System.Timers;
using StaffBusser.SharedCode;

namespace StaffBusser.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        private IOS_MyFirebaseMessagingService notif;

        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            Forms.Init();
            Firebase.Core.App.Configure();
            notif = new IOS_MyFirebaseMessagingService();

            DependencyService.Register<ToastNotification>(); // Register your dependency

            ToastNotification.Init();

            var settings = UIUserNotificationSettings.GetSettingsForTypes(
            UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, null);

            UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);

            LoadApplication(application: new App());

            // Request notification permissions from the user
            UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert, (approved, err) => {/* Handle approval*/});

            //GoogleSIngin CODE
            // You can get the GoogleService-Info.plist file at https://developers.google.com/mobile/add
            var googleServiceDictionary = NSDictionary.FromFile("GoogleService-Info.plist");
            SignIn.SharedInstance.ClientID = googleServiceDictionary["CLIENT_ID"].ToString();

            return base.FinishedLaunching(uiApplication, launchOptions);
        }

        // For iOS 9 or newer
        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            var openUrlOptions = new UIApplicationOpenUrlOptions(options);
            return SignIn.SharedInstance.HandleUrl(url, openUrlOptions.SourceApplication, openUrlOptions.Annotation);
        }

        // For iOS 8 and older
        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            return SignIn.SharedInstance.HandleUrl(url, sourceApplication, annotation);
        }

        private nint taskID = -1;

        [Export("application:didReceiveRemoteNotification:fetchCompletionHandler:")]
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            var app = UIApplication.SharedApplication;
            //This code works without using thte "notification parameter" which is awsome, as the same call can be sendt to both ios and android wuthout any device verifications inbetween.
            // this code unfortinaly only runs when app is in "foreground"
            //background might not work.  VoIP Might work ( by hacking it to fucntion as a "notification" call instead of calling for real.
            UILocalNotification notification = new UILocalNotification();
            NSDate.FromTimeIntervalSinceNow(1);
            //notification.AlertTitle = "Alert Title"; // required for Apple Watch notifications
            Task.Run(async () => await SharedHelper.InformmasterAsync().ConfigureAwait(false));
            notification.AlertAction = "View Alert";
            var body = userInfo["body"];
            var Action = userInfo["Action"];

            if (Action != null)
            {
                notification.AlertBody = "Dinner Canceled";
                timer.Stop();
                _isCanseled = true;
                UIApplication.SharedApplication.EndBackgroundTask(taskID);
            }
            else if (body != null)
            {
                Start();
                //taskID = UIApplication.SharedApplication.BeginBackgroundTask(() =>
                //{
                //    Console.WriteLine("Running out of time to complete you background task!");
                //    UIApplication.SharedApplication.EndBackgroundTask(taskID);
                //});
                //Task.Factory.StartNew(() => FinishLongRunningTask(taskID, userInfo));

                //UIApplication.SharedApplication.EndBackgroundTask(taskID);
            }
        }

        private bool _isCanseled = false;
        private CancellationTokenSource _cts;

        public async Task Start()
        {
            _cts = new CancellationTokenSource();

            taskID = UIApplication.SharedApplication.BeginBackgroundTask("LongRunningTask", OnExpiration);

            try
            {
                //INVOKE THE SHARED CODE
                Vibration();

                //_isCanseled = false;

                //while (!_isCanseled)
                //{
                //    SystemSound.Vibrate.PlayAlertSound();
                //    //if (count <= 10)
                //    //{
                //    //    SystemSound.Vibrate.PlayAlertSound();
                //    //}
                //    //else if (count == 30)
                //    //{
                //    //    count = 0;
                //    //}
                //    Thread.Sleep(500);
                //    //  count++;
                //}
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                if (_cts.IsCancellationRequested)
                {
                    // var message = new CancelledMessage();
                }
            }

            UIApplication.SharedApplication.EndBackgroundTask(taskID);
        }

        public void Stop()
        {
            _cts.Cancel();
        }

        private void OnExpiration()
        {
            _cts.Cancel();
        }

        private void FinishLongRunningTask(nint taskID, NSDictionary userInfo)
        {
            Console.WriteLine("Starting task {0}", taskID);
            Console.WriteLine("Background time remaining: {0}", UIApplication.SharedApplication.BackgroundTimeRemaining);
            // notif.CheckPayload(userInfo);
            // sleep for 15 seconds to simulate a long running task
            //Vibration();

            _isCanseled = false;

            while (!_isCanseled)
            {
                SystemSound.Vibrate.PlayAlertSound();
                //if (count <= 10)
                //{
                //    SystemSound.Vibrate.PlayAlertSound();
                //}
                //else if (count == 30)
                //{
                //    count = 0;
                //}
                Thread.Sleep(500);
                //  count++;
            }
            //Thread.Sleep(300000);

            Console.WriteLine("Task {0} finished", taskID);
            Console.WriteLine("Background time remaining: {0}", UIApplication.SharedApplication.BackgroundTimeRemaining);

            // call our end task
            //UIApplication.SharedApplication.EndBackgroundTask(taskID);
        }

        public static System.Timers.Timer timer = new System.Timers.Timer();
        private int count = 1;

        public void Vibration()
        {
            if (!timer.Enabled)
            {
                timer.Interval = 1000; // runs every second
                timer.Elapsed += Timer_Elapsed;

                timer.Start();
                System.Console.WriteLine("MYF. Vibration() Timer is started ");

                count++;
            }
        }

        public nint BackgroundTaskId { get; private set; }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("MYF. Timer_elapsed Timer is running");
            if (count <= 10)
            {
                //_isVibrating = true;
                SystemSound.Vibrate.PlayAlertSound();
            }
            switch (count)
            {
                case 20:
                    count = 0;
                    break;

                default:
                    count++;
                    break;
            }
        }

        private static void CreateNotification(String title = "")
        {
            var content = new UNMutableNotificationContent
            {
                Title = "Pleace Pickup Dinner",
                //content.Subtitle = "Notification Subtitle";
                Body = "Dinner is Waiting",
                Badge = 0,
                Sound = UNNotificationSound.Default
            };
            var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(5, true);

            var requestID = "Dinner";
            var request = UNNotificationRequest.FromIdentifier(requestID, content, trigger);

            UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) =>
            {
                if (err != null)
                {
                    // Do something with error...
                }
            });
        }

        private static void SetNotification(UILocalNotification notification, UILocalNotification localNotification, String _text)
        {
            notification.AlertBody = _text;

            UIApplication.SharedApplication.ScheduleLocalNotification(notification);
            System.Console.WriteLine(_text);
            //new UIAlertView(localNotification.AlertAction, localNotification.AlertBody, null, "OK", null).Show();

            var okCancelAlertController = UIAlertController.Create("New Notification", _text, UIAlertControllerStyle.Alert);
            UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(okCancelAlertController, true, null);
        }

        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            CreateNotification("Remote Notification");
        }

        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            // show an alert
            UIAlertController okayAlertController = UIAlertController.Create(notification.AlertAction, notification.AlertBody, UIAlertControllerStyle.Alert);
            okayAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

            //Window.RootViewController.PresentViewController(okayAlertController, true, null);
            System.Console.WriteLine("APPDEL:  Notification was swiped");
            IOS_MyFirebaseMessagingService.CancelVibration();
            // reset our badge
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }
    }
}