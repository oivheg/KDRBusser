using Foundation;
using Plugin.Toasts;
using StaffBusser.iOS.FCM;
using System;
using System.Threading.Tasks;
using UIKit;
using UserNotifications;
using Xamarin.Forms;

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

            return base.FinishedLaunching(uiApplication, launchOptions);
        }

        [Export("application:didReceiveRemoteNotification:fetchCompletionHandler:")]
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            nint taskID = -1;
            //This code works without using thte "notification parameter" which is awsome, as the same call can be sendt to both ios and android wuthout any device verifications inbetween.
            // this code unfortinaly only runs when app is in "foreground"
            //background might not work.  VoIP Might work ( by hacking it to fucntion as a "notification" call instead of calling for real.
            UILocalNotification notification = new UILocalNotification();
            NSDate.FromTimeIntervalSinceNow(1);
            //notification.AlertTitle = "Alert Title"; // required for Apple Watch notifications
            notification.AlertAction = "View Alert";
            var body = userInfo["body"];
            var Action = userInfo["Action"];

            if (Action != null)
            {
                notification.AlertBody = "Dinner Canceled";
            }
            else if (body != null)
            {
                notification.AlertBody = "Dinner is Ready";
            }

            var app = UIApplication.SharedApplication;
            UILocalNotification localNotification = userInfo[UIApplication.LaunchOptionsLocalNotificationKey] as UILocalNotification;
            if (localNotification != null)
            {
                //new UIAlertView(localNotification.AlertAction, localNotification.AlertBody, null, "OK", null).Show();
                var okCancelAlertController = UIAlertController.Create("tiitle", localNotification.AlertBody, UIAlertControllerStyle.Alert);
                UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(okCancelAlertController, true, null);
                CreateNotification();
                //notif.CreateTimedNotification();
                // reset our badge
                UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
            }

            Task.Factory.StartNew(() =>
            {
                //    // this only works fora limited time,, should restart or continue somhow.
                // this also does work while app is in background, but are not allowed to vibrate / use timer.
                taskID = app.BeginBackgroundTask(() =>
                {
                    //SetNotification(notification, localNotification, "Pleace Pick Up Dinner");
                    notif.CreateTimedNotification();
                    CreateNotification("Notification Factory");
                    app.EndBackgroundTask(taskID);
                });

                notif.CheckPayload(userInfo);
                SharedCode.SharedHelper.ToastedUserAsync("Notification");
                //FinishLongRunningTask();
                if (taskID != -1)
                {
                    SetNotification(notification, localNotification, "Backgorudn task Finishing stuff");

                    app.EndBackgroundTask(taskID);
                }
            });
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
            var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(30, true);

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