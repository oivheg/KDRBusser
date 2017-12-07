using Firebase.CloudMessaging;
using Foundation;
using KDRBusser.SharedCode;
using Plugin.Toasts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using UserNotifications;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace KDRBusser.iOS.FCM
{
    [Register("IOS_MyFirebaseMessagingService")]
    class IOS_MyFirebaseMessagingService : FormsApplicationDelegate, IMessagingDelegate, IUNUserNotificationCenterDelegate
    {

        public IOS_MyFirebaseMessagingService()
        {
            RegisterForNotifications();
        }

     
        //public IntPtr Handle => throw new NotImplementedException();
        //public event EventHandler<UserInfoEventArgs> NotificationReceived;

        public void  RegisterForNotifications(){
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


        // To receive notifications in foreground on iOS 10 devices.
        [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
        public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            // Do your magic to handle the notification data
            System.Console.WriteLine(notification.Request.Content.UserInfo);
            var notific = notification.Request.Content.UserInfo;
 var title = notific["title"];
            if (title != null)
            {

                System.Console.WriteLine("MYF:DInner is ready"); //Console is not found in system
                ToastedUserAsync("Dinner is Ready");
                //Vibration();
                Task.Run(async () => await DependencyService.Get<SharedHelper>().InformmasterAsync());
            }
           
            var body = notific["body"];
        }


        public async void ToastedUserAsync(String title)
        {
            var options = new NotificationOptions()
            {
                Title = title,
                Description = "Toasted from android",
                IsClickable = false // Set to true if you want the result Clicked to come back (if the user clicks it)
            };
            var notification = DependencyService.Get<IToastNotificator>();
            var result = await notification.Notify(options);

        }

        // Receive data message on iOS 10 devices.
        public void ApplicationReceivedRemoteMessage(RemoteMessage remoteMessage)
        {
            Console.WriteLine(remoteMessage.AppData);
        }
        public void DidRefreshRegistrationToken(Messaging messaging, string fcmToken)
        {
            //throw new NotImplementedException();
            string fmc_token = fcmToken;
        }
        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
            Messaging.SharedInstance.Disconnect();
            Console.WriteLine("Disconnected from FCM");
        }

        [Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            // If you are receiving a notification message while your app is in the background,
            // this callback will not be fired 'till the user taps on the notification launching the application.
            Messaging.SharedInstance.AppDidReceiveMessage(userInfo);

            // Do your magic to handle the notification data
            System.Console.WriteLine(userInfo);
        }
        // To receive notifications in foreground on iOS 10 devices.

       
       


        
    }
}
