using AudioToolbox;
using Firebase.CloudMessaging;
using Foundation;
using KDRBusser.SharedCode;
using ObjCRuntime;
using Plugin.Toasts;
using System;
using System.Threading.Tasks;
using System.Timers;
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
            var Action = notific["Action"];
            if (title != null)
            {

                System.Console.WriteLine("MYF:DInner is ready"); //Console is not found in system
                                                                 //ToastedUserAsync("Dinner is Ready");
                                                                 //SharedHelper.ToastedUserAsync("IOS", "Dinner IOs Ready");
                Vibration();
                //Vibration();
                
                Task.Run(async () => await SharedHelper.InformmasterAsync());
            }
            else if (Action != null)
            {
                //Finds the key dictionary "action" and check if the value is "cancelVibrations" 
                var tmp = notific["Action"];
                {
                    //SharedHelper.ToastedUserAsync("Before timer stopped ", "Canceled diner");
                    if (timer.Enabled)
                    {
                        //SharedHelper.ToastedUserAsync("Timer Stopped", "Canceled diner");
                        timer.Stop();
                    }
                }

            }
           
            //var body = notific["body"];
            //RegisterForNotifications();
        }

        public static Timer timer = new Timer();


        private void Vibration()
        {
           

            if (!timer.Enabled)
            {

                timer.Interval = 500; // runs every second
                timer.Elapsed += Timer_Elapsed;
                timer.Start();
            }

        }
        private int count = 1;
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {

            if (count < 5)
            {
                count++;

            }
            else
            {
                count = 1;
                Vibrate();
            }
        }

        private void Vibrate()
        {
            SystemSound.Vibrate.PlaySystemSound();
        }

       
        

             
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

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo,  Action<UIBackgroundFetchResult> completionHandler)
        {
            base.DidReceiveRemoteNotification(application, userInfo, completionHandler);
        }
        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            if (application.ApplicationState == UIApplicationState.Active)
            {
            }
            else if (application.ApplicationState == UIApplicationState.Background)
            {
            }
            else if (application.ApplicationState == UIApplicationState.Inactive)
            {
            }
        }


    }
}
