using Android.App;
using Android.Content;
using Android.OS;
using Firebase.Messaging;
using KDRBusser.Classes;
using KDRBusser.Communication;
using Plugin.Toasts;
using System;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;

namespace KDRBusser.Droid
{
    [Service(Name = "com.example.oivhe.resturantbusser.MyFirebaseMessagingService" ,Exported = true)]
    [BroadcastReceiver]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    class MyFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "MyFirebaseMessagingService";
            
        private int count = 1;
        static Timer timer;

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
                    Vibration();
                    Task.Run(async () =>  await  InformmasterAsync());
                }
                else if (message.Data.ContainsKey("Action")){
                    switch (message.Data["Action"])
                    {
                        case "cancelVibration":
                            ToastUser("Vibrations Canceled");
                            timer.Stop();
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

      
            void Vibration()
            {

                timer = new Timer();
                timer.Interval = 1000;
                timer.Elapsed += Timer_Elapsed;
                timer.Start();
            }

            private void Timer_Elapsed(object sender, ElapsedEventArgs e)
            {
                if (count < 10)
                {
                count++;
             
            }
            else
            {
                count = 1;
                Vibrator vibrator = (Vibrator)this.ApplicationContext.GetSystemService(Context.VibratorService);
                long[] vibPatterns = { 200, 250, 350, 250, 350, 1000, 500, 350, 500, 350, 1000 };
                vibrator.Vibrate(vibPatterns, -1);
            }
        }

            //Vibrator vibrator = (Vibrator)this.ApplicationContext.GetSystemService(Context.VibratorService);
            //long[] vibPatterns = { 200, 500, 350, 500, 350, 1000, 500, 350, 500, 350, 1000 };
            //vibrator.Vibrate(vibPatterns, -1);
   

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

        public  async Task InformmasterAsync()
        {
            User user = new User
            {
                Appid = DependencyService.Get<IFCMLoginService>().GetToken()
            };
            await RestApiCommunication.Post(user, "Msgreceived");
          
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