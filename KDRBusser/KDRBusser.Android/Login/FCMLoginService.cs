using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Firebase;
using Firebase.Auth;
using Firebase.Iid;
using KDRBusser.Classes;
using KDRBusser.Communication;
using KDRBusser.Droid;
using KDRBusser.SharedCode;
using Plugin.Toasts;
using System;
using Xamarin.Forms;

[assembly: Dependency(typeof(FCMLoginService))]

namespace KDRBusser.Droid
{
    [Activity(Label = "FCM Login")]
    internal class FCMLoginService : AppCompatActivity, IFCMLoginService
    {
        private const string TAG = "FCMActivity";

        //[START declare_auth]
        private FirebaseAuth mAuth;

        public string GetEmail()
        {
            return mAuth.CurrentUser.Email;
        }

        public void Init()
        {
            mAuth = FirebaseAuth.Instance;
            mAuth.AuthState += AuthStateChangedAsync;
        }

        public void UpdateToken(String Token)
        {
            FCMToken = Token;
            SharedHelper.UpdateUserTokenAsync(mAuth.CurrentUser.Email, GetToken());
        }

        private String FCMToken;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnStop()
        {
            base.OnStop();
            mAuth.AuthState -= AuthStateChangedAsync;
        }

        public void Createuser(String email, String password, String masterid, String UserName)
        {
            //ShowProgressDialog(this);
            DependencyService.Get<IHelperClass>().IsLoading(true, "Creating User");
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
            //ShowProgressDialog(this);
            LogInUserAsync(email, password);
        }

        public async void LogInUserAsync(String email, string password)
        {
            try
            {
                await mAuth.SignInWithEmailAndPasswordAsync(email, password);
                // sign in sucess message
                //ToastedUserAsync("Sign In Success ");

                await SharedHelper.UpdateUserTokenAsync(mAuth.CurrentUser.Email, GetToken());
                App.IsUserLoggedIn = true;

                //ChangeActivity();
            }
            catch (Exception ex)
            {
                DependencyService.Get<IHelperClass>().IsLoading(false);
                Console.WriteLine(ex);
                // Sign-in failed, display a message to the user
                // If sign in succeeds, the AuthState event handler will
                //  be notified and logic to handle the signed in user can happen there
                ToastedUserAsync("Sign In failed" + ex);
            }
        }

        //public async System.Threading.Tasks.Task UpdateUserToken(String _email, String _appid,Boolean logout = false)
        //{
        //    User user = new User
        //    {
        //        Email = _email,
        //        Appid = _appid
        //    };
        //    if (logout)
        //    {
        //        user.Appid = "logged Out";
        //    }
        //    await RestApiCommunication.Post(user, "UpdatUser");
        //}

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

        private async void AuthStateChangedAsync(object sender, FirebaseAuth.AuthStateEventArgs e)
        {
            var user = e.Auth.CurrentUser;
            if (user != null)
            {
                DependencyService.Get<IHelperClass>().IsLoading(true, "Logger INN");
                FirebaseApp.InitializeApp(this);
                // User is signed in
                //ToastedUserAsync("onAuthStateChanged:signed_in:" + user.Uid);
                App.IsUserLoggedIn = true;
                await SharedHelper.UpdateUserTokenAsync(mAuth.CurrentUser.Email, GetToken());
                ChangeActivity();
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

        private static void ChangeActivity()

        {
            Xamarin.Forms.Application.Current.MainPage = new NavigationPage(new ActiveUser());
        }

        public string GetToken()
        {
            String tkn = FirebaseInstanceId.Instance.Id;
            var mauttoken = mAuth.CurrentUser.Uid;

            var refreshedToken = FirebaseInstanceId.Instance.Token;

            if (refreshedToken == null)
            {
                LogOut();
                //mAuth.SignOut();
            }
            return refreshedToken;
        }

        public async void LogOut()
        {
            await SharedHelper.UpdateUserTokenAsync(mAuth.CurrentUser.Email, GetToken(), true);
            FirebaseAuth.Instance.SignOut();
        }

        //Gogle is stil in test phase, migt not needed at all ?
        public void LogInGoogle()
        {
            //StartActivity(typeof(GoogleSignInActivity));

            //Intent i = new Intent(this, typeof(GoogleSignInActivity));
            //this.StartActivity(i);

            //Xamarin.Forms.Application.Current.MainPage = new GoogleSignInActivity();
        }
    }
}