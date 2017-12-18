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
using Plugin.Toasts;
using Xamarin.Forms;

namespace KDRBusser.Droid.HelperClass
{
    public class MyBroadcastReceiver : BroadcastReceiver
    {


        public override void OnReceive(Context context, Intent intent)
        {
            ToastedUserAsync("Notification cancel/dismissed registered");
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