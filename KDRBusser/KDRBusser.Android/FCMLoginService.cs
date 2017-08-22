using System;
using Plugin.Toasts;
using Xamarin.Forms;
using KDRBusser.Droid;
using Firebase.Auth;
using Firebase;
using Android.Support.V7.App;
using Firebase.Iid;

[assembly: Dependency(typeof(FCMLoginService))]
namespace KDRBusser.Droid
{
    class FCMLoginService : AppCompatActivity, IFCMLoginService
    {
        

        //[START declare_auth]
        //FirebaseAuth mAuth;

        //[END declare_auth]

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
                //FirebaseApp.InitializeApp(this);

                await FirebaseAuth.Instance.SignInWithEmailAndPasswordAsync(email, password);
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


        public Boolean IsLoggedIn()
        {

            // this is where the FIREBASE system is initialized. every firebase related initilasion shoudl start here, at lest for now.
            var firebaseapp = FirebaseApp.InitializeApp(this);

            var user = FirebaseAuth.Instance.CurrentUser;
            var signedIn = user != null;

            return signedIn;
        }

        public string GetToken()
        {

            String tkn = FirebaseInstanceId.Instance.Token;
            var refreshedToken = FirebaseInstanceId.Instance.Token;
          
            return refreshedToken;
        }
    }
}