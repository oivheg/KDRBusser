﻿using Android.App;
using Android.Content;
using Firebase.Iid;
using Plugin.Toasts;
using System;
using Xamarin.Forms;

namespace KDRBusser.Droid
{
    [Service]
    [BroadcastReceiver]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class MyFirebaseIIDService : FirebaseInstanceIdService
    {
        private const string TAG = "MyFirebaseIIDService";

        public override void OnTokenRefresh()
        {
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            ToastUser("Refreshed token: " + refreshedToken);

            SendRegistrationToServerAsync(refreshedToken);
        }

        public void ToastUser(String title)
        {
            ToastedUserAsync(title);
        }

        public void OnReceive(Context context, Intent intent)
        {
            ToastedUserAsync("onReceived");
        }

        public async void ToastedUserAsync(String title)
        {
            var options = new NotificationOptions()
            {
                Title = title,
                Description = "Toasted from android",
                IsClickable = false // Set to true if you want the result Clicked to come back (if the user clicks it)
            };
            var notification = DependencyService.Get<IToastNotificator>();
            var result = await notification.Notify(options);
        }

        private async void SendRegistrationToServerAsync(string token)
        {
            String tkn = token;
            DependencyService.Get<IFCMLoginService>().UpdateToken(tkn);
        }
    }
}