using Android;
using Android.App;
using Android.Content;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Tasks;
using Android.OS;
using Android.Support.V7.App;
using Firebase;
using Firebase.Auth;
using Firebase.Iid;
using StaffBusser.Classes;
using StaffBusser.Communication;
using StaffBusser.Droid;
using StaffBusser.SharedCode;
using System;
using Xamarin.Forms;

[assembly: Dependency(typeof(FCMLoginService))]

namespace StaffBusser.Droid
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

        public async void UpdateTokenAsync(String Token)
        {
            FCMToken = Token;
            if (mAuth.CurrentUser != null)
            {
                await SharedHelper.UpdateUserTokenAsync(mAuth.CurrentUser.Email, GetToken());
            }
        }

        private String FCMToken;
        //private GoogleApiClient mGoogleApiClient;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //  GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
            //.RequestIdToken("810661396254-d05a62hlbilehtl9ld5neonh64n40sq8.apps.googleusercontent.com")
            //.RequestEmail()
            //.Build();
            //  mGoogleApiClient = new GoogleApiClient.Builder(this)
            //      .EnableAutoManage(this, this)
            //      .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
            //      .AddConnectionCallbacks(this)
            //      .AddOnConnectionFailedListener(this)

            //      .Build();
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
            User Mstr = new User
            {
                MasterKey = masterid.Trim()
            };
            Boolean response;
            response = await RestApiCommunication.PostMasterKey(Mstr, "ChckKey");

            if (!response)
            {
                DependencyService.Get<IHelperClass>().IsLoading(false, "Creating User");
                await App.Current.MainPage.DisplayAlert("Feil i MasterID", "Sjekk tegn og prøv på nytt", "OK");
            }
            else
            {
                try
                {
                    IAuthResult FResults = await FirebaseAuth.Instance.CreateUserWithEmailAndPasswordAsync(email, password);
                    User newUser = new User
                    {
                        Email = email,
                        UserName = UserName.Trim(),
                        MasterKey = masterid.Trim(),
                        Appid = GetToken(),
                        Active = false
                    };

                    await RestApiCommunication.Post(newUser, "CreateUser");

                    //ToastedUserAsync("FCM User Created");
                }
                catch (Exception ex)
                {
                    // Sign-up failed, display a message to the user
                    // If sign in succeeds, the AuthState event handler will
                    //  be notified and logic to handle the signed in user can happen there
                    DependencyService.Get<IHelperClass>().IsLoading(false);
                    await App.Current.MainPage.DisplayAlert("Bruker eksisterer", "Vennligst gå tilbake og logg inn", "OK");
                    //DependencyService.Get<IHelperClass>().isAlert();

                    //SharedHelper.ToastedUserAsync("Create user failed" + ex);
                }
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

                //await SharedHelper.UpdateUserTokenAsync(mAuth.CurrentUser.Email, GetToken());
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
                //SharedHelper.ToastedUserAsync("Sign In failed" + ex);
                await App.Current.MainPage.DisplayAlert("Feil", "Vennligst sjekk epost og passord", "OK");
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

        //public void ToastUser(String title)
        //{
        //    ToastedUserAsync(title);
        //}

        //public async void ToastedUserAsync(String title)
        //{
        //    var options = new NotificationOptions()
        //    {
        //        Title = title,
        //        Description = "Toasted from android",
        //        IsClickable = false // Set to true if you want the result Clicked to come back (if the user clicks it)
        //    };
        //    var notification = DependencyService.Get<IToastNotificator>();
        //    var result = await notification.Notify(options);
        //}

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
            //var signInIntent = Auth.GoogleSignInApi.GetSignInIntent(mGoogleApiClient);
            //mGoogleApiClient.Connect();
        }

        private void FirebaseAuthWithGoogle(GoogleSignInAccount acct)
        {
            // Log.Debug(Tag, "FirebaseAuthWithGoogle:" + acct.Id);
            //AuthCredential credential = GoogleAuthProvider.GetCredential(acct.IdToken, null);
            //mAuth.SignInWithCredential(credential).AddOnCompleteListener(this, this);
        }

        public void CancelVIbrations()
        {
            Droid_MyFirebaseMessagingService.CancelTimerVibration();
        }

        public void OnConnected(Bundle connectionHint)
        {
            throw new NotImplementedException();
        }

        public void OnConnectionSuspended(int cause)
        {
            throw new NotImplementedException();
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            throw new NotImplementedException();
        }
    }
}