using Android.App;
using Android.Content;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
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
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(FCMLoginService))]

namespace StaffBusser.Droid
{
    [Activity(Label = "FCM Login")]
    public class FCMLoginService : AppCompatActivity, IFCMLoginService, GoogleApiClient.IOnConnectionFailedListener, GoogleApiClient.IConnectionCallbacks
    {
        private const string TAG = "FCMActivity";

        //[START declare_auth]
        private FirebaseAuth mAuth;

        private object mGoogleSignInClient;
        private GoogleApiClient mGoogleApiClient;

        public string GetEmail()
        {
            return mAuth.CurrentUser.Email;
        }

        public void Init()
        {
            // [START config_signin]
            // Configure Google Sign In
            var gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                    .RequestIdToken("1084249644121-cv7dvgf3tmpcoh5gbiqatamfuceurejn.apps.googleusercontent.com")
                    .RequestEmail()
                    .Build();
            // [END config_signin]

            Device.BeginInvokeOnMainThread(() =>
            {
                Context ct = Xamarin.Forms.Forms.Context;
                mGoogleApiClient = new GoogleApiClient.Builder(ct)
                   .EnableAutoManage(this /* FragmentActivity */, this /* OnConnectionFailedListener */)
                   .AddApi(Android.Gms.Auth.Api.Auth.GOOGLE_SIGN_IN_API, gso)
                   .Build();
            });

            mAuth = FirebaseAuth.Instance;
            mAuth.AuthState += AuthStateChangedAsync;
        }

        public async void UpdateTokenAsync()
        {
            if (mAuth.CurrentUser != null)
            {
                await SharedHelper.UpdateUserTokenAsync(mAuth.CurrentUser.Email, GetToken()).ConfigureAwait(false);
            }
        }

        //private GoogleApiClient mGoogleApiClient;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // [START config_signin]
            // Configure Google Sign In
            var gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                    .RequestIdToken("1084249644121-cv7dvgf3tmpcoh5gbiqatamfuceurejn.apps.googleusercontent.com")
                    .RequestEmail()
                    .Build();
            // [END config_signin]

            mGoogleApiClient = new GoogleApiClient.Builder(this)
                    .EnableAutoManage(this /* FragmentActivity */, this /* OnConnectionFailedListener */)
                    .AddApi(Android.Gms.Auth.Api.Auth.GOOGLE_SIGN_IN_API, gso)
                    .Build();
        }

        public int RC_SIGN_IN = 9001;

        //protected override void OnActivityResultAsync(int requestCode, Result resultCode, Intent data)
        //{
        //    base.OnActivityResult(requestCode, resultCode, data);

        //    // Result returned from launching the Intent from GoogleSignInApi.getSignInIntent(...);
        //    if (requestCode == 9001)
        //    {
        //        var result = Android.Gms.Auth.Api.Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
        //        if (result.IsSuccess)
        //        {
        //            // Google Sign In was successful, authenticate with Firebase
        //            FirebaseAuthWithGoogle(result.SignInAccount);
        //        }
        //        else
        //        {
        //            // Google Sign In failed, update UI appropriately
        //            // [START_EXCLUDE]
        //            // UpdateUI(null);
        //            // [END_EXCLUDE]
        //        }
        //    }
        //}

        // [START auth_with_google]
        public async void FirebaseAuthWithGoogle(GoogleSignInAccount acct)
        {
            Android.Util.Log.Debug(TAG, "firebaseAuthWithGoogle:" + acct.Id);
            // [START_EXCLUDE silent]
            //   ShowProgressDialog();
            // [END_EXCLUDE]

            AuthCredential credential = GoogleAuthProvider.GetCredential(acct.IdToken, null);

            try
            {
                await mAuth.SignInWithCredentialAsync(credential);
            }
            catch
            {
                //   Toast.MakeText(this, "Authentication failed.", ToastLength.Short).Show();
            }
            // [START_EXCLUDE]
            // HideProgressDialog();
            // [END_EXCLUDE]
        }

        // [END onactivityresult]
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
            response = await RestApiCommunication.PostMasterKey(Mstr, "ChckKey").ConfigureAwait(false);

            if (!response)
            {
                DependencyService.Get<IHelperClass>().IsLoading(false, "Creating User");
                await App.Current.MainPage.DisplayAlert("Feil i MasterID", "Sjekk tegn og prøv på nytt", "OK").ConfigureAwait(false);
            }
            else
            {
                try
                {
                    IAuthResult FResults = await FirebaseAuth.Instance.CreateUserWithEmailAndPasswordAsync(email, password).ConfigureAwait(false);
                    User newUser = new User
                    {
                        Email = email,
                        UserName = UserName.Trim(),
                        MasterKey = masterid.Trim(),
                        Appid = GetToken(),
                        Active = false
                    };

                    await RestApiCommunication.Post(newUser, "CreateUser").ConfigureAwait(false);

                    //ToastedUserAsync("FCM User Created");
                }
                catch (Exception ex)
                {
                    // Sign-up failed, display a message to the user
                    // If sign in succeeds, the AuthState event handler will
                    //  be notified and logic to handle the signed in user can happen there
                    DependencyService.Get<IHelperClass>().IsLoading(false);
                    await App.Current.MainPage.DisplayAlert("Bruker eksisterer", "Vennligst gå tilbake og logg inn", "OK").ConfigureAwait(false);
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
                await mAuth.SignInWithEmailAndPasswordAsync(email, password).ConfigureAwait(false);
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
                await App.Current.MainPage.DisplayAlert("Feil", "Vennligst sjekk epost og passord", "OK").ConfigureAwait(false);
            }
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
            await SharedHelper.UpdateUserTokenAsync(mAuth.CurrentUser.Email, GetToken(), true).ConfigureAwait(false);
            FirebaseAuth.Instance.SignOut();
        }

        //Gogle is stil in test phase, migt not needed at all ?
        private Activity activity = (MainActivity)Forms.Context;

        public void LogInGoogle()
        {
            var signInIntent = new Intent(Android.Gms.Auth.Api.Auth.GoogleSignInApi.GetSignInIntent(mGoogleApiClient));
            activity.StartActivityForResult(signInIntent, RC_SIGN_IN);
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