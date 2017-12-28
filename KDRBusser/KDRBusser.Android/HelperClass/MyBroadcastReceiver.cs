﻿using Android.Content;
using Plugin.Toasts;
using System;
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