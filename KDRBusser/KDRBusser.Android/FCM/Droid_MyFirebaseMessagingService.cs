using Android.App;
using Android.Content;
using Android.OS;
using Firebase.Messaging;
using KDRBusser.Classes;
using KDRBusser.Communication;
using KDRBusser.Droid.HelperClass;
using KDRBusser.Droid.Receivers;
using Plugin.Toasts;
using System;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;

namespace KDRBusser.Droid
{
    [Service]

    //[BroadcastReceiver]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class Droid_MyFirebaseMessagingService : FirebaseMessagingService
    {
        private const string TAG = "Droid_MyFirebaseMessagingService";
        public static Droid_MyFirebaseMessagingService Instance;
        private int count = 1;
        public static Timer timer = new Timer();

        public override void OnCreate()
        {
            Instance = this;
            base.OnCreate();
            System.Console.WriteLine("MYF:test"); //Console is not found in system
            deleteReceiver = new MyBroadcastReceiver();
        }

        public override void OnMessageReceived(RemoteMessage message)
        {
            //ToastUser("From: " + message.From);
            System.Console.WriteLine("MYF: Message recieved"); //Console is not found in system
            var name = string.Empty; ;
            if (message.Data.Count > 0)
            {
                if (message.Data.ContainsKey("title"))
                {
                    name = message.Data["title"];
                    //ToastUser("inform user of dinner is ready");
                    System.Console.WriteLine("MYF:DInner is ready"); //Console is not found in system
                    SendNotification("Dinner is Ready");
                    Vibration();
                    Task.Run(async () => await InformmasterAsync());
                }
                else if (message.Data.ContainsKey("Action"))
                {
                    switch (message.Data["Action"])
                    {
                        case "cancelVibration":
                            //ToastUser("Vibrations Canceled");
                            SendNotification("CANCELED");
                            if (timer.Enabled)
                            {
                                //timer.Elapsed -= Timer_Elapsed;
                                timer.Stop();
                            }

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
            // Check if message contains a notification payload.
            //if (message.GetNotification() != null)
            //{
            //    SendNotification("Dinner is Ready");
            //    Vibration();
            //    Task.Run(async () => await InformmasterAsync());
            //}
        }

        private void Vibration()
        {
            Vibrate();

            if (!timer.Enabled)
            {
                timer.Interval = 1000; // runs every second
                timer.Elapsed += Timer_Elapsed;
                timer.Start();
            }
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
                Vibrate();
            }
        }

        private void Vibrate()
        {
            Vibrator vibrator = (Vibrator)this.ApplicationContext.GetSystemService(Context.VibratorService);
            long[] vibPatterns = GetVibPatterns();

            vibrator.Vibrate(vibPatterns, -1);

            // ________Code for Newer Androdid levle 26 with VibrationEffect________

            //if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            //{
            //    if (vibrator.HasAmplitudeControl)
            //    {
            //        VibrationEffect effect = VibrationEffect.CreateOneShot(1000, VibrationEffect.DefaultAmplitude);
            //        vibrator.Vibrate(effect);
            //    }
            //    else
            //    {
            //        vibrator.Vibrate(vibPatterns, -1);
            //    }
            //}
            //--------------END CODE --------------

            //<--- Only works in android api 26 and up androud 8 --->
            //VibrationEffect effect = VibrationEffect.CreateOneShot(1000, VibrationEffect.DefaultAmplitude);
            //vibrator.Vibrate(effect);
            //<--- END --->
        }

        private static long[] GetVibPatterns()
        {
            int _vibtype = ActiveUser.VibType;
            long[] pattern = new long[] { 100, 2000, 90, 2000, 80 };
            switch (_vibtype)
            {
                case 1:
                    pattern = new long[] { 20, 250, 20, 250, 20, 1000, 20, 350, 20, 350, 20 };
                    break;

                case 2:
                    pattern = new long[] { 500, 50, 500, 50, 500, 50, 500, 350, 500, 350, 500 };
                    break;

                default:
                    break;
            }
            return pattern;
        }

        private MyBroadcastReceiver deleteReceiver;
        private static readonly int REQUEST_CODE = 2323;
        public readonly string ACTION_NOTIFICATION_DELETE = "com.xamarin.Droid_MyFirebaseMesagingService.delete";

        private void SendNotification(string messageBody)
        {
            Intent intent = new Intent("CancelVibrations");
            intent.PutExtra("fromNotification", true);
            var pendingIntent = PendingIntent.GetBroadcast(this, 1, intent, PendingIntentFlags.UpdateCurrent);

            var notificationBuilder = new Android.App.Notification.Builder(this)
                    .SetSmallIcon(Resource.Drawable.abc_btn_check_material)
                    .SetContentTitle("FCM Message")
                    .SetContentText(messageBody)
                    .SetAutoCancel(true)
                    .SetDeleteIntent(pendingIntent)
                    .SetContentIntent(pendingIntent);

            var notificationManager = NotificationManager.FromContext(this);
            notificationManager.Notify(0, notificationBuilder.Build());
        }

        private object CancelDinner()
        {
            //throw new NotImplementedException();
        }

        public async Task InformmasterAsync()
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

        private StopVibrationReceiver mBroadcastReceiver;
    }
}