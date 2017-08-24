using System;
using Plugin.Toasts;
using Xamarin.Forms;
using KDRBusser.Droid;
using Android.Support.V7.App;
using Firebase.Iid;
using Firebase.Auth;
using Android.OS;
using Firebase;

[assembly: Dependency(typeof(FCMLoginService))]
namespace KDRBusser.Droid
{
    class FCMLoginService : AppCompatActivity, IFCMLoginService
    {
        //[START declare_auth]
        FirebaseAuth mAuth;

        //[END declare_auth]

        public void Init()
        {
        
            mAuth = FirebaseAuth.Instance;
            mAuth.AuthState += AuthStateChanged;
        }
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
               

                await mAuth.SignInWithEmailAndPasswordAsync(email, password);
                ToastedUserAsync("Sign In Success ");

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


        public Boolean IsLoggedIn()
        {

            // this is where the FIREBASE system is initialized. every firebase related initilasion shoudl start here, at lest for now.
           
            //var firebaseapp = FirebaseApp.InitializeApp(this);

            var user = mAuth.CurrentUser;
            var signedIn = user != null;

            return signedIn;
        }

        public string GetToken()
        {
            
            String tkn = FirebaseInstanceId.Instance.Id;
            var refreshedToken = FirebaseInstanceId.Instance.Token;


            return refreshedToken;
        }

        public void LogOut()
        {
            FirebaseAuth.Instance.SignOut();
          
        }

        
    }
}