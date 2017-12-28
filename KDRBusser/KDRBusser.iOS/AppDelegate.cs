using Foundation;
using KDRBusser.iOS.FCM;
using Plugin.Toasts;
using System;
using System.Threading.Tasks;
using UIKit;
using UserNotifications;
using Xamarin.Forms;

namespace KDRBusser.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        private IOS_MyFirebaseMessagingService notif;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Forms.Init();
            Firebase.Core.App.Configure();
            notif = new IOS_MyFirebaseMessagingService();

            DependencyService.Register<ToastNotification>(); // Register your dependency

            ToastNotification.Init();

            var settings = UIUserNotificationSettings.GetSettingsForTypes(
   UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound
   , null);
            UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            LoadApplication(new App());

            // Request notification permissions from the user
            UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert, (approved, err) =>
            {
                // Handle approval
            });

            return base.FinishedLaunching(app, options);
        }

        [Export("application:didReceiveRemoteNotification:fetchCompletionHandler:")]
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            nint taskID = -1;
            //This code works without using thte "notification parameter" which is awsome, as the same call can be sendt to both ios and android wuthout any device verifications inbetween.
            // this code unfortinaly only runs when app is in "foreground"
            //background might not work.  VoIP Might work ( by hacking it to fucntion as a "notification" call instead of calling for real.
            UILocalNotification notification = new UILocalNotification();
            NSDate.FromTimeIntervalSinceNow(15);
            //notification.AlertTitle = "Alert Title"; // required for Apple Watch notifications
            notification.AlertAction = "View Alert";
            notification.AlertBody = "Your 15 second alert has fired!";
            UIApplication.SharedApplication.ScheduleLocalNotification(notification);

            var app = UIApplication.SharedApplication;

            Task.Run(() =>
            {
                // this only works fora limited time,, should restart or continue somhow.
                // this also does work while app is in background, but are not allowed to vibrate / use timer.
                taskID = app.BeginBackgroundTask(() =>
                {
                    System.Console.WriteLine("Bacground time expires");
                });

                notif.CheckPayload(userInfo);

                //FinishLongRunningTask();
                if (taskID != -1)
                {
                    app.EndBackgroundTask(taskID);
                }
            });
        }

        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            // show an alert
            UIAlertController okayAlertController = UIAlertController.Create(notification.AlertAction, notification.AlertBody, UIAlertControllerStyle.Alert);
            okayAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

            Window.RootViewController.PresentViewController(okayAlertController, true, null);

            // reset our badge
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }
    }
}