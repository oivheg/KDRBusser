using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Messaging;
using Plugin.Toasts;
using Xamarin.Forms;
using KDRBusser.Communication;
using KDRBusser.Classes;

namespace KDRBusser.Droid
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    class MyFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "MyFirebaseMsgService";
        public override void OnMessageReceived(RemoteMessage message)
        {
            //ToastUser("From: " + message.From);
            var name = string.Empty; ;
            if (message.Data.Count > 0)
            {
             
            
                if (message.Data.ContainsKey("title"))
                {
                    name = message.Data["title"];
                    ToastUser("inform user of dinner is ready");
                    InformmasterAsync();
                }
                else if (message.Data.ContainsKey("Action")){
                    switch (message.Data["Action"])
                    {
                        case "cancelVibration":
                            ToastUser("should cancel vibrations");
                            break;

                        case "recieved":
                            ToastUser("Something recieved");
                            break;
                        default:
                            ToastUser("Default value");
                            break;
                    }
                }
                
            }
                {

                
            }
            
          
            //SendNotification(message.GetNotification().Body);
        }

        void SendNotification(string messageBody)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            var notificationBuilder = new Android.App.Notification.Builder(this)
                .SetSmallIcon(Resource.Drawable.abc_btn_check_material)
                .SetContentTitle("FCM Message")
                .SetContentText(messageBody)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent);

            var notificationManager = NotificationManager.FromContext(this);
            notificationManager.Notify(0, notificationBuilder.Build());
        }

        public async System.Threading.Tasks.Task InformmasterAsync()
        {
            User user = new User();
            user.Appid = DependencyService.Get<IFCMLoginService>().GetToken();
            await RestApiCommunication.post(user, "Msgreceived?");
        }
        public void ToastUser(String title)
        {
            ToastedUserAsync(title);
        }


        public async void ToastedUserAsync(String title)
        {
            var options = new NotificationOptions()
            {
                Title = title,
                Description = "KDRBusser",
                IsClickable = false // Set to true if you want the result Clicked to come back (if the user clicks it)
            };
            var notification = DependencyService.Get<IToastNotificator>();
            var result = await notification.Notify(options);

        }



    }
}