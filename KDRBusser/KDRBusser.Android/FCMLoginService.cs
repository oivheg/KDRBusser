﻿using System;
using Plugin.Toasts;
using Xamarin.Forms;
using KDRBusser.Droid;
using Android.Support.V7.App;
using Firebase.Iid;
using Firebase.Auth;
using Android.OS;
using Firebase;
using KDRBusser.Communication;
using KDRBusser.Classes;
using System.ComponentModel;

[assembly: Dependency(typeof(FCMLoginService))]
namespace KDRBusser.Droid
{
    class FCMLoginService : AppCompatActivity, IFCMLoginService
    {
        //[START declare_auth]
        FirebaseAuth mAuth;

        

        public string GetEmail()
        {
            return mAuth.CurrentUser.Email;


        }



        public void Init()
        {
            DependencyService.Register<MyFirebaseMessagingService>();
            mAuth = FirebaseAuth.Instance;
            mAuth.AuthState += AuthStateChanged;
            
        }

        public void UpdateToken(String Token)
        {
            FCMToken = Token;
            UpdateUserToken();
        }

        String FCMToken;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // [START initialize_auth]
            //mAuth = FirebaseAuth.Instance;
            // [END initialize_auth]
        }
        

        protected override void OnStart()
        {
            base.OnStart();
            //mAuth.AuthState += AuthStateChanged;
        }

        protected override void OnStop()
        {
            base.OnStop();
            mAuth.AuthState -= AuthStateChanged;
        }
        public void Createuser(String email, String password, String masterid, String UserName)
        {
           
            CreateUserAsync(email, password, masterid, UserName);
        }

        public async void CreateUserAsync(String email, String password, String masterid, String UserName)
        {
            
            try
            {
                User newUser = new User
                {
                    Email = email,
                    UserName = UserName.Trim(),
                    MasterKey = masterid.Trim(),
                    Appid = "not initialised".Trim(),
                    Active = false
                };
                await RestApiCommunication.Post(newUser, "CreateUser");

                await FirebaseAuth.Instance.CreateUserWithEmailAndPasswordAsync(email, password);
                //ToastedUserAsync("FCM User Created");
             
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
                await mAuth.SignInWithEmailAndPasswordAsync(email, password);
                ToastedUserAsync("Sign In Success ");

                await UpdateUserToken();
                App.IsUserLoggedIn = true;
              
                Xamarin.Forms.Application.Current.MainPage = new ActiveUser();

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

        public async System.Threading.Tasks.Task UpdateUserToken()
        {
            User user = new User
            {
                Email = mAuth.CurrentUser.Email,
                Appid = GetToken()
            };
          await RestApiCommunication.Post(user, "UpdatUser");
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

        void AuthStateChanged(object sender, FirebaseAuth.AuthStateEventArgs e)
        {
            var user = e.Auth.CurrentUser;
            if (user != null)
            { FirebaseApp.InitializeApp(this);
                // User is signed in
                //ToastedUserAsync("onAuthStateChanged:signed_in:" + user.Uid);
                App.IsUserLoggedIn = true;
                UpdateUserToken();
                Xamarin.Forms.Application.Current.MainPage = new ActiveUser();
            }
            else
            {
                
                // User is signed out
               //ToastedUserAsync("onAuthStateChanged:signed_out");
                App.IsUserLoggedIn = false;
                Xamarin.Forms.Application.Current.MainPage = new FCmLogin();
            }
            // [START_EXCLUDE]
            //UpdateUI(user);
            // [END_EXCLUDE]
        
        }


        //not in use ate the momend, chaneg with authstate listener.
        public Boolean IsLoggedIn()
        {

            // this is where the FIREBASE system is initialized. every firebase related initilasion shoudl start here, at lest for now.

            //var firebaseapp = FirebaseApp.InitializeApp(this);

            var user = mAuth.CurrentUser;

            var signedIn = user != null;

         
            if (signedIn)
            {
                UpdateUserToken();
            }

            return signedIn;
        }

        public string GetToken()
        {
            
            String tkn = FirebaseInstanceId.Instance.Id;
            var  mauttoken = mAuth.CurrentUser.Uid;
          
            var refreshedToken = FirebaseInstanceId.Instance.Token;

            if (refreshedToken == null)
            {
              
                LogOut();
                //mAuth.SignOut();

            }



            return refreshedToken;
        }

        public void LogOut()
        {
            FirebaseAuth.Instance.SignOut();
          
        }

        
    }
}