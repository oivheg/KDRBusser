
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
using ObjCRuntime;


namespace KDRBusser.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
 
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
         
            Forms.Init();
            Firebase.Core.App.Configure();
            IOS_MyFirebaseMessagingService notif = new IOS_MyFirebaseMessagingService();
         
        
            DependencyService.Register<ToastNotification>(); // Register your dependency
        
            ToastNotification.Init();

           
            LoadApplication(new App());

            // Request notification permissions from the user
            UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert, (approved, err) => {
                // Handle approval
            });

            

            return base.FinishedLaunching(app, options);
        }


        



    }
}
