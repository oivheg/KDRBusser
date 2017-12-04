
using Foundation;
using UIKit;
using Xamarin.Forms;
using Plugin.Toasts;
using Firebase.Auth;
using Firebase.Core;
using UserNotifications;
using Firebase.CloudMessaging;
using System;
using Firebase.InstanceID;
using KDRBusser.iOS.FCM;


namespace KDRBusser.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IUNUserNotificationCenterDelegate, IMessagingDelegate
    {
        public void DidRefreshRegistrationToken(Messaging messaging, string fcmToken)
        {
            throw new NotImplementedException();
        }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
         
            Forms.Init();
            Firebase.Core.App.Configure();
            IOS_MyFirebaseMessagingService notif = new IOS_MyFirebaseMessagingService();

        
            DependencyService.Register<ToastNotification>(); // Register your dependency
            ToastNotification.Init();

           
            LoadApplication(new App());


           

            return base.FinishedLaunching(app, options);
        }


        // To receive notifications in foreground on iOS 10 devices.
[Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            // Do your magic to handle the notification data
            System.Console.WriteLine(notification.Request.Content.UserInfo);
        }

        [Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
        public void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            // Do your magic to handle the notification data
            System.Console.WriteLine("infor");
        }

        // Receive data message on iOS 10 devices.
        public void ApplicationReceivedRemoteMessage(RemoteMessage remoteMessage)
        {
            Console.WriteLine(remoteMessage.AppData);
        }
        public void RegisterForNotifications()
        {
            // Register your app for remote notifications.
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                // iOS 10 or later
                var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
                UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) => {
                    Console.WriteLine(granted);
                });

                // For iOS 10 display notification (sent via APNS)
                UNUserNotificationCenter.Current.Delegate = this;

                // For iOS 10 data message (sent via FCM)
                Messaging.SharedInstance.RemoteMessageDelegate = this;
            }
            else
            {
                // iOS 9 or before
                var allNotificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
                var settings = UIUserNotificationSettings.GetSettingsForTypes(allNotificationTypes, null);
                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            }

            UIApplication.SharedApplication.RegisterForRemoteNotifications();



        }

    }
}
