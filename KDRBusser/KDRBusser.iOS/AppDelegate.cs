
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

namespace KDRBusser.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IUNUserNotificationCenterDelegate, IMessagingDelegate
    {


        public event EventHandler<UserInfoEventArgs> NotificationReceived;
        public void DidRefreshRegistrationToken(Messaging messaging, string fcmToken)
        {
            throw new NotImplementedException();
        }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
         
            Forms.Init();
            Firebase.Core.App.Configure();
            DependencyService.Register<ToastNotification>(); // Register your dependency
            ToastNotification.Init();
            LoadApplication(new App());


           

            return base.FinishedLaunching(app, options);
        }

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            // If you are receiving a notification message while your app is in the background,
            // this callback will not be fired 'till the user taps on the notification launching the application.

            // If you disable method swizzling, you'll need to call this method. 
            // This lets FCM track message delivery and analytics, which is performed
            // automatically with method swizzling enabled.
            //Messaging.GetInstance ().AppDidReceiveMessage (userInfo);

            if (NotificationReceived == null)
                return;

            var e = new UserInfoEventArgs { UserInfo = userInfo };
            NotificationReceived(this, e);
        }


    }
}
