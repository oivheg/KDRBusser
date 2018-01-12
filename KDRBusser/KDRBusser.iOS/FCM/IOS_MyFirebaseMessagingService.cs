﻿using AudioToolbox;
using Firebase.CloudMessaging;
using Foundation;
using KDRBusser.SharedCode;
using System;
using System.Threading.Tasks;
using System.Timers;
using UIKit;
using UserNotifications;
using Xamarin.Forms.Platform.iOS;

namespace KDRBusser.iOS.FCM
{
    [Register("IOS_MyFirebaseMessagingService")]
    public class IOS_MyFirebaseMessagingService : FormsApplicationDelegate, IMessagingDelegate, IUNUserNotificationCenterDelegate
    {
        public IOS_MyFirebaseMessagingService()
        {
            RegisterForNotifications();
        }

        //public IntPtr Handle => throw new NotImplementedException();
        //public event EventHandler<UserInfoEventArgs> NotificationReceived;

        public void RegisterForNotifications()
        {
            // Register your app for remote notifications.
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                // iOS 10 or later
                var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound | UNAuthorizationOptions.None;
                UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) =>
                {
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

        private NSDictionary notific;
        private nint taskID = -1;

        // To receive notifications in foreground on iOS 10 devices.
        [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
        public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            // Do your magic to handle the notification data
            System.Console.WriteLine(notification.Request.Content.UserInfo);
            notific = notification.Request.Content.UserInfo;
            var app = UIApplication.SharedApplication;

            //Task.Run(() =>
            //{
            //    taskID = app.BeginBackgroundTask(() =>
            //    {
            //        Console.WriteLine("Bacground time expires");
            //    });

            //    //FinishLongRunningTask();
            //    if (taskID != -1)
            //    {
            //        app.EndBackgroundTask(taskID);
            //    }
            //});
            //CheckPayloadAsync(notific);
            //var body = notific["body"];
            //RegisterForNotifications
            //CheckPayload(notific);
            //runs on main or background thread
        }

        private Boolean myFlag = true;
        private Boolean tmp_bol = true;

        private void FinishLongRunningTask()
        {
        }

        private void OnExpiration()
        {
            throw new NotImplementedException();
        }

        private bool _IsVibrating = true;

        public bool IsVibrating()
        {
            return _IsVibrating;
        }

        public void CreateTimedNotification()
        {
            CreateNotification();
        }

        public void CheckPayload(NSDictionary notific)
        {
            //Action<UNNotificationPresentationOptions> completionHandler for variable into funciton
            var title = notific["title"];
            var Action = notific["Action"];
            if (title != null)
            {
                _IsVibrating = true;
                System.Console.WriteLine("MYF:DInner is ready"); //Console is not found in system

                //tests if vibrations works in background
                SystemSound.Vibrate.PlaySystemSound();

                //the original ivbrations with timer, works in foregound.

                Vibration();

                Task.Run(async () => await SharedHelper.InformmasterAsync());
                tmp_bol = true;
            }
            else if (Action != null)
            {
                //Finds the key dictionary "action" and check if the value is "cancelVibrations"
                var tmp = notific["Action"];
                {
                    //SharedHelper.ToastedUserAsync("Before timer stopped ", "Canceled diner");
                    if (timer.Enabled)
                    {
                        if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                        {
                            UNUserNotificationCenter.Current.RemoveAllPendingNotificationRequests(); // To remove all pending notifications which are not delivered yet but scheduled.
                            UNUserNotificationCenter.Current.RemoveAllDeliveredNotifications(); // To remove all delivered notifications
                        }
                        else
                        {
                            UIApplication.SharedApplication.CancelAllLocalNotifications();
                        }
                        CancelVibration();
                        _IsVibrating = false;
                    }
                }
            }
        }

        public void CancelVibration()
        {
            //SharedHelper.ToastedUserAsync("Timer Stopped", "Canceled diner");
            timer.Stop();
            tmp_bol = false;
            System.Console.WriteLine("MYF:Dinner Canceled");
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            new UIAlertView("Error registering push notifications", error.LocalizedDescription, null, "OK", null).Show();
        }

        public static System.Timers.Timer timer = new System.Timers.Timer();

        public void Vibration()
        {
            if (!timer.Enabled)
            {
                timer.Interval = 1000; // runs every second
                timer.Elapsed += Timer_Elapsed;
                timer.Start();
                System.Console.WriteLine("MYF. Vibration() Timer is started ");
            }
        }

        private int count = 1;

        public nint BackgroundTaskId { get; private set; }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (count < 5)
            {
                count++;
            }
            else
            {
                count = 1;
                CreateNotification();
                Vibrate();
                System.Console.WriteLine("MYF. Timer_elapsed Timer is running");
            }
        }

        private static void CreateNotification()
        {
            var content = new UNMutableNotificationContent
            {
                Title = "Remember Dinner",
                //content.Subtitle = "Notification Subtitle";
                Body = "Dinner is Ready",
                Badge = 0,
                Sound = UNNotificationSound.Default
            };
            var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(1, false);

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

        [Export("application:didReceiveRemoteNotification:fetchCompletionHandler:")]
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            var test = "test";
            base.DidReceiveRemoteNotification(application, userInfo, completionHandler);
        }

        // Receive data message on iOS 10 devices.

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