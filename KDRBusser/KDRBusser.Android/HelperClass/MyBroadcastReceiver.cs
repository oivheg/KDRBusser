using Android.Content;
using StaffBusser.SharedCode;
using Plugin.Toasts;
using System;
using Xamarin.Forms;

namespace StaffBusser.Droid.HelperClass
{
    public class MyBroadcastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            //ToastedUserAsync("Notification cancel/dismissed registered");
            SharedHelper.ToastedUserAsync("Notification cancel / dismissed registered");
        }

        //public async void ToastedUserAsync(String title)
        //{
        //    var options = new NotificationOptions()
        //    {
        //        Title = title,
        //        Description = "StaffBusser",
        //        IsClickable = false // Set to true if you want the result Clicked to come back (if the user clicks it)
        //    };

        //    var notification = DependencyService.Get<IToastNotificator>();
        //    var result = await notification.Notify(options);
        //}
    }
}