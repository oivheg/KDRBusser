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
using Firebase.Auth;
using Plugin.Toasts;
using Xamarin.Forms;

namespace KDRBusser.Droid
{
    class FCmLogin
    {

        public async System.Threading.Tasks.Task CreateUserAsync(String email, String password)
        {



            try
            {
                await FirebaseAuth.Instance.CreateUserWithEmailAndPasswordAsync(email, password);
            }
            catch (Exception ex)
            {

                // Sign-up failed, display a message to the user
                // If sign in succeeds, the AuthState event handler will
                //  be notified and logic to handle the signed in user can happen there

                //Toast.MakeText(this, "Sign In failed", ToastLength.Short).Show();
                var options = new NotificationOptions()
                {
                    Title = "Failed",
                    Description = "User Creation failed",
                    IsClickable = false // Set to true if you want the result Clicked to come back (if the user clicks it)
                };
                var notification = DependencyService.Get<IToastNotificator>();
                var result = await notification.Notify(options);

            }
        }
    }

 
}