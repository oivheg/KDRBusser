﻿using System;
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
using Android.Gms.Tasks;
using KDRBusser.Droid;
using Firebase.Auth;
using Firebase;
using Android.Support.V7.App;

[assembly: Dependency(typeof(FCMLoginService))]
namespace KDRBusser.Droid
{
    class FCMLoginService : AppCompatActivity, IFCMLoginService
    {

        // [START declare_auth]
        //FirebaseAuth mAuth;
        
        // [END declare_auth]

        public void Createuser(String email, String password)
        {
            CreateUserAsync(email, password);
        }

        
        
            
        public async void CreateUserAsync(String email, String password)
        {
            try
            {

                await FirebaseAuth.Instance.CreateUserWithEmailAndPasswordAsync(email, password);
                ToastedUserAsync("FCM User Created");
            }
            catch (Exception ex)
            {
                // Sign-up failed, display a message to the user
                // If sign in succeeds, the AuthState event handler will
                //  be notified and logic to handle the signed in user can happen there
                ToastedUserAsync("Create user failed" + ex);
            }

        }



        public void LogInnUser(String email, String password)
        {

            LogInUserAsync(email, password);
        }

        public async void LogInUserAsync(String email, string password)
        {
            try
            {
                Firebase.FirebaseApp.InitializeApp(this);

                await FirebaseAuth.Instance.SignInWithEmailAndPasswordAsync(email, password);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // Sign-in failed, display a message to the user
                // If sign in succeeds, the AuthState event handler will
                //  be notified and logic to handle the signed in user can happen there
                ToastedUserAsync("Sign In failed" + ex);
            }
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
                Description = "Toasted from android",
                IsClickable = false // Set to true if you want the result Clicked to come back (if the user clicks it)
            };
            var notification = DependencyService.Get<IToastNotificator>();
            var result = await notification.Notify(options);

        }

        
        public void Init()
        {
            var firebaseapp = FirebaseApp.InitializeApp(this);


            throw new NotImplementedException();
        }
    }
}